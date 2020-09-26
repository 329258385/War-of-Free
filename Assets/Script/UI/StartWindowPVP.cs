using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Solarmax;






public class StartWindowPVP : BaseWindow
{
    public UISprite     userIconTexture;
	public UILabel      userNameLabel;
	public UILabel      scoreLabel;
	public UILabel      goldLabel;
    public UILabel      powerLabel;
    public UILabel      userScore;


    private int         connectedNum = 0;
	private void Awake()
	{
        connectedNum = 0;
    }

	public override bool Init ()
	{
        base.Init();
        RegisterEvent (EventId.RequestUserResult);
        RegisterEvent (EventId.ReconnectResult);
        RegisterEvent(EventId.OnRenameFinished);
        RegisterEvent(EventId.UpdatePower);
        RegisterEvent(EventId.UpdateMoney);
        return true;
	}


	public override void OnShow()
	{
        base.OnShow();
        SetPlayerInfo ();
		AudioManger.Get ().PlayAudioBG ("Empty");
        //string[] strName = gameObject.name.Split('(');
        //GuideManager.StartGuide(GuildCondition.GC_Ui, strName[0], gameObject);
    }


	public override void OnHide ()
	{
        StopCoroutine( "LoginServer" );
        //GuideManager.ClearGuideData();
	}


	public override void OnUIEventHandler (EventId eventId, params object[] args)
	{
		switch ((EventId)eventId)
		{
		case EventId.ReconnectResult:
			{
				// 刷新界面
				//SetPlayerInfo ();
                JoinGame();
			}
			break;
        case EventId.UpdatePower:
            {
               powerLabel.text = FormatPower(); 
            }
            break;
        case EventId.UpdateMoney:
            {
                scoreLabel.text = LocalPlayer.Get().playerData.money.ToString();
            }
            break;
        }
	}

	private void SetPlayerInfo()
	{
        // 设置用户名，头像等
        userNameLabel.text = LocalPlayer.Get().playerData.name;
        powerLabel.text    = FormatPower();
        scoreLabel.text    = LocalPlayer.Get().playerData.money.ToString();
        if(NetSystem.Instance.GetConnector().GetConnectStatus() != ConnectionStatus.CONNECTED)
        {
            userScore.text = LanguageDataProvider.GetValue(1115);
        }
        else
        {
            userScore.text = LocalPlayer.Get().playerData.score.ToString();
        }
        
    }


    private string FormatPower()
    {
        int nPower = LocalPlayer.Get().playerData.power;
        string fmt = string.Empty;
        fmt        = string.Format("{0} / 30", nPower);
        return fmt;
    }


    private void PlayAnimation(string state)
	{
		
	}

    /// <summary>
	/// 开始PVP战斗
	/// </summary>
	public void JoinGameOnClicked()
    {
        UISystem.Get().HideAllWindow();
        UISystem.Get().ShowWindow("PvPRoomWindow");
        //if( NetSystem.Instance.GetConnector().GetConnectStatus() != ConnectionStatus.CONNECTED )
        //{
        //    UISystem.Instance.ShowWindow("ReconnectWindow");
        //}
        //else
        //{
        //    JoinGame();
        //}
    }

    /// <summary>
	/// 开始PVP战斗
	/// </summary>
	public void JoinGame()
    {
        // 只发送匹配3
		NetSystem.Instance.helper.StartMatchReq (NetMessage.MatchType.MT_Ladder, string.Empty, NetMessage.CooperationType.CT_1v1v1v1, 4);
        //GuideManager.TriggerGuideEnd(GuildEndEvent.startpvp);
    }

    /// <summary>
    /// 设置
    /// </summary>
    public void OnSettingClick()
	{
		UISystem.Get ().HideAllWindow ();
		UISystem.Get ().ShowWindow ("SettingWindow");
	}

	/// <summary>
	/// 返回
	/// </summary>
	/// <param name="go">Go.</param>
	public void OnBackClick()
	{
        UISystem.Get().HideAllWindow();
        UISystem.Get().ShowWindow("LobbyWindowView");
        EventSystem.Instance.FireEvent(EventId.UpdateChaptersWindow);
    }
	/// <summary>
	/// 成就
	/// </summary>
	/// <param name="go">Go.</param>
	public void OnRewardClick()
	{
		UISystem.Get ().HideAllWindow ();
        UISystem.Get ().ShowWindow("FriendWindow");
	}
	/// <summary>
	/// 排行
	/// </summary>
	/// <param name="go">Go.</param>
	public void OnRankClick()
	{
		UISystem.Get ().HideAllWindow ();
		UISystem.Get ().ShowWindow ("RankWindow");
	}
	/// <summary>
	/// 回放
	/// </summary>
	/// <param name="go">Go.</param>
	public void OnRecordClick()
	{
		UISystem.Get ().HideAllWindow ();
		UISystem.Get ().ShowWindow ("ReplayWindow");
	}

	/// <summary>
	/// 房间入口
	/// </summary>
	public void OnRoomClick ()
	{
        UISystem.Get().HideAllWindow();
        UISystem.Get().ShowWindow("CreateRoomWindow");
	}


    /// <summary>
    /// 玩家自定义头像
    /// </summary>
    public void OnCustomPlayerHead()
    {

        int userID = LocalPlayer.Get().playerData.userId;
        if( userID > 0 )
        {
            NetSystem.Instance.helper.FriendSearch("", userID);
        }
    }


    /// <summary>
    /// 单机关卡
    /// </summary>
    public void OnBreakThroughMode()
	{
        UISystem.Get().HideAllWindow();
        UISystem.Get().ShowWindow("CustomSelectWindow");
    }

    /// <summary>
    /// 任务
    /// </summary>
    public void OnClickTask() {
        UISystem.Get().HideAllWindow();
        UISystem.Get().ShowWindow("TaskWindow");
    }

    /// <summary>
    /// 好友
    /// </summary>
    public void OnClickFriends()
    {
       /* Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(602), 1.0f);
        return;*/
        UISystem.Get().HideAllWindow();
        UISystem.Get().ShowWindow("FriendWindow");
    }


    /// <summary>
    /// 打开动画结束回调
    /// </summary>
    public void OnPlayerFAniamionEnd()
    {

    }

    public void OnClickAddPower()
    {
        UISystem.Get().ShowWindow("CommonDialogWindow");
        UISystem.Get().OnEventHandler( (int)EventId.OnCommonDialog, "CommonDialogWindow",
                                       2, LanguageDataProvider.GetValue(201), new EventDelegate(AddPower));
    }

    public void AddPower()
    {
        
    }
}

