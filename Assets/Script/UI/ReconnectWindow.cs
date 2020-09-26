using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Solarmax;







public class ReconnectWindow : BaseWindow
{

	public UILabel              tips;
	private float               waitBeginTime;
    private int                 connectedNum = 0;


    public override bool Init ()
	{
        base.Init();
        RegisterEvent (EventId.RequestUserResult);
        RegisterEvent (EventId.CreateUserResult);
        RegisterEvent (EventId.NetworkStatus);
        return true;
	}

	public override void OnShow ()
	{
        base.OnShow();
        // 0.5s后开始重连
        Invoke ("Reconnect", 0.5f);
		waitBeginTime   = Time.realtimeSinceStartup;
		tips.text       = LanguageDataProvider.GetValue (501);
		InvokeRepeating ("UpdateTips", 1.0f, 1.0f);
	}

	public override void OnUIEventHandler (EventId eventId, params object[] args)
	{
        if (eventId == EventId.RequestUserResult)
        {
            // 重连后获取玩家数据
            NetMessage.ErrCode code = (NetMessage.ErrCode)args[0];
            if (code == NetMessage.ErrCode.EC_Ok)
            {

                EventSystem.Instance.FireEvent(EventId.ReconnectResult);
                UISystem.Instance.HideWindow("ReconnectWindow");

            }
            else if (code == NetMessage.ErrCode.EC_NoExist)
            {
                CreateDefaultUser();
                UISystem.Instance.HideWindow("ReconnectWindow");
            }
            else if (code == NetMessage.ErrCode.EC_NeedResume)
            {
                // 需要恢复
                UISystem.Instance.HideWindow("ReconnectWindow");
                EventSystem.Instance.FireEvent(EventId.OnCommonDialog, 1, LanguageDataProvider.GetValue(20));
                NetSystem.Instance.helper.ReconnectResume(); // 此时根据结果来判断是走哪种恢复
                //EventSystem.Instance.FireEvent(EventId.OnCommonDialog, 2, LanguageDataProvider.GetValue(20), 
                //                                                          new EventDelegate(ReconnectResume), 
                //                                                          new EventDelegate(ReconnectGiveup) );
            }
        }

        else if (eventId == EventId.CreateUserResult)
        {
            NetMessage.ErrCode code = (NetMessage.ErrCode)args[0];
            if (code == NetMessage.ErrCode.EC_Ok)
            {
                EventSystem.Instance.FireEvent(EventId.ReconnectResult);
                UISystem.Instance.HideWindow("ReconnectWindow");
            }
        }

        else if( eventId == EventId.NetworkStatus )
        {
            bool bConnected = (bool)args[0];
            if (bConnected)
            {
                //UISystem.Instance.HideWindow("ReconnectWindow");
            }
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

    public override void OnHide ()
	{
		
	}

	public void UpdateTips ()
	{
        if(connectedNum <= 4 )
        {
            int seconds = Mathf.RoundToInt(Time.realtimeSinceStartup - waitBeginTime);
            tips.text = string.Format(LanguageDataProvider.GetValue(502), seconds);
        }
		else
        {
            tips.text = LanguageDataProvider.GetValue(18);
            EndBattle();
        }
	}


	private void Reconnect ()
	{
        CoroutineMono.Start (LoginServer());
	}


    private IEnumerator LoginServer()
	{
        connectedNum++;
        yield return NetSystem.Instance.helper.ConnectServer (false);
		if (NetSystem.Instance.GetConnector ().GetConnectStatus () == ConnectionStatus.CONNECTED)
        {
			NetSystem.Instance.helper.RequestUser ();
		}
        else
        {
			yield return new WaitForSeconds (3.0f);
            if (connectedNum <= 4)
            {
                CoroutineMono.Start(LoginServer());
            }
            else
            {
                UISystem.Instance.HideWindow("ReconnectWindow");
            }
		}
	}

    /// <summary>
    /// 创建默认玩家
    /// </summary>
    private void CreateDefaultUser()
    {
        //int index       = Random.Range(0, 10);
        //string icon     = SelectIconWindow.GetIcon(index);
        //if (NetSystem.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTED)
        //{
        //    NetSystem.Instance.helper.CreateUser("", icon);
        //}
        //else
        //{
        //    NetSystem.Instance.helper.CreateUserSingle("", icon);
        //}
    }


    private void EndBattle()
    {
        if( BattleSystem.Get().battleData.gameType == GameType.PVP )
        {
            BattleSystem.Get().pvpBattleController.QuitBattle(false);
        }
    }
}
