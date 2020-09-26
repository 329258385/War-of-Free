/*
 * Name: UpdateSystem
 * Function: 提供游戏数据更新功能
 * Author: FangJun
 * Date: 2016-06-23
 * Framework: 
 *          主要提供游戏中数据更新，分两步：
 * 				1，整包更新优先，如果有整包更新，则直接弹出，并不更新之间的小包
 * 				2，StreamingAsstes中的配置文件，诸如txt,lua，这种数据流文件，自己解析的这些
 * 				3，Prefab文件，需要用AB更新。
 * 
 * 保存的文件：
 *			主要有filelist.txt、AssetBundles、assets/xxx.ab
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Solarmax
{
    [Serializable]
	public class TableVersion
	{
		/// <summary>
		/// 版本号
		/// </summary>
		public string version;
		/// <summary>
		/// 是否整包更新
		/// </summary>
		public bool isPackage;
		/// <summary>
		/// 资源表的路径
		/// </summary>
		public string assetTableUrl;
		/// <summary>
		/// assetbundle依赖文件
		/// </summary>
		public string assetBundlesFileUrl;
		/// <summary>
		/// 资源大小 B
		/// </summary>
		public int assetDataSize;
		/// <summary>
		/// 资源路径前缀
		/// </summary>
		public string assetUrlHead;
		/// <summary>
		/// 描述
		/// </summary>
		public string desc;
	}

    [Serializable]
	public class TableAsset
	{
		/// <summary>
		/// 资源路径名
		/// </summary>
		public string assetPath;
		/// <summary>
		/// ab包路径
		/// </summary>
		public string assetBundlePath;
	}

	public class UpdateSystem : Singleton<UpdateSystem>, Lifecycle
	{
		private const string AppVersionMark = "_AppVersion_";
		private const string AssetBundleMark = "_AssetBundleMark_";
		private string appVersion;
		private Dictionary<System.Object, TableAsset> localAssets;
		private AssetBundleManifest assetBundleManifest;
		private bool haveAssetBundle = false;

		public string serverUrl;
		public string saveRoot;
        public string saveVideo;
		public string streamAssetsRoot;

		private Dictionary<System.Object, TableVersion> allVersions;
		private List<TableVersion> waitVersions;
		private Dictionary<System.Object, TableAsset> waitAssets;


		public bool Init()
		{
			LoggerSystem.Instance.Debug ("UpdateSystem    init  begin");
			
			//serverUrl = "http://192.168.1.106:8080/examples/";
			serverUrl = string.Empty;
			if (!ConfigSystem.Instance.TryGetConfig("UpdateServer", out serverUrl))
			{
				serverUrl = "http://120.92.142.56/";
			}

			if (Application.platform == RuntimePlatform.WindowsEditor) {
				saveRoot = Application.dataPath + "/cache";
                saveVideo = Application.dataPath + "/video/";
                streamAssetsRoot = "file://" + Application.streamingAssetsPath;
			} else if (Application.platform == RuntimePlatform.OSXEditor) {
				saveRoot = Application.persistentDataPath;
                saveVideo = Application.persistentDataPath + "/video/";
                streamAssetsRoot = "file://" + Application.streamingAssetsPath;
			} else if (Application.platform == RuntimePlatform.Android) {
				saveRoot    = Application.persistentDataPath;
                saveVideo   = Application.persistentDataPath + "/video/";
                streamAssetsRoot = Application.streamingAssetsPath;
			} else if (Application.platform == RuntimePlatform.IPhonePlayer) {
				saveRoot    = Application.persistentDataPath;
                saveVideo   = Application.persistentDataPath + "/video/";
                streamAssetsRoot = "file://" + Application.streamingAssetsPath;
			}
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                saveRoot = Application.dataPath + "/cache";
                saveVideo = Application.dataPath + "/video/";
                streamAssetsRoot = "file://" + Application.streamingAssetsPath;
            }

			if (!saveRoot.EndsWith("/"))
				saveRoot += "/";
			if (!streamAssetsRoot.EndsWith("/"))
				streamAssetsRoot += "/";

			LoadLocalAssetTable ();

			InitVersion ();

			LoggerSystem.Instance.Debug ("UpdateSystem    init  end");

			return true;
		}

		public void Tick(float interval)
		{
		
		}

		public void Destroy()
		{
			LoggerSystem.Instance.Debug ("UpdateSystem    destroy  begin");
		}

		private void InitVersion()
		{
			// 上次版本，更新后的
			string lastVersion = PlayerPrefs.GetString(AppVersionMark);

			// 包版本
			string packageVersion = string.Empty;
			ConfigSystem.Instance.TryGetConfig ("version", out packageVersion);

			if (string.IsNullOrEmpty(packageVersion)) {
				packageVersion = "0.0.0.0";
			}

			// 如果lastversion新于packageversion，则认为本地为lastversion版本
			if (CheckVersion (lastVersion, packageVersion)) {
				appVersion = lastVersion;
				// 重新设置assetbundle标志
				haveAssetBundle = PlayerPrefs.GetInt (AssetBundleMark) > 0;
				if (haveAssetBundle) {
					// 重新加载配置表
					//TableMagr.Get().Initialize();
					DataProviderSystem.Instance.Destroy ();
					DataProviderSystem.Instance.Init ();
				}
			} else {
				appVersion = packageVersion;

				// 如果包版本高于本地版本，则清除本地保存的数据
				ClearUpdateData ();
			}

			LoggerSystem.Instance.Info ("包版本：{0}，缓存版本：{1}，是否使用AB：{2}", packageVersion, lastVersion, haveAssetBundle);
		}

		/// <summary>
		/// 检查是否需要升级, true需要升级
		/// </summar>
		private bool CheckVersion(string ver1, string ver2)
		{
			int v1 = CalCode (ver1.ToLower ());
			int v2 = CalCode (ver2.ToLower ());

			return v1 > v2;
		}

		private int CalCode(string str)
		{
			int ret = 0;
			string [] all = str.Split('.');
			if (all.Length == 4) {
				str = str.Substring (0, str.LastIndexOf ('.'));
			}

			char[] array = str.ToCharArray ();
			int up = 1;
			for (int i = array.Length - 1; i >= 0; --i) {
				var c = array [i];
				if (c == '.')
					continue;
				ret += (c - '0') * up;
				up *= 10;
			}

			return ret;
		}

		private void LoadLocalAssetTable ()
		{
			// 加载本地tableasset
			if (System.IO.File.Exists (saveRoot + "filelist.txt")) {
                using (FileStream stream = new FileStream(saveRoot + "filelist.txt", FileMode.Open))
                {
                    byte[] bytes    = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);
                    localAssets     = Json.DeCode<Dictionary<System.Object, TableAsset>>(bytes);
                }
			}

			if (localAssets == null){
				localAssets = new Dictionary<object, TableAsset> ();
			}
		}

		private void SaveLocalAssetTable()
		{
            byte[] bs = Json.EnCodeBytes (localAssets);
			SaveFile (saveRoot + "filelist.txt", bs);
		}

		private void SaveFile(string path, byte[] bytes)
		{
			Debug.Log ("写入文件:" + path);

			int pos         = path.LastIndexOf ('/');
			string folder   = path.Substring (0, pos);
			if (!System.IO.Directory.Exists (folder)) {
				System.IO.Directory.CreateDirectory (folder);
			}

			System.IO.FileStream fs = new System.IO.FileStream (path, System.IO.FileMode.Create);
			fs.Write (bytes, 0, bytes.Length);
			fs.Flush ();
			fs.Close ();
			fs.Dispose ();
		}

		/// <summary>
		/// 清除本地更新数据
		/// </summary>
		private void ClearUpdateData ()
		{
			// 删除文件，删除saveRoot目录的filelist.txt，AssetBundles，assets/文件
			string filePath = saveRoot;
			if (Directory.Exists (filePath)) {
				try
				{
					// 文件列表文件
					filePath = saveRoot + "filelist.txt";
					if (File.Exists (filePath))
					{
						File.Delete (filePath);
					}
					// AssetBundles文件
					filePath = saveRoot + "AssetBundles";
					if (File.Exists (filePath))
					{
						File.Delete (filePath);
					}
					// 详细资源文件
					filePath = saveRoot + "assets/";
					if (Directory.Exists (filePath))
					{
						DirectoryInfo dir = new DirectoryInfo (filePath);
						dir.Delete (true);
					}
				}
				catch (Exception e) {
					LoggerSystem.Instance.Error ("UpdateSystem ClearUpdateData cause exception:" + e.Message);
				}
			}

			// 清除标记
			PlayerPrefs.SetString (AppVersionMark, "");
			PlayerPrefs.SetInt (AssetBundleMark, 0);
		}

		/// <summary>
		/// 获取客户端版本
		/// </summary>
		/// <returns>The app version.</returns>
		public string GetAppVersion()
		{
			return appVersion;
		}

		public void SetAppVersion(string ver)
		{
			LoggerSystem.Instance.Info ("更新本地版本至：{0}", ver);
			appVersion = ver;
			PlayerPrefs.SetString (AppVersionMark, ver);
		}

		public bool HaveAssetBundle()
		{
			return haveAssetBundle;
		}

		public void SetAssetBundle(bool status)
		{
			if (status && !HaveAssetBundle()) {
				haveAssetBundle = true;
				PlayerPrefs.SetInt (AssetBundleMark, 1);
			}
		}

		/// <summary>
		/// 下载版本文件表格
		/// </summary>
		private IEnumerator DownloadVersions(string url)
		{
			using (WWW www = new WWW (url)) {
				while (!www.isDone) {
					EventSystem.Instance.FireEvent (EventId.OnABDownloading, www.progress, www.bytesDownloaded);
					yield return 1;
				}

				try
				{
					allVersions = Json.DeCode<Dictionary<System.Object, TableVersion>> (System.Text.Encoding.UTF8.GetBytes(www.text));
				}
				catch (Exception e) {
					Debug.LogErrorFormat ("DownloadVersions: {0}", e.Message);
					allVersions = null;
				}
			}

		}

		/// <summary>
		/// 下载版本中涉及到的资源表格
		/// </summary>
		private IEnumerator DownloadAssets(string url)
		{
			using (WWW www = new WWW (url)) {
				while (!www.isDone) {
					EventSystem.Instance.FireEvent (EventId.OnABDownloading, www.progress, www.bytesDownloaded);
					yield return 1;
				}

				try
				{
					waitAssets = Json.DeCode<Dictionary<System.Object, TableAsset>> (System.Text.Encoding.UTF8.GetBytes(www.text));
				}
				catch (Exception e) {
					Debug.LogErrorFormat ("DownloadAssets: {0}", e.Message);
					waitAssets = null;
				}
			}

		}

		/// <summary>
		/// 下载bytes文件，并存储
		/// </summary>
		private IEnumerator DownloadBytes(string url, string savePath)
		{
			using (WWW www = new WWW (url)) {
				while (!www.isDone) {
					EventSystem.Instance.FireEvent (EventId.OnABDownloading, www.progress, www.bytesDownloaded);
					yield return 1;
				}

				if (string.IsNullOrEmpty (www.error)) {
					SaveFile (savePath, www.bytes);
				} else {
					Debug.LogErrorFormat ("DownloadBytes: {0}", www.error);
				}

			}
		}

		/// <summary>
		/// 请求更新数据
		/// </summary>
		/// <returns>The update.</returns>
		public IEnumerator RequestVersion()
		{
			yield return DownloadVersions (serverUrl + "Version.txt");

			if (allVersions == null) {
				EventSystem.Instance.FireEvent (EventId.OnABGetVersions, 0);
			} else {
				// 判断需要多少个版本需要更新
				waitVersions = new List<TableVersion>();
				foreach (var v in allVersions.Values) {
					LoggerSystem.Instance.Info ("检查版本：{0}， 本地版本{1}", v.version, appVersion);
					if (CheckVersion (v.version, appVersion)) {
						waitVersions.Add (v);
					}
				}

				// 检查待更新队列中是否有整包更新的版本，如果有，则检测为整包更新模式
				TableVersion packageUpdateVersion = null;
				for (int i = waitVersions.Count - 1; i >=0; --i) {
					if (waitVersions [i].isPackage) {
						packageUpdateVersion = waitVersions [i];
						break;
					}
				}
				if (packageUpdateVersion != null) {
					// 提示有整包更新，则优先上应用商店更新
					EventSystem.Instance.FireEvent (EventId.OnPackageUpdate, packageUpdateVersion);
				} else {
					EventSystem.Instance.FireEvent (EventId.OnABGetVersions, waitVersions.Count);
				}
			}
		}

		/// <summary>
		/// 开始更新
		/// 小版本资源更新
		/// </summary>
		/// <returns>The update.</returns>
		public IEnumerator StartUpdate()
		{
			yield return new WaitForSeconds (1.0f);

			if (waitVersions == null || waitVersions.Count == 0)
				yield return 0;

			bool haveDownload = false;

			for (int i = 0; i < waitVersions.Count; ++i) {
				var v = waitVersions [i];
				LoggerSystem.Instance.Info ("开始下载版本版本：{0}", v.version);
				EventSystem.Instance.FireEvent (EventId.OnABDownloadIndex, i, v.assetDataSize);

				//需要下载
				//先下载assetbundle依赖文件
				yield return DownloadBytes (v.assetBundlesFileUrl, saveRoot + "AssetBundles");

				//下载assetTable就好了
				yield return DownloadAssets (v.assetTableUrl);

				if (waitAssets == null)
					continue;

				//根据assettable下载所有的ab包
				List<string> allABs = new List<string> ();
				foreach (var a in waitAssets.Values) {
					if (!allABs.Contains (a.assetBundlePath))
						allABs.Add (a.assetBundlePath);
				}
				foreach(var ab in allABs) {
					yield return DownloadBytes (v.assetUrlHead + ab, saveRoot + ab);
				}

				//合并进入本地tableAsset
				foreach (var ta in waitAssets) {
					if (localAssets.ContainsKey (ta.Key))
						localAssets [ta.Key] = ta.Value;
					else
						localAssets.Add (ta.Key, ta.Value);
				}

				SetAssetBundle (true);

				//更新version
				SetAppVersion (v.version);

				haveDownload = true;
			}

			if (haveDownload) {
				SaveLocalAssetTable ();
			}

			EventSystem.Instance.FireEvent (EventId.OnABDownloadingFinished);
		}

		/// <summary>
		/// 重新加载client配置
		/// </summary>
		public void ReloadClient()
		{
			//TableMagr.Get().Initialize();
			DataProviderSystem.Instance.Destroy ();
			DataProviderSystem.Instance.Init ();

			LocalStorageSystem.Get ().Init ();
		}

		public IEnumerator AssetUpdate()
		{
			// 首先获取服务器version文件
			yield return DownloadVersions (serverUrl + "Version.txt");

			if (allVersions != null) {
				bool haveDownload = false;
				foreach (var v in allVersions.Values) {
					LoggerSystem.Instance.Info ("检查版本：{0}， 本地版本{1}", v.version, appVersion);
					if (CheckVersion (v.version, appVersion)) {
						LoggerSystem.Instance.Info ("开始下载版本版本：{0}", v.version);
						//需要下载
						//先下载assetbundle依赖文件
						yield return DownloadBytes (v.assetBundlesFileUrl, saveRoot + "AssetBundles");

						//下载assetTable就好了
						yield return DownloadAssets (v.assetTableUrl);

						//根据assettable下载所有的ab包
						List<string> allABs = new List<string> ();
						foreach (var a in waitAssets.Values) {
							if (!allABs.Contains (a.assetBundlePath))
								allABs.Add (a.assetBundlePath);
						}
						foreach (var ab in allABs) {
							yield return DownloadBytes (v.assetUrlHead + ab, saveRoot + ab);
						}

						//合并进入本地tableAsset
						foreach (var ta in waitAssets) {
							if (localAssets.ContainsKey (ta.Key))
								localAssets [ta.Key] = ta.Value;
							else
								localAssets.Add (ta.Key, ta.Value);
						}

						SetAssetBundle (true);

						//更新version
						SetAppVersion (v.version);

						haveDownload = true;
					}
				}

				if (haveDownload) {
					SaveLocalAssetTable ();
				}
			}

		}

		/// <summary>
		/// 请求离线公告
		/// 离线公告的格式；"type;text"，type:0普通，1停服不再登录
		/// </summary>
		/// <returns>The version.</returns>
		public IEnumerator RequestHttpNotice()
		{
			string url            = string.Format ("{0}Notice/Notice.txt", serverUrl);
			string notice         = string.Empty;
            using (WWW www = new WWW(url))
            {
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
                    notice = www.text;
                }
            }
			// 公告发送到界面
			EventSystem.Instance.FireEvent (EventId.OnHttpNotice, notice);
		}

		public IEnumerator LoadAssetAsync(string path, List<object> assetList)
		{
			if (assetBundleManifest == null) {
				var req = AssetBundle.LoadFromFileAsync (saveRoot + "AssetBundles");
				while (!req.isDone)
					yield return 1;
				var abr = req.assetBundle.LoadAssetAsync<AssetBundleManifest> ("AssetBundleManifest");
				while (!abr.isDone)
					yield return 1;
				assetBundleManifest = abr.asset as AssetBundleManifest;
				req.assetBundle.Unload (false);
			}

			string fullpath = "Assets/Resources/" + path;

			if (localAssets.ContainsKey (fullpath)) {
				var la = localAssets [fullpath];
				// 获取依赖项
				string[] depends = assetBundleManifest.GetAllDependencies(fullpath);
				AssetBundle[] dependAssetBundles = new AssetBundle[depends.Length];
				//  加载依赖项
				for (int i = 0; i < depends.Length; ++i) {
					var req = AssetBundle.LoadFromFileAsync (saveRoot + depends);
					while (!req.isDone)
						yield return 1;
					dependAssetBundles [i] = req.assetBundle;
				}
				var r = AssetBundle.LoadFromFileAsync (saveRoot + la.assetBundlePath);
				while (!r.isDone)
					yield return 1;
				var nowBundle = r.assetBundle;
				var l = nowBundle.LoadAssetAsync (fullpath);
				while (!l.isDone)
					yield return 1;
				assetList.Add (l.asset);

			}
		}

		public bool LoadAsset(string path, out object asset)
		{
			if (!HaveAssetBundle () || localAssets.Count == 0) {
				asset = null;
				return false;
			}

			if (assetBundleManifest == null) {
				var manifestBundle = AssetBundle.LoadFromFile (saveRoot + "AssetBundles");
				if (manifestBundle == null) {
					asset = null;
					return false;
				}

				assetBundleManifest = manifestBundle.LoadAsset<AssetBundleManifest> ("AssetBundleManifest");
				manifestBundle.Unload (false);
			}

			asset = null;

			if (string.IsNullOrEmpty (path)) {
				return false;
			}

			string fullpath = "Assets/Resources/" + path;

			if (localAssets.ContainsKey (fullpath)) {
				var la = localAssets [fullpath];
				// 获取依赖项
				string[] depends = assetBundleManifest.GetAllDependencies(fullpath);
				AssetBundle[] dependAssetBundles = new AssetBundle[depends.Length];
				//  加载依赖项
				for (int i = 0; i < depends.Length; ++i) {
					dependAssetBundles [i] = AssetBundle.LoadFromFile (saveRoot + depends [i]);
				}
				var nowBundle = AssetBundle.LoadFromFile (saveRoot + la.assetBundlePath);

				if (nowBundle == null) {
					return false;
				}

				int pos = path.LastIndexOf ('/');
				string realName = path.Substring (pos + 1);
				asset = nowBundle.LoadAsset (realName);
				nowBundle.Unload (false);

				for (int i = 0; i < dependAssetBundles.Length; ++i) {
					dependAssetBundles [i].Unload (false);
				}

				return true;
			}

			return false;
		}

		public bool LoadStreamingAssets (string path, out object asset)
		{
			asset = null;
			string fullpath = UtilTools.GetStreamAssetsByPlatform( "/" + path);
			if (localAssets.ContainsKey (fullpath)) {

				var la = localAssets [fullpath];
				asset = saveRoot + la.assetBundlePath;

				return true;
			}

			return false;
		}

		public void DownloadEditMapInfo ()
		{
            #if UINTY_EDITORMAP_SERVER
            string saveFolder = saveRoot + "EditMap/";
			if (!System.IO.Directory.Exists (saveFolder))
            {
				System.IO.Directory.CreateDirectory (saveFolder);
			}
			try
			{
                NetSystem.Instance.helper.GenPresignedUrl("MapList.xml", "GET", "text/plain", saveFolder + "MapList.xml", (int)EventId.OnGenPresignedUrlMaplist);
            }
			catch (Exception e)
            {
				Debug.LogError (e.Message);
				Debug.LogError (e.StackTrace);
			}
			#endif

		}
	}
}

