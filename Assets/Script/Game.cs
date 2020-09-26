using UnityEngine;
using System;
using Solarmax;
using System.Collections;
using System.Collections.Generic;




public class Game : MonoBehaviour
{
	public GameObject battleRoot;

	// 进入pause时间
	private float appPauseBeginTime;

	public bool initFinished = false;
    private int mPressTimes = 0;
    public static Game game;

	void Awake()
	{
		// 文件位置
		if (Application.isEditor)
		{
			Framework.Instance.SetWritableRootDir(Application.temporaryCachePath);
		}
		else
		{
			Framework.Instance.SetWritableRootDir(Application.temporaryCachePath);
		}

		// console输出
		LoggerSystem.Instance.SetConsoleLogger(new Solarmax.Logger(UnityEngine.Debug.Log));

		initFinished = false;

		game = this;
	}

    
    void Start ()
	{
        UISystem.DirectShowPrefab ("UI/SplashWindow");
	}

	public void Init ()
	{
		// 初始化
		if (Framework.Instance.Init())
		{
			LoggerSystem.Instance.Info("系统启动！");
		}
		else
		{
			LoggerSystem.Instance.Error("系统启动失败！");
		}

		AsyncInitMsg ();

		AudioManger.Get ().Init ();
		BattleSystem.Instance.battleData.root = battleRoot;

        /// 出示数据统计
        Flurry.Instance.FlurryInit();
        Flurry.Instance.LogUserID(LocalAccountStorage.Get().account);
        
        initFinished = true;
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            mPressTimes++;
            StartCoroutine("ResetMPressTimes", 1.0f);//若过了1秒都没有按第2次则重置mPressTimes
            if (mPressTimes == 2)
            {
                DateTime start = new DateTime(1970, 1, 1);
                long timeStamp = (long)(DateTime.Now - start).TotalSeconds;
                timeStamp = timeStamp - (long)LocalPlayer.Get().PowerRefreshTime;
                LocalAccountStorage.Get().timeStamp = timeStamp;
                LocalStorageSystem.Instance.NeedSaveToDisk();
                LocalStorageSystem.Instance.SaveStorage();
                Flurry.Instance.FlurryLoginEvent("LogOut", "0");
                Flurry.Instance.EndSession();
                Application.Quit();
            }
        }
    }

    IEnumerator ResetMPressTimes(float sec)
    {
        yield return new WaitForSeconds(sec);
        mPressTimes = 0;
    }


    /// <summary>
    /// 异步加载msg，否则在第一次使用message时会延时很高
    /// </summary>
    private void AsyncInitMsg()
	{
		AsyncThread at = new AsyncThread ((arg) => {
			System.Threading.Thread.Sleep (100);
			//NetMessage.Msg.RegisterAllExtensions (null);
		});
		at.Start ();

//		System.Threading.Thread t = new System.Threading.Thread (() => {
//			NetMessage.Msg.RegisterAllExtensions (null);	
//		});
	}

	void FixedUpdate()
	{
		if (initFinished) {
			Framework.Instance.Tick(Time.deltaTime);
		}
	}

    /// <summary>
    /// android 程序停止不保证调到此接口
    /// 官方说明链接： https://answers.unity.com/questions/1248489/onapplicationquit-function-is-not-working-with-uni.html
    /// </summary>
	void OnDestroy() 
	{
 
    }

    /// <summary>
    /// android 程序停止不保证调到此接口
    /// </summary>
    void OnApplicationQuit()
    {

    }

    /// <summary>
    /// 应用进入暂停
    /// </summary>
    /// <param name="pauseStatus">If set to <c>true</c> pause status.</param>
    void OnApplicationPause(bool pauseStatus)
	{

		if (!initFinished)
			return;

        if( !pauseStatus )
        {
            // 处理离线时间,暂时不考虑时间作弊的情况,策划要求
            //Debug.Log("Unity : enter server front!!!!");
            //long timeStamp  = LocalAccountStorage.Get().timeStamp;
            //DateTime Start  = new DateTime(1970, 1, 1);
            //Start           = Start.AddSeconds(timeStamp);
            //TimeSpan ts     = DateTime.Now - Start;
            //LocalPlayer.Get().OperatorOffLine((int)ts.TotalSeconds);
            
        }
        else
        {
            /// 离线保存 
            Debug.Log("Unity : enter server back!!!!");
            DateTime start      = new DateTime(1970, 1, 1);
            long timeStamp      = (long)(DateTime.Now - start).TotalSeconds;
            timeStamp           = timeStamp - (long)LocalPlayer.Get().PowerRefreshTime;
            LocalAccountStorage.Get().timeStamp = timeStamp;
            LocalStorageSystem.Instance.NeedSaveToDisk();
            LocalStorageSystem.Instance.SaveStorage();
            
            // 需要对断线进行处理
            if (pauseStatus)
            {
                appPauseBeginTime = Time.realtimeSinceStartup;
            }
            else
            {
                // pvp模式时才重连，单机不管
                GameType gt = BattleSystem.Instance.battleData.gameType;
                if (gt == GameType.PVP || gt == GameType.League)
                {

                    if (Time.realtimeSinceStartup - appPauseBeginTime >= 10)
                    {
                        // 主动断开连接
                        if (NetSystem.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTED)
                        {

                            Debug.Log("在后台超过10s，主动断开连接");
                            NetSystem.Instance.Close();
                        }
                        else
                        {
                            // 先屏蔽掉
                            //NetSystem.Instance.DisConnectedCallback ();
                        }
                    }
                }
            }
        }
        
	}
}
