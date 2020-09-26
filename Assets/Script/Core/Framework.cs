using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Solarmax
{
    public class Framework : Singleton<Framework>, Lifecycle
    {
        private string mVersion = string.Empty;
        private string mWritableRootDir = string.Empty;

        public bool Init()
        {
            do
            {
                if (!ConfigSystem.Instance.Init ()) break;
                if (!LoggerSystem.Instance.Init ()) break;
				LoggerSystem.Instance.Debug ("Framework    init begin.");
                if (!TimeSystem.Instance.Init ()) break;
				if (!EventSystem.Instance.Init ()) break;
                if (!DataProviderSystem.Instance.Init ()) break;
				if (!DataHandlerSystem.Instance.Init ()) break;
				#if !SERVER
				if (!EngineSystem.Instance.Init ()) break;
				if (!UpdateSystem.Instance.Init ()) break;
				if (!LocalStorageSystem.Instance.Init ()) break;
                if (!DataProviderSystem.Instance.LoadLanguage() ) break;
                if (!UISystem.Instance.Init ()) break;
                if (!ThirdPartySystem.Instance.Init ()) break;
				#endif
                if (!NetSystem.Instance.Init ()) break;
				if (!BattleSystem.Instance.Init ()) break;
				LoggerSystem.Instance.Debug("Framework init end.");
                return true;
            }
            while(false);

            return false;
        }

        /**
         * This function will be call in logicthread
         * */
        public void Tick(float interval)
        {
			ConfigSystem.Instance.Tick (interval);
            LoggerSystem.Instance.Tick (interval);
            DataProviderSystem.Instance.Tick (interval);
			DataHandlerSystem.Instance.Tick (interval);
			TimeSystem.Instance.Tick (interval);
			EventSystem.Instance.Tick (interval);
			#if !SERVER
			EngineSystem.Instance.Tick (interval);
			UpdateSystem.Instance.Tick (interval);
			LocalStorageSystem.Instance.Tick (interval);
			UISystem.Instance.Tick (interval);
			ThirdPartySystem.Instance.Tick (interval);
			#endif
            NetSystem.Instance.Tick (interval);

			BattleSystem.Instance.Tick (interval);
        }

        /**
         * This function will be call in renderthread
         * */
        public void Destroy()
        {
			LoggerSystem.Instance.Debug("Framework destroy begin");
			ConfigSystem.Instance.Destroy ();
            EventSystem.Instance.Destroy ();
            DataProviderSystem.Instance.Destroy ();
			DataHandlerSystem.Instance.Destroy ();
			TimeSystem.Instance.Destroy ();
			#if !SERVER
			EngineSystem.Instance.Destroy ();
			UpdateSystem.Instance.Destroy ();
			LocalStorageSystem.Instance.Destroy ();
			UISystem.Instance.Destroy ();
			ThirdPartySystem.Instance.Destroy ();
			#endif
            NetSystem.Instance.Destroy ();
			BattleSystem.Instance.Destroy ();

			LoggerSystem.Instance.Debug("Framework destroy end.");
            
            LoggerSystem.Instance.Destroy ();
        }


        public void SetVersion(string version)
        {
            mVersion = version;
            LocalStorageSystem.Instance.SetAppVersion(mVersion);
        }

        public string GetVersion()
        {
            return mVersion;
        }

        public void SetWritableRootDir(string path)
        {
            mWritableRootDir = path;
        }

        public string GetWritableRootDir()
        {
            return mWritableRootDir;
        }
	}

}
