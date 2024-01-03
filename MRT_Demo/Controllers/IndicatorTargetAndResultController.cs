using MRT_Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;
using System.Web.UI.WebControls;


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
            ViewbagData(indicator);

            return View(indicator);
        }

        public ActionResult AddPredictOwner(Indicator indicator)
        {
            PredicOwner predicOwner = new PredicOwner();
            predicOwner.IndicatorID = indicator.ID;
            predicOwner.IsDelete = false;
            indicator.PredicOwner.Add(predicOwner);

            ViewbagData(indicator);
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

            ViewbagData(indicator);
            return View(_SceneView, indicator);
        }

        public ActionResult AddTargetMeasurement(Indicator indicator)
        {
            ModelState.Clear();
            indicator.ImportantIndicatorTargetMeasurement.Add(InitImportantIndicatorTargetMeasurementRow());
            ViewbagData(indicator);
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
            ViewbagData(indicator);
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
            InitStatic(indicator);

            if (indicator == null)
            {
                return HttpNotFound();
            }
            indicator = SetupTargetMeasurementFromDB(indicator);

            if (indicator.ImportantIndicatorResultMeasurement.Count == 0)
            {
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
                            i.ForecastPeriod.Add(new ForecastPeriod() { IsDelete = false});
                        }
                    }
                }
                foreach (var i in indicator.ImportantIndicatorResultMeasurement)
                {
                    foreach (var j in i.ForecastPeriod)
                    {
                        j.ForecastPeriodToolAndMethod = InitForecastPeriodMethodList();
                        j.ForecastValueAndRealValue = InitForecastValueAndRealValueList();
                        j.ForecastPeriodCompetitorValue.Add(new ForecastPeriodCompetitorValue() { IsDelete = false });
                        j.ForecastPeriodResultRemark.Add(new ForecastPeriodResultRemark() { IsDelete = false, IsLastDelete = false });
                    }
                }
            }
            else
            {
                var PMoQoYList = db.PeriodMountOrQuarterOrYear.ToList();
                var FPToolList = db.ForecastTool.ToList();
                if (PMoQoYList == null || FPToolList == null)
                {
                    return HttpNotFound();
                }

                for (int i = 0; i < indicator.ImportantIndicatorResultMeasurement.Count; i++)
                {
                    indicator.ImportantIndicatorResultMeasurement.ToList()[i].PMoYName = PMoQoYList[i].Period;
                    foreach (var j in indicator.ImportantIndicatorResultMeasurement.ToList()[i].ForecastPeriod)
                    {
                        for (int h = 0; h < j.ForecastPeriodToolAndMethod.Count; h++)
                        {
                            if (h > FPToolList.Count - 1)
                            {
                                j.ForecastPeriodToolAndMethod.ToList()[h].FPToolName = "อื่นๆ โปรดระบุ";
                                j.ForecastPeriodToolAndMethod.ToList()[h].IsOtherTool = true;
                            }
                            else
                            {
                                j.ForecastPeriodToolAndMethod.ToList()[h].FPToolName = FPToolList[h].ForecastTool1;
                            }
                        }
                    }
                }

            }

            indicator.PeriodSelected_Index = 0;
            indicator.ForeCastPeriodSelected_Index = 0;
            SetForeCastPeriodSelectedIndexTrue(indicator);

            ViewbagData(indicator);
            return View(indicator);
        }

        public ActionResult ChangePeriod(Indicator indicator)
        {
            indicator.ForeCastPeriodSelected_Index = 0;
            SetForeCastPeriodSelectedIndexTrue(indicator);

            ViewbagData(indicator);
            return View(_SceneView, indicator);
        }
        public ActionResult ChangeMonthQuarterHailfYear(Indicator indicator)
        {
            SetForeCastPeriodSelectedIndexTrue(indicator);

            ViewbagData(indicator);
            return View(_SceneView, indicator);
        }
        public ActionResult AddCompetitorValue(Indicator indicator)
        {
            SetForeCastPeriodSelectedIndexTrue(indicator);

            indicator.ImportantIndicatorResultMeasurement.ToList()[indicator.PeriodSelected_Index]
                .ForecastPeriod.ToList()[indicator.ForeCastPeriodSelected_Index].ForecastPeriodCompetitorValue.
                Add(new ForecastPeriodCompetitorValue()
                {
                    ForecastPeriodID = indicator.ImportantIndicatorResultMeasurement.ToList()[indicator.PeriodSelected_Index].ForecastPeriod.ToList()[indicator.ForeCastPeriodSelected_Index].ID,
                    IsDelete = false,
                });

            ViewbagData(indicator);
            return View(_SceneView, indicator);
        }
        public ActionResult DeleteCompetitorValue(Indicator indicator)
        {
            SetForeCastPeriodSelectedIndexTrue(indicator);
            foreach (var i in GetForeCastPeriodByIsSelect(indicator).ForecastPeriodCompetitorValue)
            {
                if (i.isDeleteFPCValue)
                {
                    if (i.ID == 0)
                    {
                        var tempFPCValueList = GetForeCastPeriodByIsSelect(indicator).ForecastPeriodCompetitorValue.ToList();
                        tempFPCValueList.Remove(i);

                        indicator.ImportantIndicatorResultMeasurement.ToList()[indicator.PeriodSelected_Index]
                        .ForecastPeriod.ToList()[indicator.ForeCastPeriodSelected_Index]
                        .ForecastPeriodCompetitorValue = tempFPCValueList;
                    }
                    else
                    {
                        i.IsDelete = true;
                    }
                    break;
                }
            }
            ViewbagData(indicator);
            return View(_SceneView, indicator);
        }
        public ActionResult ChangeFile(Indicator indicator)
        {

            ViewbagData(indicator);
            return View(_SceneView, indicator);
        }

        [HttpPost]
        public ActionResult Result(Indicator indicator)
        {
            ForecastPeriodResultRemark FPResultRemark = GetForeCastPeriodByIsSelect(indicator).ForecastPeriodResultRemark.First();

            if (FPResultRemark.FilePeriodDoc.First() != null)
            {
                foreach (var i in FPResultRemark.FilePeriodDoc)
                {
                    HttpPostedFileBase file = i;
                    if (file.ContentLength > 0)
                    {
                        ForecastPeriodDocFile forecastPeriodDocFile = new ForecastPeriodDocFile()
                        {
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            IsDelete = false,
                            IsLastDelete = false,
                        };

                        MRTFile mRTFile = InitMRTFile(file, out string serverPath);

                        file.SaveAs(serverPath);
                        db.MRTFile.Add(mRTFile);
                        db.SaveChanges();
                        forecastPeriodDocFile.FilePathID = mRTFile.ID;
                        forecastPeriodDocFile.ForecastPeriodResultRemarkID = FPResultRemark.ID;
                        FPResultRemark.ForecastPeriodDocFile.Add(forecastPeriodDocFile);
                    }
                }
                foreach(var i in FPResultRemark.ForecastPeriodDocFile)
                {
                    db.ForecastPeriodDocFile.Add(i);
                }
            }

            if (FPResultRemark.FileAnalysisResults.First() != null)
            {
                foreach (var i in FPResultRemark.FileAnalysisResults)
                {
                    HttpPostedFileBase file = i;
                    if (file.ContentLength > 0)
                    {
                        ForecastAnalysisResultFile forecastAnalysisResultFile = new ForecastAnalysisResultFile()
                        {
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            IsDelete = false,
                            IsLastDelete = false,
                        };
                        FPResultRemark.ForecastAnalysisResultFile.Add(forecastAnalysisResultFile);

                        MRTFile mRTFile = InitMRTFile(file, out string serverPath);

                        file.SaveAs(serverPath);
                        db.MRTFile.Add(mRTFile);
                        db.SaveChanges();
                        forecastAnalysisResultFile.FilePathID = mRTFile.ID;
                        forecastAnalysisResultFile.ForecastPeriodResultRemarkID = FPResultRemark.ID;
                        db.ForecastAnalysisResultFile.Add(forecastAnalysisResultFile);
                    }
                }
                foreach(var i in FPResultRemark.ForecastAnalysisResultFile)
                {
                    db.ForecastAnalysisResultFile.Add(i);
                }
            }

            if (FPResultRemark.FileChangeActionPlan.First() != null)
            {
                foreach (var i in FPResultRemark.FileChangeActionPlan)
                {
                    HttpPostedFileBase file = i;
                    if (file.ContentLength > 0)
                    {
                        ForecastChangeActionPlanFile forecastChangeActionPlanFile = new ForecastChangeActionPlanFile()
                        {
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            IsDelete = false,
                            IsLastDelete = false,
                        };
                        FPResultRemark.ForecastChangeActionPlanFile.Add(forecastChangeActionPlanFile);

                        MRTFile mRTFile = InitMRTFile(file, out string serverPath);

                        file.SaveAs(serverPath);
                        db.MRTFile.Add(mRTFile);
                        db.SaveChanges();
                        forecastChangeActionPlanFile.FilePathID = mRTFile.ID;
                        forecastChangeActionPlanFile.ForecastPeriodResultRemarkID = FPResultRemark.ID;
                        db.ForecastChangeActionPlanFile.Add(forecastChangeActionPlanFile);
                    }
                }
                foreach(var i in FPResultRemark.ForecastChangeActionPlanFile)
                {
                    db.ForecastChangeActionPlanFile.Add(i);
                }
            }

            if(FPResultRemark.ID != 0)
            {
                db.Entry(FPResultRemark).State = EntityState.Modified;
            }

            indicator.ImportantIndicatorResultMeasurement.ToList()[indicator.PeriodSelected_Index]
                .ForecastPeriod.ToList()[indicator.ForeCastPeriodSelected_Index]
                .ForecastPeriodResultRemark.ToList()[0] = FPResultRemark;

            foreach (var i in indicator.ImportantIndicatorResultMeasurement)
            {
                SaveImResult(indicator);
                foreach (var j in i.ForecastPeriod)
                {
                    SaveForecastPeriod(i);
                    foreach (var fVaRValue in j.ForecastValueAndRealValue)
                    {
                        fVaRValue.UpdateDate = DateTime.Now;
                        if (fVaRValue.ID == 0)
                        {
                            fVaRValue.CreateDate = DateTime.Now;
                            db.ForecastValueAndRealValue.Add(fVaRValue);
                        }
                        else
                        {
                            db.Entry(fVaRValue).State = EntityState.Modified;
                        }
                    }
                    foreach (var comValue in j.ForecastPeriodCompetitorValue)
                    {
                        comValue.UpdateDate = DateTime.Now;
                        if (comValue.ID == 0)
                        {
                            comValue.CreateDate = DateTime.Now;
                            db.ForecastPeriodCompetitorValue.Add(comValue);
                        }
                        else
                        {
                            db.Entry(comValue).State = EntityState.Modified;
                        }
                    }

                    foreach (var fPtool in j.ForecastPeriodToolAndMethod)
                    {
                        fPtool.UpdateDate = DateTime.Now;
                        if (fPtool.ID == 0)
                        {
                            fPtool.CreateDate = DateTime.Now;
                            db.ForecastPeriodToolAndMethod.Add(fPtool);
                        }
                        else
                        {
                            db.Entry(fPtool).State = EntityState.Modified;
                        }
                    }
                }
            }

            //TODO unSave
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Report(int? sOEPlanID)
        {
            return View();
        }

        private void ViewbagData(Indicator indicator)
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
            ViewBag.PeriodDic = _PeriodDic[indicator.PeriodSelected_Index];
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

        private List<ForecastPeriodToolAndMethod> InitForecastPeriodMethodList()
        {
            List<ForecastPeriodToolAndMethod> forecastPeriodToolAndMethodList = new List<ForecastPeriodToolAndMethod>();
            foreach (var i in db.ForecastTool)
            {
                //!tool
                forecastPeriodToolAndMethodList.Add(new ForecastPeriodToolAndMethod() { FPToolName = i.ForecastTool1, ForecastToolID = i.ID });
            }
            //!other tool
            forecastPeriodToolAndMethodList.Add(new ForecastPeriodToolAndMethod() { FPToolName = "อื่นๆ โปรดระบุ", IsOtherTool = true });
            return forecastPeriodToolAndMethodList;
        }

        private List<ForecastValueAndRealValue> InitForecastValueAndRealValueList()
        {
            List<ForecastValueAndRealValue> forecastValueAndRealValueList = new List<ForecastValueAndRealValue>();
            for (int i = 0; i < _IndicatorUnitDropdownList.Count; i++)
            {
                forecastValueAndRealValueList.Add(new ForecastValueAndRealValue() { UnitIndex = i });
            }

            return forecastValueAndRealValueList;
        }

        private void SetForeCastPeriodSelectedIndexTrue(Indicator indicator)
        {
            indicator.ImportantIndicatorResultMeasurement.ToList()[indicator.PeriodSelected_Index]
                .ForecastPeriod.ToList()[indicator.ForeCastPeriodSelected_Index].IsSelect = true;
        }

        private ForecastPeriod GetForeCastPeriodByIsSelect(Indicator indicator)
        {
            SetForeCastPeriodSelectedIndexTrue(indicator);

            return indicator.ImportantIndicatorResultMeasurement.ToList()[indicator.PeriodSelected_Index]
            .ForecastPeriod.ToList()[indicator.ForeCastPeriodSelected_Index];
        }

        private MRTFile InitMRTFile(HttpPostedFileBase file, out string serverPath)
        {
            string folder = "~/FilesUpload";

            MRTFile mRTFile = new MRTFile();
            mRTFile.Name = Path.GetFileName(file.FileName);
            mRTFile.Path = folder + "/" + mRTFile.Name;
            mRTFile.isDelete = false;
            serverPath = Path.Combine(Server.MapPath(folder), mRTFile.Name);
            return mRTFile;
        }

        private void SaveImResult(Indicator indicator)
        {
            var PMoQoY = db.PeriodMountOrQuarterOrYear.ToList();
            int indexId = 0;
            foreach (var i in indicator.ImportantIndicatorResultMeasurement)
            {
                i.UpdateDate = DateTime.Now;
                if (i.ID == 0)
                {
                    i.IndicatorID = indicator.ID;
                    i.CreateDate = DateTime.Now;
                    i.PeriodMounthOrQuarterOrYearID = PMoQoY[indexId].ID;
                    db.ImportantIndicatorResultMeasurement.Add(i);
                    indexId++;
                }
                else
                {
                    db.Entry(i).State = EntityState.Modified;
                }
            }
        }

        private void SaveForecastPeriod(ImportantIndicatorResultMeasurement imResult)
        {
            if (imResult.ID == 0)
            {
                switch (imResult.ForecastPeriod.Count)
                {
                    case 2:
                        int key_year = 0;
                        foreach (var i in imResult.ForecastPeriod)
                        {
                            i.ImportantIndicatorResultMeasureID = imResult.ID;
                            i.MountOrQuarterOrYear = _yearDic[key_year];
                            i.CreateDate = DateTime.Now;
                            i.UpdateDate = DateTime.Now;
                            db.ForecastPeriod.Add(i);
                            key_year++;
                        }
                        break;
                    case 4:
                        int key_quarter = 0;
                        foreach (var i in imResult.ForecastPeriod)
                        {
                            i.ImportantIndicatorResultMeasureID = imResult.ID;
                            i.MountOrQuarterOrYear = _quartersDic[key_quarter];
                            i.CreateDate = DateTime.Now;
                            i.UpdateDate = DateTime.Now;
                            db.ForecastPeriod.Add(i);
                            key_quarter++;
                        }
                        break;
                    case 12:
                        int key_month = 0;
                        foreach (var i in imResult.ForecastPeriod)
                        {
                            i.ImportantIndicatorResultMeasureID = imResult.ID;
                            i.MountOrQuarterOrYear = _monthsDic[key_month];
                            i.CreateDate = DateTime.Now;
                            i.UpdateDate = DateTime.Now;
                            db.ForecastPeriod.Add(i);
                            key_month++;
                        }
                        break;
                }
            }
            else
            {
                foreach (var i in imResult.ForecastPeriod)
                {
                    db.Entry(i).State = EntityState.Modified;
                }
            }
        }

    }
}