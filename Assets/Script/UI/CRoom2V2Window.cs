using System;
using UnityEngine;
using Solarmax;

public class CRoom2V2Window : BaseWindow
{
    /// <summary>
    /// 金钱
    /// </summary>
    public UILabel      playerMoney;

    /// <summary>
    /// 队友
    /// </summary>
    public GameObject    self;
    public GameObject    player;
    

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
        UISystem.Get().ShowWindow("FriendMembersWindow");
        SetPlayerBaseInfo();
        SetSelfInfo(self);
        SetPlayerInfo(player, null);
    }

	public override void OnHide ()
	{
        UISystem.Get().HideWindow("FriendMembersWindow");
	}

	public override void OnUIEventHandler (EventId eventId, params object[] args)
	{
        if (eventId == EventId.UpdateMoney)
        {
            playerMoney.text = LocalPlayer.Get().playerData.money.ToString();
        }
    }

	public void OnTwoClick ()
	{
        NetSystem.Instance.helper.StartMatchReq(NetMessage.MatchType.MT_Ladder, string.Empty, NetMessage.CooperationType.CT_2V2, 4);
	}

	public void OnBackClick ()
	{
		UISystem.Instance.HideAllWindow ();
        UISystem.Get().ShowWindow("PvPRoomWindow");
    }

    private void SetPlayerBaseInfo()
    {
        if (LocalPlayer.Get().playerData != null)
        {
            playerMoney.text    = LocalPlayer.Get().playerData.money.ToString();
        }
    }


    private void SetSelfInfo(GameObject go )
    {
        go.SetActive(true);
        NetTexture icon     = go.transform.Find("IconB").GetComponent<NetTexture>();
        UILabel name        = go.transform.Find("Label").GetComponent<UILabel>();
        UILabel score       = go.transform.Find("Score/Score").GetComponent<UILabel>();
        {
            
            icon.picUrl     = LocalPlayer.Get().playerData.icon;
            name.text       = LocalPlayer.Get().playerData.name;
            score.text      = LocalPlayer.Get().playerData.score.ToString();
        }
    }

    private void SetPlayerInfo(GameObject go, PlayerData pd)
    {
        go.SetActive(true);
        NetTexture icon     = go.transform.Find("IconB").GetComponent<NetTexture>();
        UILabel name        = go.transform.Find("Label").GetComponent<UILabel>();
        UILabel score       = go.transform.Find("Score/Score").GetComponent<UILabel>();
        if (pd == null)
        {
            icon.gameObject.SetActive(false);
            name.gameObject.SetActive(false);
            score.gameObject.SetActive(false);
            return;
        }
       
        {
            
            icon.picUrl     = pd.icon;
            name.text       = pd.name;
            score.text      = pd.score.ToString();
        }
    }
}

