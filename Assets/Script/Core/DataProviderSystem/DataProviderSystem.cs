using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Solarmax
{
    public class DataProviderSystem : Singleton<DataProviderSystem>, Lifecycle
    {
        private List<IDataProvider> mDataProvider       = new List<IDataProvider>();

        /// <summary>
        /// 语言配置单独拿出来,因为数据初始化顺序游客不一样了 fixed ljp
        /// </summary>
        private LanguageDataProvider languageProvider   = LanguageDataProvider.Instance;

        public bool Init()
        {
			LoggerSystem.Instance.Debug("DataProviderSystem   init   begin");

            // 注册
            RegisterDataProvider(NameFilterConfigProvider.Instance);
			RegisterDataProvider (ItemConfigProvider.Instance);
			RegisterDataProvider (NameConfigProvider.Instance);
            RegisterDataProvider (UIWindowConfigProvider.Instance);
			RegisterDataProvider (MapConfigProvider.Instance);
            RegisterDataProvider (GameVariableConfigProvider.Instance);
            RegisterDataProvider (CampConfigConfigProvider.Instance);
            RegisterDataProvider (AIStrategyConfigProvider.Instance);
            RegisterDataProvider (TaskConfigProvider.Instance);

            // 加载
            if (!Load()) return false;

			LoggerSystem.Instance.Debug("DataProviderSystem   init   end");
            return true;
        }

		public void Tick(float interval)
        {

        }

        public void Destroy()
        {
            mDataProvider.Clear();
        }

        private bool Load()
        {
            IDataProvider provider = null;
            for (int i = 0; i < mDataProvider.Count; ++i)
            {
                provider = mDataProvider[i];
                if (null != provider)
                {
                    if( !provider.IsXML() )
					    FileReader.LoadPath(AssetManager.Get().LoadStramingAsset(provider.Path()));
                    provider.Load();
                    if (!provider.Verify()) return false;

                    if (!provider.IsXML())
                        FileReader.UnLoad();
                }
            }

            return true;
        }


        /// <summary>
        /// 语言加载单独那出来
        /// </summary>
        public bool LoadLanguage()
        {
            if (languageProvider != null)
            {
                int local = LocalAccountStorage.Get().localLanguage;
                if( local <= 0)
                {
                    SystemLanguage language = Application.systemLanguage;
                    switch (language)
                    {
                        case SystemLanguage.Chinese:
                        case SystemLanguage.ChineseSimplified:
                        case SystemLanguage.ChineseTraditional:
                            language = SystemLanguage.Chinese;
                            break;
                        default:
                            break;
                    }
                    LocalAccountStorage.Get().localLanguage = (int)language;
                    languageProvider.LoadLanguage(language);
                }
                else
                {
                    if( local == (int)SystemLanguage.ChineseSimplified || local == (int)SystemLanguage.ChineseTraditional)
                    {
                        LocalAccountStorage.Get().localLanguage = (int)SystemLanguage.Chinese;
                    }
                    languageProvider.LoadLanguage((SystemLanguage)local);
                }
            }
                
            return true;
        }

        private void RegisterDataProvider(IDataProvider dataProvider)
        {
            mDataProvider.Add(dataProvider);
        }

		public static string FormatDataProviderPath(string datapath)
        {
            return System.IO.Path.Combine(UtilTools.GetStreamAssetsByPlatform(), datapath);
		}
		public void LoadExtraData (string path)
		{
			IDataProvider provider = null;
			for (int i = 0; i < mDataProvider.Count; ++i)
			{
				provider = mDataProvider[i];
				if (null != provider)
				{
					FileReader.LoadPath(System.IO.Path.Combine(path, provider.Path()));

					provider.LoadExtraData ();

					provider.Verify();

					FileReader.UnLoad();
				}
			}
		}
    }
}
