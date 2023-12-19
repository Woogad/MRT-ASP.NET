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
        static Dictionary<int, Dictionary<int, string>> _PeriodDic = new Dictionary<int, Dictionary<int, string>>();

        readonly Dictionary<int, string> _monthsDic = new Dictionary<int, string>() {
            {0,"ต.ค" },
            {1,"พ.ย" },
            {2,"ธ.ค" },
            {3,"ม.ค" },
            {4,"ก.พ" },
            {5,"มี.ค" },
            {6,"เม.ย" },
            {7,"พ.ค" },
            {8,"มิ.ย" },
            {9,"ก.ค" },
            {10,"ส.ค" },
            {11,"ก.ย" },
        };
        readonly Dictionary<int, string> _quartersDic = new Dictionary<int, string>() {
            {0,"ไตรมาส 1" },
            {1,"ไตรมาส 2" },
            {2,"ไตรมาส 3" },
            {3,"ไตรมาส 4" },
        };
        readonly Dictionary<int, string> _yearDic = new Dictionary<int, string>() {
            {0,"ราย 6 เดือน" },
            {1,"สิ้นปี" },
        };

        readonly List<SelectListItem> _predicOwnerDropdownList = new List<SelectListItem>()
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

            if (indicator.PredicOwner.Count == 0)
            {
                PredicOwner predicOwner = new PredicOwner();
                predicOwner.IndicatorID = indicator.ID;
                predicOwner.IsDelete = false;
                indicator.PredicOwner.Add(predicOwner);
            }

            if (indicator.ImportantIndicatorTargetMeasurement.Count > 0)
            {
                int indexPointer = 0;
                indicator = SetupTargetMeasurementFromDB(indicator);
                foreach (var i in indicator.IndicatorXIndicatorType)
                {
                    indicator.ImportantIndicatorTargetMeasurement.ToList()[indexPointer].IsDispaly = true;
                    if (i.IsCheck == false)
                    {
                        indicator.ImportantIndicatorTargetMeasurement.ToList()[indexPointer].IsUnCheck = true;
                    }
                    indexPointer++;
                }
            }
            else
            {
                List<ImportantIndicatorTargetMeasurement> temp = new List<ImportantIndicatorTargetMeasurement>();
                foreach (var i in indicator.IndicatorXIndicatorType)
                {
                    var imTargetTypeDisplay = InitImportantIndicatorTargetMeasurementRow();
                    imTargetTypeDisplay.IndicatorTypeID = i.IndicatorTypeID;
                    imTargetTypeDisplay.IsDispaly = true;
                    imTargetTypeDisplay.IndicatorID = indicator.ID;

                    if (i.IsCheck == false)
                    {
                        imTargetTypeDisplay.IsUnCheck = true;
                    }
                    temp.Add(imTargetTypeDisplay);

                }
                temp.AddRange(indicator.ImportantIndicatorTargetMeasurement.ToList());
                indicator.ImportantIndicatorTargetMeasurement = temp;
            }

            InitStatic(indicator);
            ViewbagData();

            return View(indicator);
        }

        public ActionResult AddPredictOwner(Indicator indicator)
        {
            PredicOwner predicOwner = new PredicOwner();
            predicOwner.IndicatorID = indicator.ID;
            predicOwner.IsDelete = false;
            indicator.PredicOwner.Add(predicOwner);

            ViewbagData();
            return View(_SceneView, indicator);
        }
        public ActionResult DeletePredictOwner(Indicator indicator)
        {
            ModelState.Clear();
            foreach (var i in indicator.PredicOwner)
            {
                if (i.IsDeletePredicOwner)
                {
                    if (i.ID == 0)
                    {
                        var tempList = indicator.PredicOwner.ToList();
                        tempList.Remove(i);
                        indicator.PredicOwner = tempList;
                    }
                    else
                    {
                        i.IsDelete = true;
                    }
                }
            }

            ViewbagData();
            return View(_SceneView, indicator);
        }

        public ActionResult AddTargetMeasurement(Indicator indicator)
        {
            ModelState.Clear();
            indicator.ImportantIndicatorTargetMeasurement.Add(InitImportantIndicatorTargetMeasurementRow());
            ViewbagData();
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
            ViewbagData();
            return View(_SceneView, indicator);
        }

        [HttpPost]
        public ActionResult Target(Indicator indicator)
        {
            foreach (var i in indicator.PredicOwner)
            {
                i.UpdateDate = DateTime.Now;
                if (i.ID == 0)
                {
                    i.CreateDate = DateTime.Now;
                    db.PredicOwner.Add(i);
                }
                else
                {
                    db.Entry(i).State = EntityState.Modified;
                }
            }

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
            ClearStatic();
            _SceneView = "Result";
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

            foreach (var i in db.PeriodMountOrQuarterOrYear)
            {
                ImportantIndicatorResultMeasurement imResult = new ImportantIndicatorResultMeasurement()
                {
                    PeriodMounthOrQuarterOrYearID = i.ID,
                    PMoYName = i.Period
                };
                indicator.ImportantIndicatorResultMeasurement.Add(imResult);
            }

            Dictionary<string, int> _PMoYRangeDic = new Dictionary<string, int>() {
            {"รายเดือน",12 },
            {"รายไตรมาส",4 },
            {"รายปี",2 }, };

            foreach (var i in indicator.ImportantIndicatorResultMeasurement)
            {
                if (_PMoYRangeDic.ContainsKey(i.PMoYName))
                {
                    int range = _PMoYRangeDic[i.PMoYName];
                    for (int j = 0; j < range; j++)
                    {
                        i.ForecastPeriod.Add(new ForecastPeriod());
                    }
                }
            }

            indicator.PeriodSelected_Index = 0;
            ViewbagPeriodDic(_monthsDic);

            InitStatic(indicator);
            ViewbagData();
            return View(indicator);
        }

        public ActionResult ChangePeriod(Indicator indicator)
        {

            ViewbagPeriodDic(indicator);
            ViewbagData();
            return View(_SceneView, indicator);
        }
        public ActionResult ChangeMonthQuarterHailfYear(Indicator indicator)
        {
            //Hell Index Search hehe
            indicator.ImportantIndicatorResultMeasurement.ToList()[indicator.PeriodSelected_Index]
                .ForecastPeriod.ToList()[indicator.ForeCastPeriodSelected_Index].IsSelect = true;

            ViewbagPeriodDic(indicator);
            ViewbagData();
            return View(_SceneView, indicator);
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
        public ActionResult Result(Indicator indicator)
        {
            return View();
        }
        public ActionResult Report(int? sOEPlanID)
        {
            return View();
        }

        private void ViewbagData()
        {
            ViewBag.IndicatorTypeDropdownList = _IndicatorTypeDropdownList;
            ViewBag.IndicatorUnitDropdownList = _IndicatorUnitDropdownList;
            ViewBag.PredicOwnerDropdownList = _predicOwnerDropdownList;

            Dictionary<string, int> PMoYID_dic = new Dictionary<string, int>();
            foreach (var i in db.PeriodMountOrQuarterOrYear.ToList())
            {
                PMoYID_dic.Add(i.Period, i.ID);
            }
            ViewBag.PMoYID_dic = PMoYID_dic;
        }

        private void ClearStatic()
        {
            _IndicatorID = null;
            _IndicatorTypeDropdownList.Clear();
            _IndicatorUnitDropdownList.Clear();
            _PeriodDic.Clear();
        }

        private ImportantIndicatorTargetMeasurement InitImportantIndicatorTargetMeasurementRow()
        {
            ImportantIndicatorTargetMeasurement imTargetRow = new ImportantIndicatorTargetMeasurement();

            List<ImportantIndicatorTargetMeasurement> targetList = new List<ImportantIndicatorTargetMeasurement>();
            int range = 5;

            for (int i = 0; i < range; i++)
            {
                ImportantIndicatorTargetMeasurement imTargetChild = new ImportantIndicatorTargetMeasurement();
                imTargetChild.IndicatorLevel = i + 1;
                targetList.Add(imTargetChild);
            }
            imTargetRow.TargetList = targetList;
            imTargetRow.IsDelete = false;
            return imTargetRow;
        }

        private Indicator SetupTargetMeasurementFromDB(Indicator indicator)
        {
            if (indicator.ImportantIndicatorTargetMeasurement.Count == 0) return indicator;

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

        private void InitStatic(Indicator indicator)
        {
            foreach (var i in indicator.IndicatorXIndicatorType)
            {
                if (i.IsCheck)
                {
                    SelectListItem selectListItem = new SelectListItem() { Text = i.IndicatorType.IndicatorType1, Value = i.IndicatorTypeID.ToString() };
                    _IndicatorTypeDropdownList.Add(selectListItem);
                }
            }

            foreach (var i in indicator.IndicatorUnit)
            {
                SelectListItem selectListItem = new SelectListItem() { Text = i.Unit, Value = i.ID.ToString() };
                _IndicatorUnitDropdownList.Add(selectListItem);
            }

            _PeriodDic.Add(0, _monthsDic);
            _PeriodDic.Add(1, _quartersDic);
            _PeriodDic.Add(2, _yearDic);
        }

        private void ViewbagPeriodDic(Indicator indicator)
        {
            ViewBag.PeriodDic = _PeriodDic[indicator.PeriodSelected_Index];
        }

        private void ViewbagPeriodDic(Dictionary<int, string> dic)
        {
            ViewBag.PeriodDic = dic;
        }

    }
}