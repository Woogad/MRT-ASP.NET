using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MRT_Demo.Models;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Diagnostics;
using MRT_Demo.Services;

namespace MRT_Demo.Controllers
{
    public class IndicatorController : Controller
    {
        private static List<SelectListItem> _StatusDropdownList = new List<SelectListItem>();
        private static List<SelectListItem> _DivisionDropdownList = new List<SelectListItem>();
        private static string _ViewScene = "";

        MRTEntities db = new MRTEntities();
        List<string> divisionList = new List<string>() { "ฝพธ.", "ฝพค.", "ฝนย." };
        List<SelectListItem> activeDropdownList = new List<SelectListItem>()
            {
                new SelectListItem {Text = "ทั้งหมด",Value = ""},
                new SelectListItem {Text = "ใช้งาน",Value = true.ToString()},
                new SelectListItem {Text = "ไม่ใช้งาน",Value = false.ToString()},
            };

        public ActionResult Index(string divisionSearch, string indicatorSearch, string activeState, int? indicatorDetailStatusID)
        {
            SetupDropdownStatic();
            List<Indicator> indicatorList = db.Indicator.Where(s => s.IsDelete == false).ToList();

            foreach (var i in indicatorList)
            {
                i.IndicatorOwner = i.IndicatorOwner.Where(s => s.IsDelete == false).ToList();
            }

            indicatorList = IndicatorService.IndicatorsSearch(indicatorList, divisionSearch, indicatorSearch, activeState);
            ViewbagDropdown();

            return View(indicatorList);
        }

        public ActionResult Create()
        {
            _ViewScene = "Create";
            Indicator indicator = new Indicator();

            IndicatorOwner indicatorOwner = new IndicatorOwner();
            indicatorOwner.IsDelete = false;
            indicator.IndicatorOwner.Add(indicatorOwner);

            IndicatorUnit indicatorUnit = new IndicatorUnit();
            indicatorUnit.IsDelete = false;
            indicator.IndicatorUnit.Add(indicatorUnit);


            List<IndicatorType> indicatorType = db.IndicatorType.ToList();
            foreach (var i in indicatorType)
            {
                IndicatorXIndicatorType indicatorXIndicator = new IndicatorXIndicatorType();
                indicatorXIndicator.IndicatorTypeID = i.ID;
                indicatorXIndicator.IndicatorType = i;
                indicator.IndicatorXIndicatorType.Add(indicatorXIndicator);
            }

            ViewbagDropdown();

            return View(indicator);
        }

