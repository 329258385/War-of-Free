using System;
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
    public class NameFilterConfigProvider : Singleton<NameFilterConfigProvider>, IDataProvider
    {

        private List<NameFilterConfig> nameFilterList = new List<NameFilterConfig>();


        public string Path()
        {
            return "/data/NameFilter.xml";
        }

        public bool IsXML()
        {
            return true;
        }

        public void LoadExtraData()
        {
        }

        public bool Verify()
        {
            return true;
        }
        public List<NameFilterConfig> GetFilterList()
        {
            return nameFilterList;
        }

        public void Load()
        {
            nameFilterList.Clear();
            try
            {
                string url = UtilTools.GetStreamAssetsByPlatform(Path());
                if (string.IsNullOrEmpty(url))
                    return;
                WWW www = new WWW(url);
                while (!www.isDone) ;

                if (!string.IsNullOrEmpty(www.text))
                {
                    XDocument xmlDoc = XDocument.Parse(www.text);
                    var xElement = xmlDoc.Element("names");
                    if (xElement == null)
                        return;
                    var elements = xElement.Elements("name");
                    foreach (var em in elements)
                    {
                        NameFilterConfig item = new NameFilterConfig();
                        if (item.Load(em))
                        {
                            nameFilterList.Add(item);                            
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggerSystem.Instance.Error("data/RandomName.xml resource failed " + e.ToString());
            }
        }

        public bool nameCheck(string str)
        {
            string temp;            
            for (int i = 0; i < nameFilterList.Count; i++)
            {
                temp = nameFilterList[i].desc;
                bool isContains = str.Contains(temp);
                if(isContains)
                {
                    return true;
                }
            }
            return false;            
        }
    }

    public class NameFilterConfig : ICfgEntry
    {
        public string id;
        public string desc;        

        public bool Load(XElement element)
        {
            id = element.GetAttribute("id", "0");
            desc = element.GetAttribute("desc", "0");
            return true;
        }
	}
}
