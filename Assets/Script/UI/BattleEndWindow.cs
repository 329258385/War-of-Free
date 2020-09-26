using UnityEngine;
using System.Collections;
using Solarmax;

public class BattleEndWindow : BaseWindow
{
	public SpriteRenderer lineSingleRenderer;
	public TweenAlpha lineSingle;
	public SpriteRenderer linePvpRenderer;
	public TweenAlpha linePvp;


	private bool haveResult;
	NetMessage.SCFinishBattle proto;
    private Team winTeam;

	private GameType gameType;

	public override bool Init ()
	{
        base.Init();
		RegisterEvent (EventId.OnFinished);
		RegisterEvent (EventId.OnFinishedColor);
       
		return true;
	}

	public override void OnShow ()
	{
        base.OnShow();
        gameType = BattleSystem.Instance.battleData.gameType;
		if (gameType == GameType.Single || gameType == GameType.PayLevel || gameType == GameType.TestLevel || gameType == GameType.SingleLevel) {
            // 单机
            BattleSystem.Instance.lockStep.Reset();
            Invoke ("FinishSingle", 3f);
			lineSingle.gameObject.SetActive (true);

            TweenAlpha ta = gameObject.GetComponent<TweenAlpha>();
            if (ta == null)
            {
                ta = gameObject.AddComponent<TweenAlpha>();
            }

            ta.ResetToBeginning();
            ta.from = 1f;
            ta.to = 0;
            ta.duration = 2.0f;
            ta.Play(true);

        }
        else
        {
			// 非单机
			haveResult = false;
			Invoke ("FinishPvp", 3f);
			linePvp.gameObject.SetActive (true);
		}
	}

	public override void OnHide ()
	{
		CancelInvoke ("FinishPvp");
		CancelInvoke ("FinishSingle");
		lineSingle.gameObject.SetActive (false);
		linePvp.gameObject.SetActive (false);
	}

	public override void OnUIEventHandler (EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnFinished) {
			haveResult = true;
			if (args.Length > 0)
				proto = args [0] as NetMessage.SCFinishBattle;
		} else if (eventId == EventId.OnFinishedColor) {
			Color winColor = (Color)args [0];
			lineSingleRenderer.color = winColor;
			linePvpRenderer.color = winColor;
            winTeam = args[1] as Team;
		}
	}

	public void FinishPvp()
	{
		if (haveResult) {
			UISystem.Get ().HideWindow ("BattleEndWindow");
			UISystem.Get ().HideWindow ("BattleWindow");
			UISystem.Get ().HideWindow ("ReplayBattleWindow");
			UISystem.Get ().ShowWindow ("ResultWindow");
			EventSystem.Instance.FireEvent (EventId.OnFinished, proto);
		} else {
			Invoke ("FinishPvpBattle", 0.5f);
		}
	}

    /// <summary>
    /// 异常结束pvp战斗
    /// </summary>
    public void FinishPvpBattle()
    {
        BattleSystem.Instance.Reset();
        UISystem.Get().HideAllWindow();
        UISystem.Get().ShowWindow("LobbyWindowView");
        EventSystem.Instance.FireEvent(EventId.UpdateChaptersWindow, 1);
    }

    public void FinishSingle()
	{

        //ShipFadeManager.Get().SetFadeType(ShipFadeManager.FADETYPE.OUT, 0.25f);
        //UISystem.Get().FadeBattle(false, new EventDelegate(() =>
        //{
        //    if (BattleSystem.Instance.battleData.gameType == GameType.SingleLevel)
        //    {
        //        if (BattleSystem.Instance.battleData.battleType == BattlePlayType.Replay)
        //        {
        //            UISystem.Get().HideAllWindow();
        //            if (BattleSystem.Instance.replayManager.battleData.isLevelReplay) {
        //                UISystem.Get().ShowWindow("LobbyWindowView");
        //            } else {
        //                UISystem.Get().ShowWindow("ReplayWindow");
        //            }
        //        }
        //        else
        //        {
        //            UISystem.Get().HideAllWindow();
        //            UISystem.Get().ShowWindow("LobbyWindowView");
        //            if (winTeam.team == BattleSystem.Instance.battleData.currentTeam)
        //            {
        //                int destorys    = winTeam.destory;
        //                int hitships    = winTeam.hitships;
        //                int produce     = winTeam.produces;
        //                UISystem.Get().ShowWindow("VictoryWindowView");
        //                EventSystem.Instance.FireEvent(EventId.VictioryWindowViewShow, BattleSystem.Instance.battleData.matchId, destorys, hitships, produce);
        //                EventSystem.Instance.FireEvent(EventId.OnSingleBattleEnd, 0);
        //            }
        //        }
        //        BattleSystem.Instance.Reset();
        //    }
        //    else if (BattleSystem.Instance.battleData.gameType == GameType.TestLevel)
        //    {
        //        UISystem.Get().HideAllWindow();
        //        UISystem.Get().ShowWindow("LobbyWindowView");
        //        EventSystem.Instance.FireEvent(EventId.UpdateChaptersWindow, 1);
        //    }
        //    else if (BattleSystem.Instance.battleData.gameType == GameType.PayLevel)
        //    {
        //        UISystem.Get().HideAllWindow();
        //        UISystem.Get().ShowWindow("ChapterWindow");
        //        if (winTeam.team == BattleSystem.Instance.battleData.currentTeam)
        //        {
        //            int destorys = winTeam.destory;
        //            int hitships = winTeam.hitships;
        //            int produce = winTeam.produces;
        //            UISystem.Get().ShowWindow("VictoryWindowView");
        //            EventSystem.Instance.FireEvent(EventId.VictioryWindowViewShow, BattleSystem.Instance.battleData.matchId, destorys, hitships, produce);
        //        }
        //        EventSystem.Instance.FireEvent(EventId.OnSingleBattleEnd, 0);
        //        BattleSystem.Instance.Reset();
        //    }
        //}));
    }
}

