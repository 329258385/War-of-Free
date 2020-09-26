using System;
using System.Collections.Generic;

namespace Solarmax
{
    public class UIWindowConfig
    {
        public int mID = -1;
        public string mName = string.Empty;
        public string mPrefabPath = string.Empty;
		public bool mHideWithDestroy = true;

    }

    public class UIWindowConfigProvider : Singleton<UIWindowConfigProvider>, IDataProvider
    {
        private List<UIWindowConfig> mDataList = new List<UIWindowConfig>();

        public string Path()
        {
            return "data/uiwindow.txt";
        }

        public bool IsXML()
        {
            return false;
        }


        public void Load()
		{
			mDataList.Clear ();

            UIWindowConfig item = null;
            while (!FileReader.IsEnd())
            {
                FileReader.ReadLine();
                item = new UIWindowConfig();
                item.mID = FileReader.ReadInt();
                item.mName = FileReader.ReadString();
                item.mPrefabPath = FileReader.ReadString();
				item.mHideWithDestroy = FileReader.ReadBoolean ();

                mDataList.Add(item);
            }
        }

        public bool Verify()
        {
//            foreach(var i in mDataList)
//	        {
//                LoggerSystem.Instance.Debug("UIWindow   " + i.mID + "  " + i.mName);
//	        }
	        return true;
        }

        public List<UIWindowConfig> GetAllData()
        {
            return mDataList;
        }

		public UIWindowConfig GetData(string name)
		{
			UIWindowConfig ret = null;
			for (int i = 0; i < mDataList.Count; ++i) {
				if (mDataList [i].mName.Equals (name)) {
					ret = mDataList [i];
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
