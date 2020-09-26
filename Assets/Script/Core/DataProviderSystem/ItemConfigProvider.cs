using System;
using System.Collections.Generic;
namespace Solarmax
{
	public class ItemConfig
	{
		public System.Int32 id = 0;
		public System.String name = string.Empty;
		public System.String icon = string.Empty;
		public System.String desc = string.Empty;
	}
	public class ItemConfigProvider : Singleton<ItemConfigProvider>, IDataProvider
	{
		private List<ItemConfig> dataList = new List<ItemConfig>();
		public string Path()
		{
			return "data/item.txt";
		}

        public bool IsXML()
        {
            return false;
        }
        public void Load()
		{
			dataList.Clear ();

			ItemConfig item = null;
			while (!FileReader.IsEnd())
			{
				FileReader.ReadLine();
				item = new ItemConfig();
				item.id = FileReader.ReadInt();
				item.name = FileReader.ReadString();
				item.icon = FileReader.ReadString();
				item.desc = FileReader.ReadString();
				dataList.Add(item);
			}
		}
		public bool Verify()
		{
			return true;
		}
		public List<ItemConfig> GetAllData()
		{
			return dataList;
		}
		public ItemConfig GetData(System.Int32 id)
		{
			ItemConfig ret = null;
			for (int i = 0; i < dataList.Count; ++i)
			{
				if (dataList [i].id.Equals (id)) {
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
