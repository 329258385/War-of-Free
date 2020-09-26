using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Solarmax;





public class LogoWindow : BaseWindow
{
    public GameObject progressGo = null;
    public UILabel progressLabel = null;
    public UILabel loadingTip = null;
    public UISprite foreSprite = null;

    private int progressChangeTimes = 0;
    private float progressChangeValue = 0;
    private float progressValue = 0;


    private float targetProgress;
    private float progressInterval;

    private static bool hasInit = false;

    void Awake()
    {
        progressChangeTimes = 100;
        progressChangeValue = 1.0f / progressChangeTimes;
        progressGo.SetActive(true);
    }

    void Start()
    {
        //progressBar.value = 0;
        foreSprite.fillAmount = 0;
        InvokeRepeating("UpdateProgress", progressChangeValue, progressChangeValue);
        Invoke("DelayStart", 0.5f);
    }

    private void DelayStart()
    {
        if (!hasInit)
        {
            Game.game.Init();

            UISystem.Instance.RegistWindow("LogoWindow", this);
            hasInit = true;
        }

        RequestOfflineNotice();
        //LoginSDK();
    }

    public override bool Init()
    {
        base.Init();
        RegisterEvent(EventId.RequestUserResult);
        RegisterEvent(EventId.CreateUserResult);
        RegisterEvent(EventId.OnABGetVersions);
        RegisterEvent(EventId.OnABDownloadIndex);
        RegisterEvent(EventId.OnABDownloading);
        RegisterEvent(EventId.OnABDownloadingFinished);
        RegisterEvent(EventId.OnPackageUpdate);
        RegisterEvent(EventId.OnHttpNotice);
        RegisterEvent(EventId.OnSDKLoginResult);
        RegisterEvent(EventId.UpdateChaptersView);
        RegisterEvent(EventId.OnSyncuserAndChaptersResult);
        return true;
    }

    public override void OnShow()
    {
        //AudioManger.Get ().PlayAudioBG ("Empty");
        base.OnShow();

    }

    public override void OnHide()
    {

    }

