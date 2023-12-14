using MRT_Demo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MRT_Demo.Controllers
{
    public class StrategicObjectiveController : Controller
    {
        private MRTEntities db = new MRTEntities();

        private static int? _SOEPlanID = -1;
        public ActionResult Index(int? sOEPlanID, string searchName)
        {
            if (sOEPlanID != null)
            {
                if (_SOEPlanID != sOEPlanID)
                {
                    _SOEPlanID = sOEPlanID;
                }
            }
            else
            {
                return HttpNotFound();
            }
            SOEPlan sOEPlan = db.SOEPlan.Find(_SOEPlanID);
            sOEPlan.StrategicObjective = sOEPlan.StrategicObjective.Where(s => s.IsDelete == false).ToList();
            ViewBag.SOEPlanID = _SOEPlanID;
            if (searchName != null)
            {
                List<StrategicObjective> strategicObjectivesSearchList = sOEPlan.StrategicObjective.
                    Where(s => s.StrategicObjective1 == searchName).ToList();
                if (strategicObjectivesSearchList.Count != 0)
                {
                    ViewBag.SearchResult = "Search Found! :)";
                    return View(strategicObjectivesSearchList);
                }
                else if(searchName == "")
                {
                    ViewBag.SearchResult = "";
                }
                else { ViewBag.SearchResult = "Search Not Found! :("; }
            }
            else { ViewBag.SearchResult = ""; }
            return View(sOEPlan.StrategicObjective.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(StrategicObjective strategicObjective)
        {
            if (_SOEPlanID == -1)
            {
                return HttpNotFound();
            }
            SOEPlan sOEPlan = db.SOEPlan.Find(_SOEPlanID);
            if (sOEPlan == null)
            {
                return HttpNotFound();
            }
            strategicObjective.SOEPlanID = _SOEPlanID;
            strategicObjective.CreateDate = DateTime.Now;
            strategicObjective.UpdateDate = DateTime.Now;
            strategicObjective.IsDelete = false;
            strategicObjective.IsLastDelete = false;
            strategicObjective.NO = (int)db.StrategicObjective.LongCount() + 1;
            db.StrategicObjective.Add(strategicObjective);
            db.SaveChanges();
            return RedirectToAction("Index", new { sOEPlanID = _SOEPlanID });
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StrategicObjective strategicObjective = db.StrategicObjective.Find(id);
            if (strategicObjective == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }
            return View(strategicObjective);
        }
        [HttpPost]
        public ActionResult Edit(StrategicObjective strategicObjective)
        {
            strategicObjective.UpdateDate = DateTime.Now;
            db.Entry(strategicObjective).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", new { sOEPlanID = _SOEPlanID });
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StrategicObjective strategicObjective = db.StrategicObjective.Find(id);
            if (strategicObjective == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }
            db.Entry(strategicObjective).State = EntityState.Modified;
            strategicObjective.IsDelete = true;
            db.SaveChanges();
            return RedirectToAction("Index", new { sOEPlanID = _SOEPlanID });
        }

        public ActionResult RecycleBin()
        {
            if (_SOEPlanID == -1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SOEPlan sOEPlan = db.SOEPlan.Find(_SOEPlanID);
            if (sOEPlan == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }
            List<StrategicObjective> strategicObjectiveList = sOEPlan.StrategicObjective.
                                        Where(s => s.IsDelete == true && s.IsLastDelete == false).ToList();

            return View(strategicObjectiveList);
        }
        public ActionResult Recover(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StrategicObjective strategicObjective = db.StrategicObjective.Find(id);
            if (strategicObjective == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }
            db.Entry(strategicObjective).State = EntityState.Modified;
            strategicObjective.IsDelete = false;
            strategicObjective.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("RecycleBin");
        }

        public ActionResult LastDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StrategicObjective strategicObjective = db.StrategicObjective.Find(id);
            if (strategicObjective == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }

            db.Entry(strategicObjective).State = EntityState.Modified;
            FlagIsDeleteAllChild(strategicObjective);
            strategicObjective.IsLastDelete = true;
            strategicObjective.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("RecycleBin");
        }

        public void FlagIsDeleteAllChild(StrategicObjective strategicObjective)
        {
            foreach (var i in strategicObjective.Strategy)
            {
                if (i.Tactic.Count > 0)
                {
                    StrategyController strategyController = new StrategyController();
                    strategyController.FlagIsDeleteAllChild(i);
                }
                i.UpdateDate = DateTime.Now;
                i.IsDelete = true;
                i.IsLastDelete = true;
            }
            foreach (var i in strategicObjective.Goal)
            {
                i.UpdateDate = DateTime.Now;
                i.IsDelete = true;
                i.IsLastDelete = true;
            }
        }

    }
}