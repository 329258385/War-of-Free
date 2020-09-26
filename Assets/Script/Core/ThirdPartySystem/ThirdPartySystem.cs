using System;
using UnityEngine;




namespace Solarmax
{
    public class ThirdPartySystem : Singleton<ThirdPartySystem>, Lifecycle
    {
        /**
         * We will use this system to do sdk method, every call will be collected to this obj.
         * 
         * */
        public ThirdPartySystem()
        {

        }

        public bool Init()
        {

            return true;
        }

        public void Tick(float interval)
        {

        }

        public void Destroy()
		{
			
        }

		/// <summary>
		/// 使用此接口调用第三方登录
		/// </summary>
		public void Login()
		{
            if( string.IsNullOrEmpty(LocalAccountStorage.Get().account) )
            {
                DateTime start = new DateTime(1970, 1, 1);
                long timeStamp = (long)(DateTime.Now - start).TotalSeconds;
                LocalAccountStorage.Get().account       = SystemInfo.deviceUniqueIdentifier;
                LocalAccountStorage.Get().regtimeStamp  = timeStamp;
                LocalStorageSystem.Get().SaveLocalAccount();
                Flurry.Instance.FlurryLoginEvent("Login", "0");
            }
            else
            {
                Flurry.Instance.FlurryLoginEvent("Login", "1");
            }
			OnSDKLogin (LocalAccountStorage.Get().account);
        }


        /// <summary>
        /// 第三方sdk登录结果回调
        /// </summary>
        public void OnSDKLogin(string account)
		{
			LocalAccountStorage.Get().account = account;
			LocalStorageSystem.Get ().NeedSaveToDisk ();
			EventSystem.Instance.FireEvent (EventId.OnSDKLoginResult, account);
		}

		/// <summary>
		/// 第三方sdk切换账号回调
		/// </summary>
		public void OnSDKSwitch(string msg)
		{

		}

		/// <summary>
		/// 己方服务注册回调
		/// </summary>
		public void OnRegister ()
		{

		}

		/// <summary>
		/// 己方服务登录回调
		/// </summary>
		public void OnLogin ()
		{

		}

		/// <summary>
		/// 开始单机关卡
		/// </summary>
		public void OnStartPve (string mapId)
		{
			if (string.IsNullOrEmpty (mapId))
				return;
		}

		/// <summary>
		/// 结束单机关卡
		/// </summary>
		public void OnFinishPve (string mapId)
		{
			if (string.IsNullOrEmpty (mapId))
				return;

		}

		/// <summary>
		/// 失败单机关卡
		/// </summary>
		public void OnFailPve (string mapId)
		{
			if (string.IsNullOrEmpty (mapId))
				return;
		}

		/// <summary>
		/// 获取渠道
		/// </summary>
		public string GetChannel ()
		{
			string channel = string.Empty;
			channel = "Unity_Editor";
			return channel;
		}

		/// <summary>
		/// 检查当前奖杯状态
		/// </summary>
		public void CheckMedal ()
		{

		}

		/// <summary>
		/// 检查普通战斗奖牌
		/// </summary>
		public void CheckCommonFightMedal (bool mvp, bool successed)
		{

		}
    }
}
