using System;
using UnityEngine;
using GameCore.Loader;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;

namespace Solarmax
{ 
    public class AiConfig
    {
        public List<AIStrategyConfig> aiStrategy = new List<AIStrategyConfig>();

        public AiConfig()
        {
        }

        public bool IsXML()
        {
            return true;
        }

        public string Path()
        {
            return "/data/aistrategy.xml";
        }

        public void Load()
        {
            aiStrategy.Clear();
            try
            {
                string url = UtilTools.GetStreamAssetsByPlatform(Path());
                if (string.IsNullOrEmpty(url))
                    return;

                /// 试着用www 读取 
                WWW www = new WWW(url);
                while (!www.isDone) ;

                if (!string.IsNullOrEmpty(www.text))
                {
                    XDocument xmlDoc = XDocument.Parse(www.text);
                    var xElement = xmlDoc.Element("aistrategys");
                    if (xElement == null)
                        return;

                    var aistrategy = xElement.Elements("aistrategy");
                    foreach (var ai in aistrategy)
                    {
                        AIStrategyConfig item = new AIStrategyConfig();
                        if (item.Load(ai))
                        {
                            aiStrategy.Add(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggerSystem.Instance.Error(Path() +  "resource failed !" + e.ToString());
            }
        }

        public bool Delete()
        {
            return true;
        }

        public bool Save()
        {
            return true;
        }
        
        public bool Verify()
        {
            return true;
        }

        public void LoadExtraData()
        {

        }
    }

    public class AIStrategyConfig : ICfgEntry
    {
        public int id;
        public string name;
        public string desc;
        public string actions;
        public List<int> aiActions = new List<int>();

        public bool Load(XElement element)
        {
            id = Convert.ToInt32(element.GetAttribute("id", "1"));
            name = element.GetAttribute("name", "");
            actions = element.GetAttribute("actions", "");
            desc = element.GetAttribute("desc", "");
            if (actions != "")
            {
                string[] ids = actions.Split(',');
                foreach (var item in ids)
                {
                    aiActions.Add(Convert.ToInt32(item));
                }
            }
            return true;
        }
    }

    public class AIStrategyConfigProvider : Singleton<AIStrategyConfigProvider>, IDataProvider
    {
        static public AiConfig aiConfig = new AiConfig();
        public string Path()
        {
            return "/data/aistrategy.xml";
        }
        public bool Delete(string name)
        {
            return true;
        }
        public bool IsXML()
        {
            return true;
        }
        public void Load()
        {
            aiConfig.Load();
        }
        public bool Verify()
        {
            return true;
        }
        public void LoadExtraData()
        {

        }
        public List<int> GetAIActions(int strategy)
        {
            for (int i = 0; i < aiConfig.aiStrategy.Count; i++)
            {
                if (aiConfig.aiStrategy[i].id == strategy)
                {
                    return aiConfig.aiStrategy[i].aiActions;
                }
            }
            return null;
        }
        public List<int> GetAIActions(string strategy)
        {
            for (int i = 0; i < aiConfig.aiStrategy.Count; i++)
            {
                if (aiConfig.aiStrategy[i].name == strategy)
                {
                    return aiConfig.aiStrategy[i].aiActions;
                }
            }
            return null;
        }
        public int GetAIStrategyByName(string name)
        {
            for (int i = 0; i < aiConfig.aiStrategy.Count; i++)
            {
                if (aiConfig.aiStrategy[i].name == name)
                {
                    return aiConfig.aiStrategy[i].id;
                }
            }
            return -1;
        }
    }

}
