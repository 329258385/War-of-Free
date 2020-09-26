using System;
using System.Collections.Generic;
namespace Solarmax
{
	public class GameVariableConfig
	{
		public System.Int32 id = 0;
		public string value = string.Empty;
		public string desc = string.Empty;
	}
	public class GameVariableConfigProvider : Singleton<GameVariableConfigProvider>, IDataProvider
	{
		private List<GameVariableConfig> dataList = new List<GameVariableConfig>();
		public string Path()
		{
			return "data/gamevariable.txt";
		}

        public bool IsXML()
        {
            return false;
        }

        public void Load()
		{
			dataList.Clear ();

			GameVariableConfig item = null;
			while (!FileReader.IsEnd())
			{
				FileReader.ReadLine();
				item = new GameVariableConfig();
				item.id = FileReader.ReadInt();
				item.value = FileReader.ReadString();
				dataList.Add(item);
			}
		}
		public bool Verify()
		{
			return true;
		}
		public List<GameVariableConfig> GetAllData()
		{
			return dataList;
		}
		public string GetData(System.Int32 id)
		{
			GameVariableConfig ret = null;
			for (int i = 0; i < dataList.Count; ++i)
			{
				if (dataList [i].id.Equals (id)) {
					ret = dataList [i];
					break;
				}
			}
			return ret.value;
		}
		public void LoadExtraData ()
		{

		}
	}
}