    public override void OnUIEventHandler(EventId eventId, params object[] args)
    {
        if (eventId == EventId.RequestUserResult)
        {
            // 请求玩家数据结果
            NetMessage.ErrCode code = (NetMessage.ErrCode)args[0];
            if (code == NetMessage.ErrCode.EC_Ok)
            {
                RequestAccountAndChapters();
            }
            else if (code == NetMessage.ErrCode.EC_NoExist)
            {
                // 默认玩家
                CreateDefaultUser();
            }
            else if (code == NetMessage.ErrCode.EC_NeedResume)
            {
                // 需要恢复
                UISystem.Get().ShowWindow("CommonDialogWindow");
                EventSystem.Instance.FireEvent(EventId.OnCommonDialog, 1, LanguageDataProvider.GetValue(20));
                NetSystem.Instance.helper.ReconnectResume(); // 此时根据结果来判断是走哪种恢复
                //EventSystem.Instance.FireEvent(EventId.OnCommonDialog, 2, LanguageDataProvider.GetValue(20),
                //                                                          new EventDelegate(ReconnectResume),
                //                                                          new EventDelegate(ReconnectGiveup));
            }
        }
        else if (eventId == EventId.CreateUserResult)
        {
            // 创建玩家结果
            NetMessage.ErrCode code = (NetMessage.ErrCode)args[0];
            if (code == NetMessage.ErrCode.EC_Ok)
            {
                RequestAccountAndChapters();
            }
            else if (code == NetMessage.ErrCode.EC_NameExist)
            {
                Tips.Make( LanguageDataProvider.GetValue(4));
            }
            else if (code == NetMessage.ErrCode.EC_InvalidName)
            {
                Tips.Make(LanguageDataProvider.GetValue(5));
            }
            else if (code == NetMessage.ErrCode.EC_AccountExist)
            {
                Tips.Make(LanguageDataProvider.GetValue(6));
            }
            else
            {
                Tips.Make(LanguageDataProvider.GetValue(7));
            }
        }
        else if (eventId == EventId.OnABGetVersions)
        {
            int versionCount = (int)args[0];
            loadingTip.text += "";
            if (versionCount == 0)
            {
                SetProgress(0.6f);
                RequestOfflineNotice();
            }
            else
            {
                loadingTip.text += string.Format(LanguageDataProvider.GetValue(305), versionCount);
                progressInterval = 0.5f / versionCount;
                SetProgress(0.1f);
                Coroutine.Start(UpdateSystem.Get().StartUpdate());
            }
        }
        else if (eventId == EventId.OnPackageUpdate)
        {
            // 整包更新
            TableVersion packageUpdateVersion = args[0] as TableVersion;
            // 弹出大版本更新提示框
            string desc = packageUpdateVersion.desc.Replace("\\n", "\n");
            UISystem.Instance.ShowWindow("CommonDialogWindow");
            EventSystem.Instance.FireEvent(EventId.OnCommonDialog, 4, desc
                , new EventDelegate(() =>
                {
                    Application.OpenURL(packageUpdateVersion.assetTableUrl);
                }));

        }
        else if (eventId == EventId.OnHttpNotice)
        {
            // 获取http公告
            string notice = (string)args[0];
            int len = notice.Length;
            if (len < 2 )
            {
                // 没有公告，进入sdk登录
                LoginSDK();
            }
            else
            {
                // 有公告，分析公告
                int type = 0;
                int.TryParse(notice.Substring(0, 1), out type);
                string text = notice.Substring(2).Replace("\\n", "\n");
                UISystem.Instance.ShowWindow("CommonNoticeWindow");
                if (type == 0)
                {
                    EventSystem.Instance.FireEvent(EventId.OnCommonDialog, 1, text
                        , new EventDelegate(() =>
                        {
                            LoginSDK();
                        }));
                }
                else
                {
                    EventSystem.Instance.FireEvent(EventId.OnCommonDialog, 4, text);
                }
            }

        }
        else if (eventId == EventId.OnABDownloadIndex)
        {
            int index = (int)args[0];
            int size = (int)args[1];
            loadingTip.text += string.Format(LanguageDataProvider.GetValue(306), index + 1, size / 8000);
            SetProgress(targetProgress + (index + 1) * progressInterval);

        }
        else if (eventId == EventId.OnABDownloading)
        {
            float progress = (float)args[0];
            int bytes = (int)args[1];
            Debug.Log("progress : " + progress + "  已接收:" + bytes);

        }
        else if (eventId == EventId.OnABDownloadingFinished)
        {
            loadingTip.text += LanguageDataProvider.GetValue(307);
            SetProgress(0.6f);

            loadingTip.text += LanguageDataProvider.GetValue(308);
            UpdateSystem.Get().ReloadClient();
            loadingTip.text += LanguageDataProvider.GetValue(309);

            RequestOfflineNotice();
        }
        else if (eventId == EventId.OnSDKLoginResult)
        {
            Coroutine.Start(LoginServer());
        }

        else if (eventId == EventId.UpdateChaptersView)
        {
            UISystem.Get().HideAllWindow();
            UISystem.Get().ShowWindow("LobbyWindowView");
            EventSystem.Instance.FireEvent(EventId.UpdateChaptersWindow, 1);
        }

        else if (eventId == EventId.OnSyncuserAndChaptersResult)
        {
            //int type    = (int)args[0];
            //int nResult = (int)args[1];
            //if( type == 0 )
            //{
            //    if (UserSyncSysteam.Get().curLoadStruct != null && nResult == 1)
            //    {
            //        if (UserSyncSysteam.Get().curLoadStruct.regtimeSaveFile != LocalAccountStorage.Get().regtimeSaveFile)
            //            UISystem.Instance.ShowWindow("CloudWindow");
            //        else
            //            StartSingleGame();
            //    }
            //    else
            //    {
            //        StartSingleGame();
            //    }
            //}
        }
    }

