using System;
using UnityEngine;
using GameCore.Loader;
using System.Collections.Generic;
using System.Xml.Linq;

/// <summary>
/// 本地化语言
/// </summary>
namespace Solarmax
{
    public class LanguageConfig : ICfgEntry
    {
        public int      mID = -1;
        public string   mData = string.Empty;

        public bool Load(XElement element)
        {

            return true;
        }

        public bool Load(XElement element, string language )
        {
            mID     = Convert.ToInt32(element.GetAttribute("id"));
            mData   = element.GetAttribute(language, "");
            return true;
        }
    }


    public class LanguageDataProvider : Singleton<LanguageDataProvider>, IDataProvider
    {
		private Dictionary<int, LanguageConfig> mDataList = new Dictionary<int, LanguageConfig>();

        public LanguageDataProvider()
        {

        }

        public string Path()
        {
            return "/data/Dictionary.xml";
        }

        public bool IsXML()
        {
            return true;
        }

        /// <summary>
        /// 启动时根据本地语言设置,读取数据配置
        /// </summary>
        public void Load()
        {
			mDataList.Clear ();
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
                    var xElement = xmlDoc.Element("languages");
                    if (xElement == null)
                        return;

                    SystemLanguage language = (SystemLanguage)LocalAccountStorage.Get().localLanguage;
                    string strLanguage      = GetLanguageNameConfig(language);

                    var elements    = xElement.Elements("language");
                    foreach (var em in elements)
                    {
                        LanguageConfig item = new LanguageConfig();
                        if (item.Load(em, strLanguage))
                        {
                            mDataList.Add(item.mID, item );
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggerSystem.Instance.Error("data/dictionary.xml resource failed " + e.ToString());
            }
        }


        /// <summary>
        /// 加载特定语言
        /// </summary>
        public void LoadLanguage(SystemLanguage language)
        {
            Load();
        }


        /// <summary>
        /// 根据选择的语言,返回配置表中的字段名称,默认支持中英文
        /// </summary>
        public string GetLanguageNameConfig(SystemLanguage language )
        {
            switch(language )
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    return "chinese";

                default:
                    return "english";
            }
        }

        public bool Verify()
        {
	        return true;
        }

		public string GetData(int id)
		{
			LanguageConfig item = null;
			if (mDataList.TryGetValue (id, out item)) {
				return item.mData;
			}

			return string.Empty;
		}

		public static string GetValue(int id)
		{
			return Instance.GetData(id);
		}

		public static string Format (int id, params object[] args)
		{
			string str = GetValue (id);
			return string.Format (str, args);
		}


		public void LoadExtraData ()
		{

		}
    }
}
