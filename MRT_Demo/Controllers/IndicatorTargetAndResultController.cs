using MRT_Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;


namespace MRT_Demo.Controllers
{
    public class indicatorTargetAndResultController : Controller
    {
        MRTEntities db = new MRTEntities();
        static string _SceneView = "";
        static int? _IndicatorID;
        static List<SelectListItem> _IndicatorTypeDropdownList = new List<SelectListItem>();
        static List<SelectListItem> _IndicatorUnitDropdownList = new List<SelectListItem>();

        List<SelectListItem> _predicOwnerDropdownList = new List<SelectListItem>()
        {
            new SelectListItem(){Text = "คาดการณ์หน่วย1"},
            new SelectListItem(){Text = "คาดการณ์หน่วย2"},
            new SelectListItem(){Text = "คาดการณ์หน่วย3"},
        };

        public ActionResult Index(int? year, string divisionSearch, string indicatorSearch)
        {
            ClearStatic();
            List<Indicator> indicators = db.Indicator.ToList();
            indicators = indicators.Where(s => s.IsDelete == false).ToList();
            return View(indicators);
        }

        public ActionResult Target(int? id)
        {
            ClearStatic();
            _SceneView = "Target";
            if (id == null)
            {
                return HttpNotFound();
            }
            _IndicatorID = id;
            Indicator indicator = db.Indicator.Find(id);
            indicator.ImportantIndicatorTargetMeasurement = indicator.ImportantIndicatorTargetMeasurement.Where(s => s.IsDelete == false).ToList();
            if (indicator == null)
            {
                return HttpNotFound();
            }

            indicator = SetupTargetMeasurementFromDB(indicator);
            List<ImportantIndicatorTargetMeasurement> temp = new List<ImportantIndicatorTargetMeasurement>();
            
            foreach (var i in indicator.IndicatorXIndicatorType)
            {
                if (i.IsCheck)
                {
                    ImportantIndicatorTargetMeasurement measurement = new ImportantIndicatorTargetMeasurement();
                    measurement.IndicatorType = i.IndicatorType;
                    SelectListItem selectListItem = new SelectListItem() { Text = i.IndicatorType.IndicatorType1, Value = i.IndicatorTypeID.ToString() };
                    _IndicatorTypeDropdownList.Add(selectListItem);

                    var imTargetTypeDisplay = InitImportantIndicatorTargetMeasurement();
                    imTargetTypeDisplay.IndicatorTypeID = i.IndicatorTypeID;
                    imTargetTypeDisplay.IsDispaly = true;
                    imTargetTypeDisplay.IndicatorID = indicator.ID;
                    temp.Add(imTargetTypeDisplay);
                }
            }
            temp.AddRange(indicator.ImportantIndicatorTargetMeasurement.ToList());
            indicator.ImportantIndicatorTargetMeasurement = temp;

            foreach (var i in indicator.IndicatorUnit)
            {
                SelectListItem selectListItem = new SelectListItem() { Text = i.Unit, Value = i.ID.ToString() };
                _IndicatorUnitDropdownList.Add(selectListItem);
            }

            ViewbagDropdown();

            return View(indicator);
        }

        public ActionResult AddPredictOwner(Indicator indicator)
        {
            return View();
        }

        public ActionResult AddTargetMeasurement(Indicator indicator)
        {
            ModelState.Clear();
            indicator.ImportantIndicatorTargetMeasurement.Add(InitImportantIndicatorTargetMeasurement());
            ViewbagDropdown();
            return View(_SceneView, indicator);
        }

        public ActionResult DeleteTargetMeasurement(Indicator indicator)
        {
            ModelState.Clear();
            foreach (var i in indicator.ImportantIndicatorTargetMeasurement)
            {
                if (i.IsDeleteTarget)
                {
                    if (i.ID == 0)
                    {
                        var tempList = indicator.ImportantIndicatorTargetMeasurement.ToList();
                        tempList.Remove(i);
                        indicator.ImportantIndicatorTargetMeasurement = tempList;
                    }
                    else
                    {
                        i.IsDelete = true;
                    }
                }
            }
            ViewbagDropdown();
            return View(_SceneView, indicator);
        }

