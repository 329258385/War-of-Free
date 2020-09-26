﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Solarmax
{
	public class EngineSystem : Singleton<EngineSystem>, Lifecycle
	{
        /// <summary>
        /// 网络状态检测时间
        /// </summary>
        private const float NET_STATUS_CHECK_INTERVAL = 1.0f;

        private int mFPS = 30;
        private float checkDelta = 0;
        
        public delegate void OnNetStatusChanged(NetworkReachability status);
        public OnNetStatusChanged onNetStatusChanged;

        public bool Init()
		{
			LoggerSystem.Instance.Debug("EngineSystem    init  begin");
			string val = string.Empty;
			if (ConfigSystem.Instance.TryGetConfig("fps", out val))
			{
				SetFPS(Converter.ConvertNumber<int>(val));
			}

			// background
			Application.runInBackground = true;
			// fps
			Application.targetFrameRate = GetFPS();
			// screen lock
			Screen.sleepTimeout = SleepTimeout.NeverSleep;

			// input mode
			Input.multiTouchEnabled = true;

			LoggerSystem.Instance.Debug("EngineSystem    init  end");
			return true;
		}

		public void Tick(float interval)
		{
            if (onNetStatusChanged != null) {
                if (checkDelta < NET_STATUS_CHECK_INTERVAL) {
                    checkDelta += interval;
                } else {
                    checkDelta = 0;
                    onNetStatusChanged((NetworkReachability)GetNetworkRechability());
                }
            }
		}

		public void Destroy()
		{
		
		}

		private void SetFPS(int fps)
		{
			if (fps > 0)
			{
				mFPS = fps;
			}
		}
		public int GetFPS()
		{
			return mFPS;
		}

		/// <summary>
		/// 获得网络连通性
		/// 0无网络；1运营商；2wifi
		/// </summary>
		/// <returns>The net rechability.</returns>
		public int GetNetworkRechability()
		{
#if !SERVER
			NetworkReachability reach = Application.internetReachability;
#else
			NetworkReachability reach = NetworkReachability.ReachableViaLocalAreaNetwork;
#endif
			int ret = 0;
			if (reach == NetworkReachability.NotReachable)
				ret = 0;
			else if (reach == NetworkReachability.ReachableViaCarrierDataNetwork)
				ret = 1;
			else if (reach == NetworkReachability.ReachableViaLocalAreaNetwork)
				ret = 2;

			return ret;
		}

		public string GetOS ()
		{
			return SystemInfo.operatingSystem;
		}

		public string GetUUID ()
		{
			return SystemInfo.deviceUniqueIdentifier;
		}

		public string GetDeviceModel ()
		{
			return SystemInfo.deviceModel;
		}

        const float devWidth     = 1920.0f; // 设计的尺寸高度
        const float devHeight    = 1080.0f; // 设计的尺寸宽度
        void AdjustGraphicSize()
        {
            float orthographicSize  = Camera.main.orthographicSize;
            float aspectRatic       = Screen.width * 1.0f / Screen.height;

            if (Screen.width > devWidth)
            {
                orthographicSize    = Mathf.RoundToInt(orthographicSize * (aspectRatic) + 0.5f); // 基准值 * 倍宽高比 = 相机的大小
                Camera.main.orthographicSize = orthographicSize;
            }
        }
    }
}
