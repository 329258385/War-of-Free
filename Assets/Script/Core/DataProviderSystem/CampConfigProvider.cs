using System;
using UnityEngine;
using GameCore.Loader;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Solarmax
{
	public class CampConfig : ICfgEntry
    {
		public string       id;
		public Color32      campcolor;
		public float        speed;
		public float        attack;
		public float        maxhp;
		public float        capturedspeed;
		public float        occupiedspeed;
		public float        producespeed;
        public float        speedaddition;
        public float        attackaddition;
        public float        maxhpaddition;
        public float        hidefly;
        public float        hidepop;
        public float        becapturedspeed;
        public float        stopbecapture;
        public float        quickmove;
        public bool Load(XElement element)
        {
            id              = element.GetAttribute("id", "" );
            speed           = Convert.ToSingle(element.GetAttribute("speed", "1.2" ));
            attack          = Convert.ToSingle(element.GetAttribute("attack", "10"));
            maxhp           = Convert.ToSingle(element.GetAttribute("maxhp", "100"));
            capturedspeed   = Convert.ToSingle(element.GetAttribute("capturedspeed", "1"));
            occupiedspeed   = Convert.ToSingle(element.GetAttribute("occupiedspeed", "1"));
            producespeed    = Convert.ToSingle(element.GetAttribute("producespeed", "1"));
            speedaddition   = Convert.ToSingle(element.GetAttribute("speedaddition", "0"));
            attackaddition  = Convert.ToSingle(element.GetAttribute("attackaddition", "0"));
            maxhpaddition   = Convert.ToSingle(element.GetAttribute("maxhpaddition", "0"));
            hidefly         = Convert.ToSingle(element.GetAttribute("hidefly", "0"));
            hidepop         = Convert.ToSingle(element.GetAttribute("hidepop", "0"));
            becapturedspeed = Convert.ToSingle(element.GetAttribute("becapturedspeed", "1"));
            stopbecapture   = Convert.ToSingle(element.GetAttribute("stopbecapture", "0"));
            quickmove       = Convert.ToSingle(element.GetAttribute("quickmove", "0"));

            string t_color = element.GetAttribute("campcolor", "ffffff");
            try
            {
                if (t_color.Length == 0)
                {//如果为空
                    campcolor = new Color32( 0x00,0x00,0x00,0x00);//设为黑色
                }
                else
                {//转换颜色
                    campcolor = new Color32(System.Byte.Parse(t_color.Substring(0, 2),System.Globalization.NumberStyles.AllowHexSpecifier),
                                            System.Byte.Parse(t_color.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier),
                                            System.Byte.Parse(t_color.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier),
                                            0x00);
                }
            }
            catch
            {//设为黑色
                campcolor = new Color32(0x00, 0x00, 0x00, 0x00);//设为黑色
            }

            return true;
        }
	}

	public class CampConfigConfigProvider : Singleton<CampConfigConfigProvider>, IDataProvider
	{

		private List<CampConfig> dataList = new List<CampConfig>();

		public CampConfigConfigProvider()
		{

		}

		public string Path()
		{
			return "/data/Camp.xml";
		}

        public bool IsXML()
        {
            return true;
        }

        public void Load()
		{
			dataList.Clear ();
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
                    var xElement = xmlDoc.Element("camps");
                    if (xElement == null)
                        return;

                    var elements = xElement.Elements("camp");
                    foreach (var em in elements)
                    {
                        CampConfig item = new CampConfig();
                        if (item.Load(em))
                        {
                            dataList.Add(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggerSystem.Instance.Error("data/Camp.xml resource failed " + e.ToString());
            }
        }

		public bool Verify()
		{
			return true;
		}
		public List<CampConfig> GetAllData()
		{
			return dataList;
		}
		public CampConfig GetData(string id)
		{
			CampConfig ret = null;
			for (int i = 0; i < dataList.Count; ++i)
			{
				if (dataList [i].id == id)
				{
					ret = dataList [i];
					break;
				}
			}
			return ret;
		}
		public void LoadExtraData ()
		{

		}
	}
}
