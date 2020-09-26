using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Solarmax;

public class PVPWaitWindow : BaseWindow
{
	public UILabel          tips;
	public UILabel          timeLabel;

    public GameObject[]     playerGos;
    public GameObject[]     player1V1;
    public GameObject[]     player2V2;
    public GameObject[]     player3H;
    public GameObject[]     player4H;

    public GameObject       page1V1;
    public GameObject       page2V2;
    public GameObject       page3H;
    public GameObject       page4H;

    private int             timeCount;
	private PlayerData[]    nowUserDatas = new PlayerData[4] {null, null, null, null};

	private string          matchId;
	private string          roomId;
    private int             playerNum = 0;

	public override bool Init ()
	{
        base.Init();
        RegisterEvent (EventId.OnMatchInit);
		RegisterEvent (EventId.OnMatchUpdate);
        RegisterEvent (EventId.OnMatchQuit);

        return true;
	}

	public override void OnShow ()
	{
        base.OnShow();
        timeCount = -1;
		timeLabel.gameObject.SetActive (false);
        tips.text = "";
        InvokeRepeating ("TimeUpdate", 0f, 1.0f);
    }

	public override void OnHide ()
	{
        BattleSystem.Instance.battleData.root.SetActive(true);
    }


	public override void OnUIEventHandler (EventId eventId, params object[] args)
	{
        if (eventId == EventId.OnMatchInit) {
            matchId         = (string)args[0];
            roomId          = (string)args[1];
            IList<NetMessage.UserData> userList = (IList<NetMessage.UserData>)args[2];
            IList<int> userIndexList = (IList<int>)args[3];
            timeCount       = (int)args[4];
            playerNum       = (int)args[5];
            int nPlayerCount = 0;
            for (int i = 0; i < userList.Count; ++i) {
                PlayerData pd = new PlayerData();
                pd.Init(userList[i]);
                if (pd.userId > 0) {
                    nPlayerCount++;
                }
                else {
                    // AI
                    //pd.name = AIManager.GetAIName(pd.userId);
                    //pd.icon = AIManager.GetAIIcon(pd.userId);
                }

                int index   = userIndexList[i];
                nowUserDatas[index] = pd;
                SetModelPage();
                SetPage();
                CheckAllEntered();
                Flurry.Instance.FlurryPVPBattleMatchEvent("0", matchId, "0", nPlayerCount.ToString(), "");
            }
        } else if (eventId == EventId.OnMatchUpdate) {

            IList<NetMessage.UserData> userAddList  = (IList<NetMessage.UserData>)args[0];
            IList<int> userIndexAddList             = (IList<int>)args[1];
            IList<int> userIndexDeleteList          = (IList<int>)args[2];
            for (int i = 0; i < userIndexDeleteList.Count; ++i)
            {
                int index = userIndexDeleteList[i];
                nowUserDatas[index] = null;
            }

            // add
            for (int i = 0; i < userAddList.Count; ++i) {
                PlayerData pd = new PlayerData();
                pd.Init(userAddList[i]);
                if (pd.userId <= 0)
                {
                    // AI
                    //pd.name = AIManager.GetAIName(pd.userId);
                    //pd.icon = AIManager.GetAIIcon(pd.userId);
                }

                int index   = userIndexAddList[i];
                nowUserDatas[index] = pd;
            }

            SetPage();
            CheckAllEntered();
        } else if (eventId == EventId.OnMatchQuit) {
            // quit , 谁触发的quit，谁收到quit，
            NetMessage.ErrCode code = (NetMessage.ErrCode)args[0];

            if (code == NetMessage.ErrCode.EC_NotMaster) {
                Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(905), 1.0f);
            } else if (code != NetMessage.ErrCode.EC_Ok) {
                Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.Format(901, code), 1.0f);
            } else if (code == NetMessage.ErrCode.EC_Ok) {
                BattleSystem.Instance.battleData.root.SetActive(false);
                BattleSystem.Instance.Reset();
                UISystem.Get().HideAllWindow();
                UISystem.Get().ShowWindow("StartWindow");
            }
        }
    }


	private void TimeUpdate()
	{
		if (timeCount < 0)
			return;

		timeLabel.text = string.Format ("{0:D2}:{1:D2}", timeCount / 60, timeCount % 60);
		timeLabel.gameObject.SetActive (true);
		--timeCount;
	}

    private void CheckAllEntered()
    {
        int nMachingNum = 0;
        for (int i = 0; i < nowUserDatas.Length; ++i)
        {
            if (nowUserDatas[i] != null)
                nMachingNum++;
        }
        if (IsAll(nMachingNum) )
        {
            tips.text = LanguageDataProvider.GetValue(636);
        }
    }

    private bool IsAll( int num )
    {
        if(BattleSystem.Instance.battleData.battleSubType == NetMessage.CooperationType.CT_1v1 )
        {
            if (num == 2)
                return true;
        }

        else if (BattleSystem.Instance.battleData.battleSubType == NetMessage.CooperationType.CT_1v1v1 )
        {
            if (num == 3)
                return true;
        }
        else if (BattleSystem.Instance.battleData.battleSubType == NetMessage.CooperationType.CT_1v1v1v1)
        {
            if (num == 4)
                return true;
        }
        else if (BattleSystem.Instance.battleData.battleSubType == NetMessage.CooperationType.CT_2V2)
        {
            if (num == 4)
                return true;
        }
        return false;
    }

    public void SetPage()
    {
        SetUIPage();
        for (int i = 0; i < nowUserDatas.Length; ++i)
        {
            SetPlayerInfo( playerGos[i], nowUserDatas[i]);
        }
    }

    private void SetPlayerInfo(GameObject go, PlayerData pd)
	{
        go.SetActive(true);
        NetTexture icon     = go.transform.Find("Portrait/IconB").GetComponent<NetTexture>();
        UILabel name        = go.transform.Find("name").GetComponent<UILabel>();
        UILabel score       = go.transform.Find("score/score").GetComponent<UILabel>();
        GameObject matching = go.transform.Find("Matching").gameObject;
        GameObject pic      = go.transform.Find("score/pic").gameObject;
        if (pd == null)
        {
            pic.SetActive(false);
            matching.SetActive(true);
            icon.gameObject.SetActive(false);
            name.gameObject.SetActive(false);
            score.gameObject.SetActive(false);
        }
        else
        {
            pic.SetActive(true);
            matching.SetActive(false);
            icon.gameObject.SetActive(true);
            name.gameObject.SetActive(true);
            score.gameObject.SetActive(true);
            icon.picUrl = pd.icon;
            if (pd.userId == -1)
            {
                name.text = LanguageDataProvider.Format(908, pd.name);
            }
            else
            {
                name.text = pd.name;
            }
            score.text = pd.score.ToString();
        }
    }


    private void SetUIPage()
    {
        if (BattleSystem.Instance.battleData.battleSubType == NetMessage.CooperationType.CT_1v1)
        {
            int nLength = player1V1.Length;
            for (int i = 0; i < nLength; i++)
            {
                playerGos[i] = player1V1[i];
            }
        }
        else if (BattleSystem.Instance.battleData.battleSubType == NetMessage.CooperationType.CT_2V2)
        {
            int nLength = player2V2.Length;
            for (int i = 0; i < nLength; i++)
            {
                playerGos[i] = player2V2[i];
            }
        }
        else if (BattleSystem.Instance.battleData.battleSubType == NetMessage.CooperationType.CT_1v1v1)
        {
            int nLength = player3H.Length;
            for (int i = 0; i < nLength; i++)
            {
                playerGos[i] = player3H[i];
            }
        }
        else if (BattleSystem.Instance.battleData.battleSubType == NetMessage.CooperationType.CT_1v1v1v1)
        {
            int nLength = player4H.Length;
            for (int i = 0; i < nLength; i++)
            {
                playerGos[i] = player4H[i];
            }
        }
    }

    private void SetModelPage()
    {
        page1V1.SetActive(false);
        page2V2.SetActive(false);
        page3H.SetActive(false);
        page4H.SetActive(false);
        if (BattleSystem.Instance.battleData.battleSubType == NetMessage.CooperationType.CT_1v1)
        {
            page1V1.SetActive(true);
        }
        else if (BattleSystem.Instance.battleData.battleSubType == NetMessage.CooperationType.CT_2V2)
        {
            page2V2.SetActive(true);
        }
        else if (BattleSystem.Instance.battleData.battleSubType == NetMessage.CooperationType.CT_1v1v1)
        {
            page3H.SetActive(true);
        }
        else if (BattleSystem.Instance.battleData.battleSubType == NetMessage.CooperationType.CT_1v1v1v1)
        {
            page4H.SetActive(true);
        }
    }


    public void OnClickCancel() {
        // UISystem.Instance.ShowWindow("CommonDialogWindow");
        NetSystem.Instance.helper.QuitMatch();
        /*EventSystem.Instance.FireEvent(EventId.OnCommonDialog, 2, LanguageDataProvider.GetValue(907)
            , new EventDelegate(() => {
            }));
            */
    }
}