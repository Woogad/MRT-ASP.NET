using MRT_Demo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MRT_Demo.Controllers
{
    public class GoalController : Controller
    {
        MRTEntities db = new MRTEntities();
        private static int? _StrategicObjectiveID;
        public ActionResult Manage(int? StrategicObjectiveID)
        {
            StrategicObjective strategicObjective = db.StrategicObjective.Find(StrategicObjectiveID);
            if (strategicObjective == null)
            {
                return HttpNotFound();
            }
            if (strategicObjective.Goal.Count == 0)
            {
                Goal goal = new Goal();
                goal.IsDelete = false;
                goal.IsLastDelete = false;
                strategicObjective.Goal.Add(goal);
            }
            _StrategicObjectiveID = StrategicObjectiveID;
            return View(strategicObjective);
        }

        public ActionResult AddGoal(StrategicObjective strategicObjective)
        {
            Goal goal = new Goal();
            goal.IsDelete = false;
            goal.IsLastDelete = false;
            strategicObjective.Goal.Add(goal);
            return View("Manage", strategicObjective);
        }
        public ActionResult DeleteGoal(StrategicObjective strategicObjective)
        {
            ModelState.Clear();
            foreach (var i in strategicObjective.Goal)
            {
                if (i.IsDeleteGoal)
                {
                    if (i.ID == 0)
                    {
                        var tempList = strategicObjective.Goal.ToList();
                        tempList.RemoveAt(tempList.IndexOf(i));
                        strategicObjective.Goal = tempList;
                    }
                    else
                    {
                        i.IsDelete = true;
                    }
                }
            }
            return View("Manage", strategicObjective);
        }

        [HttpPost]
        public ActionResult Manage(StrategicObjective strategicObjective)
        {
            foreach (var i in strategicObjective.Goal)
            {
                i.UpdateDate = DateTime.Now;
                if (i.ID == 0)
                {
                    i.CreateDate = DateTime.Now;
                    i.StrategicObjectiveID = _StrategicObjectiveID;
                    i.NO = (int)db.Goal.LongCount() +1;
                    db.Goal.Add(i);
                    db.SaveChanges();
                }
                else
                {
                    db.Entry(i).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index", "StrategicObjective",new {sOEPlanID = strategicObjective.SOEPlanID});
        }

    }
}