    public void ReconnectResume()
    {
        // 此时根据选择是否需要恢复
        NetSystem.Instance.helper.ReconnectResume();
    }


    public void ReconnectGiveup()
    {
        // 此时根据选择是否需要恢复
        NetSystem.Instance.helper.RequestCancelBattle();
    }


    private void SetProgress(float progress)
    {
        targetProgress = progress;
    }

    private void UpdateProgress()
    {
        if (progressValue >= targetProgress)
        {
            return;
        }

        progressValue += progressChangeValue;
        progressValue = Mathf.Clamp01(progressValue);
        foreSprite.fillAmount = progressValue;
        progressLabel.text = string.Format("{0}%", Mathf.RoundToInt(progressValue * 100));
    }

    /// <summary>
    /// 请求离线公告
    /// </summary>
    private void RequestOfflineNotice()
    {
        Coroutine.Start(UpdateSystem.Instance.RequestHttpNotice());
    }

    /// <summary>
    /// 登录sdk
    /// </summary>
    private void LoginSDK()
    {
        ThirdPartySystem.Instance.Login();
    }


    private IEnumerator LoginServer()
    {
        yield return new WaitForSeconds(0.2f);
        loadingTip.text += LanguageDataProvider.GetValue(310);
        // 预加载资源
        AssetManager.Get().LoadBattleResources();
        SetProgress(0.8f);

        yield return new WaitForSeconds(0.2f);

        yield return NetSystem.Instance.helper.ConnectServer();

        if (NetSystem.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTED)
        {
            SetProgress(1.0f);
            yield return new WaitForSeconds(0.2f);
            loadingTip.text += LanguageDataProvider.GetValue(311);
            yield return new WaitForSeconds(0.2f);
            CheckUser();
        }
        else
        {
            yield return new WaitForSeconds(2.0f);
            SetProgress(1.0f);
            CheckUser();
        }
    }

    /// <summary>
    /// 检查当前有没有用户
    /// 	没有，输入用户名；有，login
    /// </summary>
    private void CheckUser()
    {
        if (NetSystem.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTED)
        {
            NetSystem.Instance.helper.RequestUser();
            NetSystem.Instance.helper.RequestUserSingle(false);
        }
        else
        {
            NetSystem.Instance.helper.RequestUserSingle();
        }
    }

    /// <summary>
    /// 创建默认玩家
    /// </summary>
    private void CreateDefaultUser()
    {
        int index   = Random.Range(0, 10);
        string icon = /*SelectIconWindow.GetIcon(index)*/"";

        if (NetSystem.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTED)
        {
            NetSystem.Instance.helper.CreateUser("", icon);
        }
        else
        {
            NetSystem.Instance.helper.CreateUserSingle("", icon);
        }
    }

    public void StartSingleGame()
    {
        TweenAlpha ta = gameObject.GetComponent<TweenAlpha>();
        if (ta == null)
        {
            ta = gameObject.AddComponent<TweenAlpha>();
        }

        ta.ResetToBeginning();
        ta.from = 1f;
        ta.to = 0;
        ta.duration = 0.5f;
        ta.SetOnFinished(() => {
            EventSystem.Instance.FireEvent(EventId.UpdateChaptersView, 1);
        });
    }


    /// <summary>
    /// 从云上拉去数据到内存中
    /// </summary>
    private void RequestAccountAndChapters()
    {
        if (NetSystem.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTED)
        {
            string objectName   = LocalAccountStorage.Get().account + ".txt";
            string filePath     = UpdateSystem.Get().saveRoot + objectName;
            NetSystem.Instance.helper.GenPresignedUrl(objectName, "GET", "text/plain", filePath, (int)EventId.OnSyncUserAndChapters);
        }
        else
        {
            StartSingleGame();
        }
    }
}