        [HttpPost]
        public ActionResult Target(Indicator indicator)
        {
            foreach (var i in indicator.ImportantIndicatorTargetMeasurement)
            {
                if (i.IsDelete == true)
                {
                    foreach (var j in i.TargetList)
                    {
                        db.Entry(j).State = EntityState.Modified;
                        j.UpdateDate = DateTime.Now;
                        j.IndicatorTypeID = i.IndicatorTypeID;
                        j.IndicatorUnitID = i.IndicatorUnitID;
                        j.IsDelete = true;
                    }
                }
                else
                {
                    foreach (var j in i.TargetList)
                    {
                        j.UpdateDate = DateTime.Now;
                        if (j.ID == 0)
                        {
                            j.IndicatorTypeID = i.IndicatorTypeID;
                            j.IndicatorUnitID = i.IndicatorUnitID;
                            j.CreateDate = DateTime.Now;
                            j.IndicatorID = _IndicatorID;
                            j.IsDelete = false;
                            db.ImportantIndicatorTargetMeasurement.Add(j);
                        }
                        else
                        {
                            db.Entry(j).State = EntityState.Modified;
                            j.IndicatorTypeID = i.IndicatorTypeID;
                            j.IndicatorUnitID = i.IndicatorUnitID;
                        }
                    }
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Result(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            _IndicatorID = id;
            Indicator indicator = db.Indicator.Find(id);
            indicator.ImportantIndicatorTargetMeasurement = indicator.ImportantIndicatorTargetMeasurement.Where(s => s.IsDelete == false).ToList();
            if (indicator == null)
            {
                return HttpNotFound();
            }
            indicator = SetupTargetMeasurementFromDB(indicator);
            ViewbagDropdown();
            return View(indicator);
        }

        public ActionResult ChangePeriod(Indicator indicator)
        {
            return View();
        }
        public ActionResult ChangeMonthQuarterHailfYear(Indicator indicator)
        {
            return View();
        }
        public ActionResult AddCompetitorValue(Indicator indicator)
        {
            return View();
        }
        public ActionResult ChangeFile(Indicator indicator)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Reuslt(Indicator indicator)
        {
            return View();
        }
        public ActionResult Report(int? sOEPlanID)
        {
            return View();
        }

        private void ViewbagDropdown()
        {
            ViewBag.IndicatorTypeDropdownList = _IndicatorTypeDropdownList;
            ViewBag.IndicatorUnitDropdownList = _IndicatorUnitDropdownList;
            ViewBag.PredicOwnerDropdownList = _predicOwnerDropdownList;
        }

        private void ClearStatic()
        {
            _IndicatorID = null;
            _IndicatorTypeDropdownList.Clear();
            _IndicatorUnitDropdownList.Clear();
        }

        private ImportantIndicatorTargetMeasurement InitImportantIndicatorTargetMeasurement()
        {
            ImportantIndicatorTargetMeasurement importantIndicatorTargetMeasurement = new ImportantIndicatorTargetMeasurement();

            List<ImportantIndicatorTargetMeasurement> targetList = new List<ImportantIndicatorTargetMeasurement>();
            int range = 5;

            for (int i = 0; i < range; i++)
            {
                ImportantIndicatorTargetMeasurement importantIndicatorTargetMeasurement1 = new ImportantIndicatorTargetMeasurement();
                importantIndicatorTargetMeasurement1.IndicatorLevel = i + 1;
                targetList.Add(importantIndicatorTargetMeasurement1);
            }
            importantIndicatorTargetMeasurement.TargetList = targetList;
            importantIndicatorTargetMeasurement.IsDelete = false;
            return importantIndicatorTargetMeasurement;
        }

        private Indicator SetupTargetMeasurementFromDB(Indicator indicator)
        {
            int row = indicator.ImportantIndicatorTargetMeasurement.Count / 5;
            int currentPoint = 0;
            int childRange = 0;
            List<ImportantIndicatorTargetMeasurement> imTargetCopy = new List<ImportantIndicatorTargetMeasurement>();
            for (int i = 0; i < row; i++)
            {
                ImportantIndicatorTargetMeasurement imTarget = new ImportantIndicatorTargetMeasurement();
                List<ImportantIndicatorTargetMeasurement> tem = new List<ImportantIndicatorTargetMeasurement>();
                childRange += 5;

                for (int j = currentPoint; j < childRange; j++)
                {
                    tem.Add(indicator.ImportantIndicatorTargetMeasurement.ToList()[j]);
                }

                imTarget.TargetList = tem;

                imTarget.ID = imTarget.TargetList[0].ID;
                imTarget.IndicatorID = imTarget.TargetList[0].IndicatorID;
                imTarget.IndicatorUnitID = imTarget.TargetList[0].IndicatorUnitID;
                imTarget.IndicatorTypeID = imTarget.TargetList[0].IndicatorTypeID;
                imTarget.IsDelete = false;

                imTargetCopy.Add(imTarget);
                currentPoint = childRange;
            }
            indicator.ImportantIndicatorTargetMeasurement = imTargetCopy;
            return indicator;

        }

    }
}