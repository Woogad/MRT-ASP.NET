using MRT_Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MRT_Demo.Services
{
    public class IndicatorService
    {
        static public List<Indicator> IndicatorsSearch(List<Indicator> indicatorList, string divisionSearch, string indicatorSearch)
        {
            if (divisionSearch == null) return indicatorList;

            if (divisionSearch != "")
            {
                List<Indicator> ownerSearchList = new List<Indicator>();
                foreach (var i in indicatorList)
                {
                    foreach (var j in i.IndicatorOwner)
                    {
                        if (j.Division.Equals(divisionSearch))
                        {
                            ownerSearchList.Add(i);
                            break;
                        }
                    }
                }
                indicatorList = ownerSearchList;
            }

            if (indicatorSearch != "")
            {
                List<Indicator> indicatorSearchList = new List<Indicator>();
                foreach (var i in indicatorList)
                {
                    if (i.Indicator1.Equals(indicatorSearch))
                    {
                        indicatorSearchList.Add(i);
                    }
                }
                indicatorList = indicatorSearchList;
            }
            return indicatorList;
        }

        static public List<Indicator> IndicatorsSearch(List<Indicator> indicatorList, string divisionSearch, string indicatorSearch, string activeState)
        {
            if (divisionSearch == null) return indicatorList;

            if (activeState != "")
            {
                bool isActive = false;
                if (activeState == "True")
                {
                    isActive = true;
                }
                List<Indicator> isActiveList = new List<Indicator>();
                foreach (var i in indicatorList)
                {
                    if (i.IsActive == isActive)
                    {
                        isActiveList.Add(i);
                    }
                }
                indicatorList = isActiveList;
            }

            if (divisionSearch != "")
            {
                List<Indicator> ownerSearchList = new List<Indicator>();
                foreach (var i in indicatorList)
                {
                    foreach (var j in i.IndicatorOwner)
                    {
                        if (j.Division.Equals(divisionSearch))
                        {
                            ownerSearchList.Add(i);
                            break;
                        }
                    }
                }
                indicatorList = ownerSearchList;
            }

            if (indicatorSearch != "")
            {
                List<Indicator> indicatorSearchList = new List<Indicator>();
                foreach (var i in indicatorList)
                {
                    if (i.Indicator1.Equals(indicatorSearch))
                    {
                        indicatorSearchList.Add(i);
                    }
                }
                indicatorList = indicatorSearchList;
            }
            return indicatorList;
        }
        static public List<Indicator> IndicatorsSearch(List<Indicator> indicatorList, string divisionSearch, string indicatorSearch, int year)
        {
            if (divisionSearch == null) return indicatorList;

            if (year != 0)
            {
                //TODO Search year of SOEPlan
            }

            if (divisionSearch != "")
            {
                List<Indicator> ownerSearchList = new List<Indicator>();
                foreach (var i in indicatorList)
                {
                    foreach (var j in i.IndicatorOwner)
                    {
                        if (j.Division.Equals(divisionSearch))
                        {
                            ownerSearchList.Add(i);
                            break;
                        }
                    }
                }
                indicatorList = ownerSearchList;
            }

            if (indicatorSearch != "")
            {
                List<Indicator> indicatorSearchList = new List<Indicator>();
                foreach (var i in indicatorList)
                {
                    if (i.Indicator1.Equals(indicatorSearch))
                    {
                        indicatorSearchList.Add(i);
                    }
                }
                indicatorList = indicatorSearchList;
            }
            return indicatorList;
        }
    }
}