        [HttpPost]
        public ActionResult Create(Indicator indicator)
        {
            foreach (var i in indicator.IndicatorOwner)
            {
                i.CreateDate = DateTime.Now;
                i.UpdateDate = DateTime.Now;
                db.IndicatorOwner.Add(i);
            }
            foreach (var i in indicator.IndicatorUnit)
            {
                i.CreateDate = DateTime.Now;
                i.UpdateDate = DateTime.Now;
                db.IndicatorUnit.Add(i);
            }
            foreach (var i in indicator.IndicatorXIndicatorType)
            {
                i.CreateDate = DateTime.Now;
                i.UpdateDate = DateTime.Now;
                i.IsDelete = false;
                i.IsLastDelete = false;
                i.IndicatorType = null;
                db.IndicatorXIndicatorType.Add(i);
            }
            indicator.CreateDate = DateTime.Now;
            indicator.UpdateDate = DateTime.Now;
            indicator.IsDelete = false;
            indicator.IsLastDelete = false;
            db.Indicator.Add(indicator);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddOwner(Indicator indicator, string viewName)
        {
            IndicatorOwner indicatorOwner = new IndicatorOwner();
            if (indicator.ID != 0)
            {
                indicatorOwner.IndicatorID = indicator.ID;
            }
            indicatorOwner.IsDelete = false;
            indicator.IndicatorOwner.Add(indicatorOwner);
            ViewbagDropdown();

            return View(_ViewScene, indicator);
        }

        public ActionResult DeleteOwner(Indicator indicator)
        {
            ModelState.Clear();
            foreach (var i in indicator.IndicatorOwner)
            {
                if (i.IsDeleteOwner)
                {
                    if (i.ID == 0)
                    {
                        List<IndicatorOwner> temp = indicator.IndicatorOwner.ToList();
                        temp.Remove(i);
                        indicator.IndicatorOwner = temp;
                    }
                    else
                    {
                        i.IsDelete = true;
                    }
                }
            }
            ViewbagDropdown();
            return View(_ViewScene, indicator);
        }

        public ActionResult AddUnit(Indicator indicator, string viewName)
        {
            IndicatorUnit indicatorUnit = new IndicatorUnit();
            if (indicator.ID != 0)
            {
                indicatorUnit.IndicatorID = indicator.ID;
            }
            indicatorUnit.IsDelete = false;
            indicator.IndicatorUnit.Add(indicatorUnit);
            ViewbagDropdown();
            return View(_ViewScene, indicator);
        }
        public ActionResult DeleteUnit(Indicator indicator)
        {
            ModelState.Clear();
            foreach (var i in indicator.IndicatorUnit)
            {
                if (i.IsDeleteUnit)
                {
                    if (i.ID == 0)
                    {
                        List<IndicatorUnit> temp = indicator.IndicatorUnit.ToList();
                        temp.Remove(i);
                        indicator.IndicatorUnit = temp;
                    }
                    else
                    {
                        i.IsDelete = true;
                    }
                }
            }
            ViewbagDropdown();
            return View(_ViewScene, indicator);
        }

        public ActionResult Edit(int? id)
        {
            _ViewScene = "Edit";
            if (id == null)
            {
                return HttpNotFound();
            }
            var indicator = db.Indicator.Find(id);
            if (indicator == null)
            {
                return HttpNotFound();
            }
            ViewbagDropdown();
            return View(indicator);
        }

        [HttpPost]
        public ActionResult Edit(Indicator indicator)
        {
            foreach (var i in indicator.IndicatorOwner)
            {
                if (i.ID == 0)
                {
                    db.IndicatorOwner.Add(i);
                    db.SaveChanges();
                }
            }
            foreach (var i in indicator.IndicatorUnit)
            {
                if (i.ID == 0)
                {
                    db.IndicatorUnit.Add(i);
                    db.SaveChanges();
                }
            }
            db.Entry(indicator).State = EntityState.Modified;
            indicator.UpdateDate = DateTime.Now;
            foreach (var i in indicator.IndicatorUnit)
            {
                i.UpdateDate = DateTime.Now;
                if (i.ID == 0)
                {
                    i.CreateDate = DateTime.Now;
                    db.IndicatorUnit.Add(i);
                }
                else
                {
                    db.Entry(i).State = EntityState.Modified;
                }
            }
            foreach (var i in indicator.IndicatorOwner)
            {
                i.UpdateDate = DateTime.Now;
                if (i.ID == 0)
                {
                    i.CreateDate = DateTime.Now;
                    db.IndicatorOwner.Add(i);
                }
                else
                {
                    db.Entry(i).State = EntityState.Modified;
                }
            }
            foreach (var i in indicator.IndicatorXIndicatorType)
            {
                i.UpdateDate = DateTime.Now;
                db.Entry(i).State = EntityState.Modified;

            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var indicator = db.Indicator.Find(id);
            if (indicator == null)
            {
                return HttpNotFound();
            }
            db.Entry(indicator).State = EntityState.Modified;
            indicator.IsDelete = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult RecycleBin(string divisionSearch, string indicatorSearch, int? indicatorID)
        {
            ViewBag.divisionDropdownList = _DivisionDropdownList;
            List<Indicator> indicatorList = db.Indicator.Where(s => s.IsDelete == true && s.IsLastDelete == false).ToList();
            indicatorList = IndicatorService.IndicatorsSearch(indicatorList, divisionSearch, indicatorSearch);
            return View(indicatorList);
        }

        public ActionResult Recover(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var indicator = db.Indicator.Find(id);
            if (indicator == null)
            {
                return HttpNotFound();
            }
            db.Entry(indicator).State = EntityState.Modified;
            indicator.IsDelete = false;
            db.SaveChanges();
            return RedirectToAction("RecycleBin");
        }

        public ActionResult LastDelete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var indicator = db.Indicator.Find(id);
            if (indicator == null)
            {
                return HttpNotFound();
            }
            db.Entry(indicator).State = EntityState.Modified;
            indicator.IsLastDelete = true;
            foreach (var i in indicator.IndicatorOwner)
            {
                i.IsDelete = true;
            }
            foreach (var i in indicator.IndicatorUnit)
            {
                i.IsDelete = true;
            }
            foreach (var i in indicator.IndicatorXIndicatorType)
            {
                i.IsDelete = true;
                i.IsLastDelete = true;
            }
            db.SaveChanges();
            return RedirectToAction("RecycleBin");
        }

        private void ViewbagDropdown()
        {
            ViewBag.ActiveDropdownList = activeDropdownList;
            ViewBag.DivisionDropdownList = _DivisionDropdownList;
            ViewBag.StatusDropdownList = _StatusDropdownList;
        }
        private void SetupDropdownStatic()
        {
            List<IndicatorDetailStatus> statusList = db.IndicatorDetailStatus.ToList();

            if (_StatusDropdownList.Count != statusList.Count)
            {
                _StatusDropdownList.Clear();
                foreach (var i in statusList)
                {
                    SelectListItem selectListItem = new SelectListItem() { Text = i.Status, Value = i.ID.ToString() };
                    _StatusDropdownList.Add(selectListItem);
                }
            }
            if (_DivisionDropdownList.Count != divisionList.Count)
            {
                foreach (var i in divisionList)
                {
                    SelectListItem item = new SelectListItem() { Text = i, Value = i };
                    _DivisionDropdownList.Add(item);
                }
            }
        }

    }
}