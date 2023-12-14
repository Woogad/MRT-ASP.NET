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
    public class StrategyController : Controller
    {
        MRTEntities db = new MRTEntities();
        private static int? _StrategicObjectiveID;
        private static string _ViewName = "";
        public ActionResult Index(int? StrategicObjectiveID)
        {
            StrategicObjective strategicObjective = db.StrategicObjective.Find(StrategicObjectiveID);
            if (strategicObjective == null)
            {
                return HttpNotFound();
            }
            List<Strategy> strategyList = strategicObjective.Strategy.Where(s => s.IsDelete == false).ToList();
            _StrategicObjectiveID = StrategicObjectiveID;
            return View(strategyList);
        }

        public ActionResult Create()
        {
            _ViewName = "Create";
            return View();
        }

        public ActionResult AddTactic(Strategy strategy)
        {
            Tactic tactic = new Tactic();
            //tactic.StrategyID = strategy.ID;
            tactic.IsDelete = false;
            tactic.IsLastDelete = false;
            strategy.Tactic.Add(tactic);
            return View(_ViewName, strategy);

        }
        public ActionResult DeleteTactic(Strategy strategy)
        {
            foreach (var i in strategy.Tactic)
            {
                if (i.IsDeleteTactic)
                {
                    if (i.ID == 0)
                    {
                        List<Tactic> TempList = strategy.Tactic.ToList();
                        TempList.RemoveAt(TempList.IndexOf(i));
                        strategy.Tactic = TempList;
                    }
                    else
                    {
                        i.IsDelete = true;
                    }
                }
            }
            return View(_ViewName, strategy);
        }

        [HttpPost]
        public ActionResult Create(Strategy strategy)
        {
            strategy.CreateDate = DateTime.Now;
            strategy.UpdateDate = DateTime.Now;
            strategy.IsDelete = false;
            strategy.IsLastDelete = false;
            strategy.StrategicObjectiveID = _StrategicObjectiveID;
            strategy.NO = (int)db.Strategy.LongCount() + 1;
            if (strategy.Tactic.Count > 0)
            {
                foreach (var i in strategy.Tactic)
                {
                    i.CreateDate = DateTime.Now;
                    i.UpdateDate = DateTime.Now;
                    i.NO = (int)db.Tactic.LongCount() + 1;
                    db.Tactic.Add(i);
                    db.SaveChanges();
                }
            }
            db.Strategy.Add(strategy);
            db.SaveChanges();
            return RedirectToAction("Index", new { StrategicObjectiveID = _StrategicObjectiveID });
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Strategy strategy = db.Strategy.Find(id);
            if (strategy == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }
            _ViewName = "Edit";
            return View(strategy);
        }

        [HttpPost]
        public ActionResult Edit(Strategy strategy)
        {
            foreach (var i in strategy.Tactic)
            {
                if (i.ID == 0)
                {
                    i.UpdateDate = DateTime.Now;
                    i.CreateDate = DateTime.Now;
                    i.StrategyID = strategy.ID;
                    db.Tactic.Add(i);
                }
                else
                {
                    db.Entry(i).State = EntityState.Modified;
                }
            }
            db.Entry(strategy).State = EntityState.Modified;
            strategy.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Index", new { StrategicObjectiveID = _StrategicObjectiveID });
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Strategy strategy = db.Strategy.Find(id);
            if (strategy == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            db.Entry(strategy).State = EntityState.Modified;
            strategy.IsDelete = true;
            strategy.UpdateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Index" , new {StrategicObjectiveID = _StrategicObjectiveID});
        }

        public ActionResult RecycleBin()
        {
            if (_StrategicObjectiveID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StrategicObjective strategicObjective = db.StrategicObjective.Find(_StrategicObjectiveID);
            if (strategicObjective == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }
            return View(strategicObjective.Strategy.Where(s => s.IsDelete == true && s.IsLastDelete == false).ToList());
        }

        public ActionResult Recover(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Strategy strategy = db.Strategy.Find(id);
            if (strategy == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }
            db.Entry(strategy).State = EntityState.Modified;
            strategy.UpdateDate = DateTime.Now;
            strategy.IsDelete = false;
            db.SaveChanges();

            return RedirectToAction("RecycleBin");
        }

        public ActionResult LastDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Strategy strategy = db.Strategy.Find(id);
            if (strategy == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }
            db.Entry(strategy).State = EntityState.Modified;
            FlagIsDeleteAllChild(strategy);
            strategy.UpdateDate = DateTime.Now;
            strategy.IsLastDelete = true;
            db.SaveChanges();
            return RedirectToAction("RecycleBin");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public void FlagIsDeleteAllChild(Strategy strategy)
        {
            foreach (var i in strategy.Tactic)
            {
                i.UpdateDate = DateTime.Now;
                i.IsDelete = true;
                i.IsLastDelete = true;
            }
        }

    }
}