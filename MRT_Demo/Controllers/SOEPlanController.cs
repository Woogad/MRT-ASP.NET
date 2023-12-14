using MRT_Demo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace MRT_Demo.Controllers
{
    public class SOEPlanController : Controller
    {
        MRTEntities db = new MRTEntities();
        List<SelectListItem> StartyearItemList = new List<SelectListItem>();
        List<SelectListItem> EndyearItemList = new List<SelectListItem>();
        int yearMax = DateTime.Now.Year + 11;
        int yearMin = DateTime.Now.Year - 10;

        public ActionResult Index()
        {
            List<SOEPlan> sOEPlanList = db.SOEPlan.Where(s => s.IsDelete == false).ToList();
            return View(sOEPlanList);
        }

        public ActionResult Create()
        {
            for (int i = yearMin; i < yearMax; i++)
            {
                SelectListItem item = new SelectListItem() { Text = i.ToString(), Value = i.ToString() };

                if (i == DateTime.Now.Year)
                {
                    item.Selected = true;
                    StartyearItemList.Add(item);
                    continue;
                }
                if (i == DateTime.Now.Year + 1)
                {
                    item.Selected = true;
                    EndyearItemList.Add(item);
                    continue;
                }
                StartyearItemList.Add(item);
                EndyearItemList.Add(item);
            }
            ViewBag.StartyearList = StartyearItemList;
            ViewBag.EndyearList = EndyearItemList;
            return View();
        }

        [HttpPost]
        public ActionResult Create(SOEPlan sOEPlan)
        {
            sOEPlan.CreateDate = DateTime.Now;
            sOEPlan.UpdateDate = DateTime.Now;
            sOEPlan.IsDelete = false;
            sOEPlan.IsLastDelete = false;
            db.SOEPlan.Add(sOEPlan);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SOEPlan sOEPlan = db.SOEPlan.Find(id);
            if (sOEPlan == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }

            for (int i = yearMin; i < yearMax; i++)
            {
                SelectListItem item = new SelectListItem() { Text = i.ToString(), Value = i.ToString() };

                if (i == DateTime.Now.Year)
                {
                    item.Selected = true;
                    StartyearItemList.Add(item);
                    continue;
                }
                if (i == DateTime.Now.Year + 1)
                {
                    item.Selected = true;
                    EndyearItemList.Add(item);
                    continue;
                }
                StartyearItemList.Add(item);
                EndyearItemList.Add(item);
            }
            ViewBag.StartyearList = StartyearItemList;
            ViewBag.EndyearList = EndyearItemList;

            return View(sOEPlan);
        }
        [HttpPost]
        public ActionResult Edit(SOEPlan sOEPlan)
        {
            sOEPlan.UpdateDate = DateTime.Now;
            db.Entry(sOEPlan).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }
            SOEPlan sOEPlan = db.SOEPlan.Find(id);
            if (sOEPlan == null)
            {
                return HttpNotFound();
            }

            db.Entry(sOEPlan).State = EntityState.Modified;
            sOEPlan.IsDelete = true;
            sOEPlan.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult RecycleBin()
        {
            return View(db.SOEPlan.Where(s => s.IsDelete == true && s.IsLastDelete == false).ToList());
        }

        public ActionResult Recover(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SOEPlan sOEPlan = db.SOEPlan.Find(id);
            if (sOEPlan == null)
            {
                return HttpNotFound();
            }

            db.Entry(sOEPlan).State = EntityState.Modified;
            sOEPlan.IsDelete = false;
            sOEPlan.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("RecycleBin");
        }

        public ActionResult LastDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SOEPlan sOEPlan = db.SOEPlan.Find(id);
            if (sOEPlan == null)
            {
                return HttpNotFound();
            }

            db.Entry(sOEPlan).State = EntityState.Modified;
            FlagIsDeleteAllChild(sOEPlan);
            sOEPlan.IsLastDelete = true;
            sOEPlan.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("RecycleBin");
        }

        public void FlagIsDeleteAllChild(SOEPlan sOEPlan)
        {
            if (sOEPlan.StrategicObjective.Count > 0)
            {
                var strategicObjectiveController = new StrategicObjectiveController();
                foreach (var i in sOEPlan.StrategicObjective)
                {
                    strategicObjectiveController.FlagIsDeleteAllChild(i);
                    i.IsDelete = true;
                    i.IsLastDelete = true;
                }
            }
        }

    }
}