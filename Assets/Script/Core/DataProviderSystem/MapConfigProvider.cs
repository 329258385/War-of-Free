using System;
using UnityEngine;
using GameCore.Loader;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;

namespace Solarmax
{ 
    public class MapConfig
    {
        public List<MapLineConfig> mlcList = new List<MapLineConfig>();
        public List<MapBuildingConfig> mbcList = new List<MapBuildingConfig>();
        public List<MapPlayerConfig> mpcList = new List<MapPlayerConfig>();
        public string name;
        public const string root = "map";
        public int player_count;
        public bool vertical;
        public string audio;
        public string defaultai = "-1";
        public string id;
        public string teamAITypes;
        public List<int> teamAiType = new List<int>();

        public MapConfig(string p_name)
        {
            name = p_name;
        }

        public bool IsXML()
        {
            return true;
        }

        public string Path()
        {
            return "/data/maplist/"+ name + ".xml";
        }

        public void Load()
        {
            mlcList.Clear();
            mbcList.Clear();
            mpcList.Clear();
            try
            {
                string url = UtilTools.GetStreamAssetsByPlatform(Path());
                if (string.IsNullOrEmpty(url))
                    return;

                /// 试着用www 读取 
                WWW www = new WWW(url);
                while (!www.isDone) ;

                //text = Resources.Load(Path()).ToString();

                if (!string.IsNullOrEmpty(www.text))
                {
                    XDocument xmlDoc = XDocument.Parse(www.text);
                    var xElement = xmlDoc.Element(root);
                    if (xElement == null)
                        return;

                    id = xElement.GetAttribute("id", "");
                    player_count = Convert.ToInt32(xElement.GetAttribute("player_count", "0"));
                    vertical = Convert.ToBoolean(xElement.GetAttribute("vertical", ""));
                    audio = xElement.GetAttribute("audio", "");
                    defaultai = xElement.GetAttribute("defaultAIStrategy", "-1");
                    teamAITypes = xElement.GetAttribute("teamAITypes", "");
                    if (teamAITypes != "")
                    {
                        string[] ids = teamAITypes.Split(',');
                        foreach (var item in ids)
                        {
                            teamAiType.Add(Convert.ToInt32(item));
                        }
                    }
                    for (int i = teamAiType.Count; i < (int)TEAM.TeamMax; i ++)
                    {
                        teamAiType.Add(-1);
                    }

                    //读取所有mapbuilding数据
                    var elements_mbs = xElement.Elements("mapbuildings");
                    if (elements_mbs != null)
                    {
                        foreach (var em in elements_mbs)
                        {
                            var elements_mb = em.Elements("mapbuilding");
                            foreach (var em_mb in elements_mb)
                            {
                                MapBuildingConfig item = new MapBuildingConfig();
                                if (item.Load(em_mb))
                                {
                                    mbcList.Add(item);
                                }
                            }
                        }
                    }
                    //读取所有mapline数据
                    var elements_mls = xElement.Elements("maplines");
                    if (elements_mls != null)
                    {
                        foreach (var em in elements_mls)
                        {
                            var elements_ml = em.Elements("mapline");
                            foreach (var em_ml in elements_ml)
                            {
                                MapLineConfig item = new MapLineConfig();
                                if (item.Load(em_ml))
                                {
                                    mlcList.Add(item);
                                }
                            }
                        }
                    }
                    //读取所有mapplayer数据
                    var elements_mps = xElement.Elements("mapplayers");
                    if (elements_mps != null)
                    {
                        foreach (var em in elements_mps)
                        {
                            var elements_mp = em.Elements("mapplayer");
                            foreach (var em_mp in elements_mp)
                            {
                                MapPlayerConfig item = new MapPlayerConfig();
                                if (item.Load(em_mp))
                                {
                                    mpcList.Add(item);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggerSystem.Instance.Error("data/maplist/" + name + ".xml resource failed " + e.ToString());
            }
        }

        public void LoadExter( string filePath )
        {
            mlcList.Clear();
            mbcList.Clear();
            mpcList.Clear();
            try
            {
                string url  = filePath;
                WWW www     = new WWW(url);
                while (!www.isDone) ;
                if (!string.IsNullOrEmpty(www.text))
                {
                    XDocument xmlDoc = XDocument.Parse(www.text);
                    var xElement = xmlDoc.Element(root);
                    if (xElement == null)
                        return;

                    id = xElement.GetAttribute("id", "");
                    player_count = Convert.ToInt32(xElement.GetAttribute("player_count", "0"));
                    vertical = Convert.ToBoolean(xElement.GetAttribute("vertical", ""));
                    audio = xElement.GetAttribute("audio", "");
                    defaultai = xElement.GetAttribute("defaultAIStrategy", "-1");
                    teamAITypes = xElement.GetAttribute("teamAITypes", "");
                    if (teamAITypes != "")
                    {
                        string[] ids = teamAITypes.Split(',');
                        foreach (var item in ids)
                        {
                            teamAiType.Add(Convert.ToInt32(item));
                        }
                    }
                    for (int i = teamAiType.Count; i < (int)TEAM.TeamMax; i ++)
                    {
                        teamAiType.Add(-1);
                    }

                    //读取所有mapbuilding数据
                    var elements_mbs = xElement.Elements("mapbuildings");
                    if (elements_mbs != null)
                    {
                        foreach (var em in elements_mbs)
                        {
                            var elements_mb = em.Elements("mapbuilding");
                            foreach (var em_mb in elements_mb)
                            {
                                MapBuildingConfig item = new MapBuildingConfig();
                                if (item.Load(em_mb))
                                {
                                    mbcList.Add(item);
                                }
                            }
                        }
                    }
                    //读取所有mapline数据
                    var elements_mls = xElement.Elements("maplines");
                    if (elements_mls != null)
                    {
                        foreach (var em in elements_mls)
                        {
                            var elements_ml = em.Elements("mapline");
                            foreach (var em_ml in elements_ml)
                            {
                                MapLineConfig item = new MapLineConfig();
                                if (item.Load(em_ml))
                                {
                                    mlcList.Add(item);
                                }
                            }
                        }
                    }
                    //读取所有mapplayer数据
                    var elements_mps = xElement.Elements("mapplayers");
                    if (elements_mps != null)
                    {
                        foreach (var em in elements_mps)
                        {
                            var elements_mp = em.Elements("mapplayer");
                            foreach (var em_mp in elements_mp)
                            {
                                MapPlayerConfig item = new MapPlayerConfig();
                                if (item.Load(em_mp))
                                {
                                    mpcList.Add(item);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggerSystem.Instance.Error("data/maplist/" + name + ".xml resource failed " + e.ToString());
            }
        }

        public bool Delete()
        {
            bool ret = false;
            try
            {
                string url = string.Empty;

                /// 试着用www 读取 
                //#if UNITY_EDITOR
                url = Application.streamingAssetsPath + Path();
                //#else
                //url = Application.streamingAssetsPath + Path();
                //#endif
                if (File.Exists(url))
                {
                    File.Delete(url);
                    ret = true;
                }
            }
            catch (Exception e)
            {
                LoggerSystem.Instance.Error("data/maplist/" + name + ".xml delete failed " + e.ToString());
            }
            return ret;

        }

        public bool Save()
        {
            bool ret = false;

            try
            {
                string url = string.Empty;

                /// 试着用www 读取 
                //#if UNITY_EDITOR
                url = Application.streamingAssetsPath + Path();
                //#else
                //url = Application.streamingAssetsPath + Path();
                //#endif
                XmlTextWriter writer = new XmlTextWriter(url, new System.Text.UTF8Encoding(false));
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement(root);

                writer.WriteAttributeString("id", name);
                writer.WriteAttributeString("player_count", player_count.ToString());
                writer.WriteAttributeString("vertical", vertical.ToString());
                writer.WriteAttributeString("audio", audio);
                writer.WriteAttributeString("defaultAIStrategy", defaultai);
                writer.WriteAttributeString("teamAITypes", teamAITypes);

                //写入mapline
                if (mlcList.Count > 0)
                {
                    writer.WriteStartElement("maplines");
                    foreach (MapLineConfig ml in mlcList)
                    {
                        writer.WriteStartElement("mapline");
                        writer.WriteAttributeString("point1", ml.point1);
                        writer.WriteAttributeString("point2", ml.point2);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }

                //写入mapbuilding
                if (mbcList.Count > 0)
                {
                    writer.WriteStartElement("mapbuildings");
                    foreach (MapBuildingConfig item in mbcList)
                    {
                        writer.WriteStartElement("mapbuilding");

                        writer.WriteAttributeString("id", item.id);
                        writer.WriteAttributeString("type", item.type);
                        writer.WriteAttributeString("size", item.size.ToString());
                        writer.WriteAttributeString("x", item.x.ToString());
                        writer.WriteAttributeString("y", item.y.ToString());
                        writer.WriteAttributeString("camption", item.camption.ToString());
                        writer.WriteAttributeString("tag", item.tag);
                        writer.WriteAttributeString("orbit", item.orbit.ToString());
                        writer.WriteAttributeString("orbitParam1", item.orbitParam1);
                        writer.WriteAttributeString("orbitParam2", item.orbitParam2);
                        writer.WriteAttributeString("orbitClockWise", item.orbitClockWise.ToString());
                        writer.WriteAttributeString("fAngle", item.fAngle.ToString());
                        if (item.lasergunRange != 0f)
                        {
                            writer.WriteAttributeString("lasergunAngle", item.lasergunAngle.ToString());
                            writer.WriteAttributeString("lasergunRange", item.lasergunRange.ToString());
                            writer.WriteAttributeString("lasergunRotateSkip", item.lasergunRotateSkip.ToString());
                        }
                        writer.WriteAttributeString("transformBulidingID", item.transformBulidingID);
                        if (!string.IsNullOrEmpty(item.aistrategy) && item.aistrategy != "-1")
                        {
                            writer.WriteAttributeString("aistrategy", item.aistrategy);
                        }
                        if (item.type == "BarrierPoint")
                        {
                            writer.WriteAttributeString("bpRange", item.fbpRange.ToString());
                        }

                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                //写入mapplayer
                if (mpcList.Count > 0)
                {
                    writer.WriteStartElement("mapplayers");
                    foreach (MapPlayerConfig mp in mpcList)
                    {
                        writer.WriteStartElement("mapplayer");

                        writer.WriteAttributeString("id", mp.id);
                        writer.WriteAttributeString("tag", mp.tag);
                        writer.WriteAttributeString("ship", mp.ship.ToString());
                        writer.WriteAttributeString("camption", mp.camption.ToString());

                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }

                //关闭
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }
            catch (Exception e)
            {
                LoggerSystem.Instance.Error("data/maplist/" + name + ".xml save failed " + e.ToString());
            }
            return ret;
        }

        public bool Verify()
        {
            return true;
        }

        public void LoadExtraData()
        {

        }
    }

    public class MapBuildingConfig : ICfgEntry
    {
        public string id;
        public string type;
        public int size;
        public Single x;
        public Single y;
        public int camption;
        public string tag;
        public int orbit;
        public string orbitParam1;
        public string orbitParam2;
        public bool orbitClockWise;
        public Single fAngle;
        public Single lasergunAngle;
        public Single lasergunRange;
        public Single lasergunRotateSkip;
        public string transformBulidingID;
        public string aistrategy;
        public Single fbpRange;
        public List<int> buildIds = new List<int>();

        public bool Load(XElement element)
        {
            id = element.GetAttribute("id", "");
            type = element.GetAttribute("type", "");
            size = Convert.ToInt32(element.GetAttribute("size", "1"));
            x = Convert.ToSingle(element.GetAttribute("x", "0.0"));
            y = Convert.ToSingle(element.GetAttribute("y", "0.0"));
            camption = Convert.ToInt32(element.GetAttribute("camption", "0"));
            tag = element.GetAttribute("tag", "");
            orbit = Convert.ToInt32(element.GetAttribute("orbit", "0"));
            orbitParam1 = element.GetAttribute("orbitParam1", "");
            orbitParam2 = element.GetAttribute("orbitParam2", "");
            orbitClockWise = Convert.ToBoolean(element.GetAttribute("orbitClockWise", "false"));
            fAngle = Convert.ToSingle(element.GetAttribute("fAngle", "0.0"));
            lasergunAngle = Convert.ToSingle(element.GetAttribute("lasergunAngle", "0.0"));
            lasergunRange = Convert.ToSingle(element.GetAttribute("lasergunRange", "0.0"));
            lasergunRotateSkip = Convert.ToSingle(element.GetAttribute("lasergunRotateSkip", "30.0"));
            transformBulidingID = element.GetAttribute("transformBulidingID", "");
            aistrategy = element.GetAttribute("aistrategy", "-1");
            fbpRange = Convert.ToSingle(element.GetAttribute("bpRange", "1.0"));
            if (transformBulidingID != "")
            {
                string[] ids = transformBulidingID.Split(',');
                foreach (var item in ids)
                {
                    buildIds.Add(Convert.ToInt32(item));
                }
            }
            return true;
        }
    }

    public class MapPlayerConfig : ICfgEntry
    {
        public string id;
        public string tag;
        public int ship;
        public int camption;

        public bool Load(XElement element)
        {
            id = element.GetAttribute("id", "");
            tag = element.GetAttribute("tag", "");
            ship = Convert.ToInt32(element.GetAttribute("ship", "0"));
            camption = Convert.ToInt32(element.GetAttribute("camption", "0"));
            return true;
        }
    }

    public class MapLineConfig : ICfgEntry
    {
        public string point1;
        public string point2;
        private const string datatitle = "line";

        public bool Load(XElement element)
        {
            point1 = element.GetAttribute("point1", "");
            point2 = element.GetAttribute("point2", "");
            return true;
        }
    }

    public class MapBarrierPoingConfig : ICfgEntry
    {
        public string tag;
        public float range;

        public bool Load(XElement element)
        {
            tag = element.GetAttribute("tag");
            range = Convert.ToSingle(element.GetAttribute("range", "1.0"));
            return true;
        }
    }


    public class MapListConfig : ICfgEntry
    {

        /// <summary>
        /// 
        /// </summary>
        public string   mapID;
        public int      version;
        public int      nAdd = 0;


        public bool Load(XElement element)
        {
            mapID       = element.GetAttribute("name", "");
            if( mapID == "DLAM01")
            {
                int i = 0;
                i++;
            }
            version     = Convert.ToInt32(element.GetAttribute("version", "0"));
            nAdd        = Convert.ToInt32(element.GetAttribute("addss", "0"));
            return true;
        }
    }


    public class MapConfigProvider : Singleton<MapConfigProvider>, IDataProvider
    {

        public Dictionary<string, MapListConfig> mapVersion = new Dictionary<string, MapListConfig>();
        public Dictionary<string, MapConfig> mapDictionary = new Dictionary<string, MapConfig>();

        public string Path()
        {
            return "/data/MapList.xml";
        }

        public bool Delete(string name)
        {
            bool ret = false;
            if (mapDictionary.ContainsKey(name))
            {
                GetData(name).Delete();
                mapDictionary.Remove(name);
                ret = true;

                if (mapVersion.ContainsKey(name))
                    mapVersion.Remove(name);
                WriteMapList();
            }
            return ret;
        }

        public bool WriteMapList(bool bSync = false )
        {
            bool ret = false;
            try
            {
                string url = string.Empty;

                /// 试着用www 读取 
                //#if UNITY_EDITOR
                url = Application.streamingAssetsPath + Path();
                //#else
                //url = Application.streamingAssetsPath + Path();
                //#endif
                XmlTextWriter writer = new XmlTextWriter(url, new System.Text.UTF8Encoding(false));
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("maps");

                foreach (MapListConfig mc in mapVersion.Values)
                {
                    writer.WriteStartElement("map");
                    writer.WriteAttributeString("name", mc.mapID);

                    if (bSync)
                    {
                        mc.nAdd = 0;
                        writer.WriteAttributeString("version", (mc.version + mc.nAdd).ToString());
                    }
                    else
                    {
                        writer.WriteAttributeString("version", mc.version.ToString());
                    }

                    writer.WriteAttributeString("add", mc.nAdd.ToString());
                    writer.WriteEndElement();
                }

                //关闭
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                ret = true;
            }
            catch (Exception e)
            {
                LoggerSystem.Instance.Error("data/MapList.xml save failed " + e.ToString());
            }
            return ret;
        }

        public bool Save(MapConfig mc)
        {
            bool ret = false;
            if (mapDictionary.ContainsKey(mc.name))
            {
                mc.Save();
                ret = true;    
            }
            else
            {
                mapDictionary.Add(mc.name, mc);


                MapListConfig ver = new MapListConfig();
                ver.mapID         = mc.name;
                ver.version       = 0;
                ver.nAdd          = 0;
                mapVersion.Add(ver.mapID,  ver );
                mc.Save();
                ret = true;
            }
            WriteMapList();
            return ret;
        }

        public void SavaAll()
        {
            foreach(KeyValuePair<string,MapConfig> pair in mapDictionary)
            {
                Save(pair.Value);
            }
        }

        public bool IsXML()
        {
            return true;
        }

        public void Load()
        {

            mapVersion.Clear();
            mapDictionary.Clear();
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
                    var xElement = xmlDoc.Element("maps");
                    if (xElement == null)
                        return;

                    var elements = xElement.Elements("map");
                    foreach (var em in elements)
                    {
                        MapListConfig item = new MapListConfig();
                        if (item.Load(em))
                        {
                            MapConfig tmc = new MapConfig(item.mapID);
                            tmc.Load();
                            mapDictionary.Add(item.mapID, tmc);
                            mapVersion.Add(item.mapID, item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggerSystem.Instance.Error("maplist resource failed " + e.ToString());
            }

            LoadExtraData();
        }

        public bool Verify()
        {
            return true;
        }
        public Dictionary<string, MapConfig> GetAllData()
        {
            return mapDictionary;
        }
        public MapConfig GetData(string id)
        {
            MapConfig ret = null;
            if (mapDictionary.ContainsKey(id))
            {
                ret = mapDictionary[id];
            }
            return ret;
        }



        public MapListConfig GetMapVersion( string Id )
        {
            MapListConfig cfg = null;
            if (mapVersion.ContainsKey(Id))
                return mapVersion[Id];
            return cfg;
        }


        /// <summary>
        /// 修改地图的版本
        /// </summary>
        public bool ModifymapVersion( string mapId )
        {
            MapListConfig cfg = GetMapVersion(mapId);
            if( cfg != null )
            {
                cfg.nAdd += 1;
                return true;
            }

            return true;
        }

        public bool SyncmapVersion(string mapId)
        {
            MapListConfig cfg = GetMapVersion(mapId);
            if (cfg != null)
            {
                cfg.version += cfg.nAdd;
                cfg.nAdd    = 0;
                return true;
            }

            return false;
        }


        /// <summary>
        /// 额外编辑数据
        /// </summary>
        public void LoadExtraData()
        {
            Dictionary<string, MapListConfig> temp = LoadExtraMapList();
            if( temp != null && temp.Count > 0 )
            {
                LoadExtraMap(temp);
            }
            temp.Clear();
        }

        public Dictionary<string, MapListConfig> LoadExtraMapList()
        {

            Dictionary<string, MapListConfig> temp = new Dictionary<string, MapListConfig>();
            try
            {
                string filePath = string.Empty;
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    filePath = Application.dataPath + "/cache/EditMap/MapList.xml";
                }
                else if (Application.platform == RuntimePlatform.Android)
                {
                    filePath = Application.persistentDataPath + "/EditMap/MapList.xml";
                }
                else if( Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    filePath = "file://" + Application.persistentDataPath + "/EditMap/MapList.xml";
                }
                else if (Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    filePath = Application.dataPath + "/cache/EditMap/MapList.xml";
                }

                StreamReader file = File.OpenText(filePath);
                string text = file.ReadToEnd();
                if (!string.IsNullOrEmpty(text))
                {
                    XDocument xmlDoc = XDocument.Parse(text);
                    var xElement = xmlDoc.Element("maps");
                    if (xElement == null)
                        return null;

                    var elements = xElement.Elements("map");
                    foreach (var em in elements)
                    {
                        MapListConfig item = new MapListConfig();
                        if (item.Load(em))
                        {
                            MapListConfig item1 = null;
                            mapVersion.TryGetValue(item.mapID, out item1);
                            if (item1 != null)
                            {
                                if (item.version > item1.version)
                                {
                                    item1.version = item.version;
                                    temp.Add(item.mapID, item);
                                }
                            }
                            else
                            {
                                mapVersion.Add(item.mapID, item);
                                temp.Add(item.mapID, item);
                            }
                        }
                    }
                }
                file.Close();
            }
            catch (Exception e)
            {
                LoggerSystem.Instance.Error("maplist resource failed " + e.ToString());
            }

            return temp;
        }

        public void LoadExtraMap(Dictionary<string, MapListConfig> maps )
        {
            string filePath = string.Empty;
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                filePath = Application.dataPath + "/cache/EditMap/data/";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                filePath = Application.persistentDataPath + "/EditMap/data/";
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                filePath = "file://" + Application.persistentDataPath + "/EditMap/data/";
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                filePath = Application.dataPath + "/cache/EditMap/data/";
            }


            foreach (var mc in maps)
            {
                try
                {
                    string url  = filePath + mc.Value.mapID + ".xml";
                    MapConfig map = new MapConfig(mc.Value.mapID);
                    map.LoadExter(url);
                    {
                        this.mapDictionary.Add(map.id, map);
                    }
                }
                catch (Exception e)
                {
                    LoggerSystem.Instance.Error("data/MapList.xml save failed " + e.ToString());
                }
            }
        }
        /// <summary>
        /// 更新原来管卡的内容
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="mapId"></param>
        public void DownLoadMap(string filePath, string mapId )
        {

            bool bNewMap = false;
            MapConfig map = GetData(mapId);
            if (map == null)
            {
                map = new MapConfig(mapId);
                map.LoadExter(filePath);
                map.id = mapId;
                bNewMap = true;
            }
            else
            {
                map.LoadExter(filePath);
            }
            if (bNewMap)
            {
                mapDictionary.Add(map.id, map);
            }
        }

        public Stack<string> DownLoadMapList(string filePath)
        {
            try
            {
                Stack<string> list = new Stack<string>();
                StreamReader file = File.OpenText(filePath);
                string text = file.ReadToEnd();
                if (!string.IsNullOrEmpty(text))
                {
                    XDocument xmlDoc = XDocument.Parse(text);
                    var xElement = xmlDoc.Element("maps");
                    if (xElement == null)
                        return null;

                    var elements = xElement.Elements("map");
                    foreach (var em in elements)
                    {
                        MapListConfig item1 = new MapListConfig();
                        if (item1.Load(em))
                        {
                            MapListConfig item2 = null;
                            mapVersion.TryGetValue(item1.mapID, out item2 );
                            if (item2 == null)
                                list.Push(item1.mapID);
                            else if (item1.version > item2.version)
                                list.Push(item1.mapID);
                        }
                    }
                }
                file.Close();
                return list;
            }
            catch (Exception e)
            {
                LoggerSystem.Instance.Error("maplist resource failed " + e.ToString());
            }
            return null;
        }


        /// <summary>
        /// 额外编辑所有地图
        /// </summary>
        public Dictionary<string, MapConfig> GetAllDataExtra()
        {
            return mapDictionary;
        }
    }
}
