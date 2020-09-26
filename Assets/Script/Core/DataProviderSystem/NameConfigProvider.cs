using System;
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Linq;
using GameCore.Loader;
namespace Solarmax
{
    public class NameConfigProvider : Singleton<NameConfigProvider>, IDataProvider
    {

        private List<NameConfig> firstNameList = new List<NameConfig>();
        private List<NameConfig> lastNameList = new List<NameConfig>();

        public string Path()
        {
            return "/data/RandomName.xml";
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
        public List<NameConfig> GetFirstNameList()
        {
            return firstNameList;
        }

        public List<NameConfig> GetLastNameList()
        {
            return lastNameList;
        }


        public void Load()
        {
            firstNameList.Clear();
            lastNameList.Clear();
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
                    var xElement = xmlDoc.Element("randNames");
                    if (xElement == null)
                        return;
                    var elements = xElement.Elements("randName");
                    foreach (var em in elements)
                    {
                        NameConfig item = new NameConfig();
                        if (item.Load(em))
                        {
                            if(item.type == 0)
                            {
                                firstNameList.Add(item);
                            }
                            else
                            {
                                lastNameList.Add(item);
                            }
                            
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggerSystem.Instance.Error("data/RandomName.xml resource failed " + e.ToString());
            }
        }
    }

    public class NameConfig : ICfgEntry
    {
        public string id;
        public int type;        //0ÊÇÐÕ£¬1ÊÇÃû
        public string chinese;
        public string english;

        public string Path()
		{
			return "data/name.txt";
		}

        public bool Load(XElement element)
        {
            id = element.GetAttribute("id", "0");
            type = Convert.ToInt32(element.GetAttribute("type", "0"));
            chinese = element.GetAttribute("chinese", "");
            english = element.GetAttribute("english", "");
            return true;
        }
	}
}
