using System;
using UnityEngine;
using Solarmax;

public class CPvPRoomWindow : BaseWindow
{
    /// <summary>
    /// 角色名字
    /// </summary>
    public UILabel      playerName;

    /// <summary>
    /// 金钱
    /// </summary>
    public UILabel      playerMoney;

    /// <summary>
    /// 分数
    /// </summary>
    public UILabel      playerScore;



    public override bool Init ()
	{
        base.Init();
        RegisterEvent (EventId.OnStartMatchResult);
        RegisterEvent (EventId.UpdateMoney);
        return true;
	}
	
	public override void OnShow ()
	{
        base.OnShow();
        SetPlayerBaseInfo();
        if (NetSystem.Instance.GetConnector().GetConnectStatus() != ConnectionStatus.CONNECTED)
        {
            UISystem.Instance.ShowWindow("ReconnectWindow");
        }
	}

	public override void OnHide ()
	{
		
	}

	public override void OnUIEventHandler (EventId eventId, params object[] args)
	{
        if (eventId == EventId.UpdateMoney)
        {
            playerMoney.text = LocalPlayer.Get().playerData.money.ToString();
        }
        
    }

	public void OnFourClick ()
	{
		NetSystem.Instance.helper.StartMatchReq (NetMessage.MatchType.MT_Ladder, string.Empty, NetMessage.CooperationType.CT_1v1v1v1, 4);
	}

    public void OnThreeClick()
    {
        NetSystem.Instance.helper.StartMatchReq(NetMessage.MatchType.MT_Ladder, string.Empty, NetMessage.CooperationType.CT_1v1v1, 3);
    }

    public void OnOneClick ()
	{
		NetSystem.Instance.helper.StartMatchReq (NetMessage.MatchType.MT_Ladder, string.Empty, NetMessage.CooperationType.CT_1v1, 2);
	}

	public void OnTwoClick ()
	{
        UISystem.Instance.HideAllWindow();
        UISystem.Instance.ShowWindow("Room2V2Window");
    }

	public void OnBackClick ()
	{
		UISystem.Instance.HideAllWindow ();
        UISystem.Get().ShowWindow("StartWindow");
    }

    private void SetPlayerBaseInfo()
    {
        if (LocalPlayer.Get().playerData != null)
        {
            playerName.text     = LocalPlayer.Get().playerData.name;
            playerMoney.text    = LocalPlayer.Get().playerData.money.ToString();
            int allStars        = LocalPlayer.Get().playerData.score;
            playerScore.text    = allStars.ToString();
        }
    }
}

