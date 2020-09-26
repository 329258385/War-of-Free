using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Solarmax;

public class PreviewWindow : BaseWindow
{
	/// <summary>
	/// The background.
	/// </summary>
	public UITexture background = null;
	/// <summary>
	/// 开始游戏倒计时事件
	/// </summary>
	public UILabel timeLabel = null;
	/// <summary>
	/// 等待中的其他玩家数量label
	/// </summary>
	public UILabel userNumLabel = null;
	/// <summary>
	/// 开始游戏地图展示
	/// </summary>
	//public MapShow mapShow = null;


	private int startCountTime = 0;

	private string mapId;

	public void Awake ()
	{
		//mapShow.transform.localScale = Vector3.one * 0.6f;
	}

	public override bool Init ()
	{
        base.Init();
        return true;
	}

	public override void OnShow()
	{
        base.OnShow();
        // jira-491
        if (BattleSystem.Instance.battleData.resumingFrame < 0) {
			// 默认时间和显示
			startCountTime = 4;
			UpdateStartCountTime ();

			// 地图名
			mapId = BattleSystem.Instance.battleData.matchId;

			// 背景
			//background.mainTexture = Resources.Load(BattleWindow.GetBattleBg(mapId)) as Texture2D;
			background.gameObject.SetActive (false);
			// 显示地图
			//mapShow.Switch (mapId);
			// 隐藏战斗的节点
			BattleSystem.Instance.battleData.root.SetActive(false);

			CoroutineMono.Start (SetInfo());

		} else {

			timeLabel.gameObject.SetActive (false);
			userNumLabel.gameObject.SetActive (false);

			// 隐藏战斗的节点
			BattleSystem.Instance.battleData.root.SetActive(false);

			Invoke ("OnCloseClick", 1.0f);
		}
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler (EventId eventId, params object[] args)
	{

	}

	private IEnumerator SetInfo ()
	{
		yield return 1;

		MapConfig map = MapConfigProvider.Instance.GetData (mapId);
		userNumLabel.text = string.Format ("{0} / {1}", map.player_count, map.player_count);


		// 显示展示的引导
		Team selfTeam =  BattleSystem.Instance.sceneManager.teamManager.GetTeam(BattleSystem.Instance.battleData.currentTeam);
		for (int i = 0; i < map.player_count; ++i) {
			Team team =  BattleSystem.Instance.sceneManager.teamManager.GetTeam ((TEAM)(i + 1));

			for (int j = 0; j < map.mpcList.Count; ++j) {
				MapPlayerConfig pi = map.mpcList[j];

				if (pi.camption == (int)team.team) {
					if (team == selfTeam) {
						// 创建
						UnityEngine.Object res = AssetManager.Get().GetResources("Effect_Birth");
						GameObject go = GameObject.Instantiate (res) as GameObject;
						//go.transform.SetParent (mapShow.GetEffectRoot(pi.tag).transform);
						NGUITools.SetLayer (go, go.transform.parent.gameObject.layer);
						go.transform.localPosition = Vector3.zero;
						go.transform.localScale = Vector3.one * 5f;
						Animator animator = go.GetComponent<Animator> ();
						animator.Play ("Effect_Birth_in");
					} else if (selfTeam.IsFriend (team.groupID)) {
						// 创建
						UnityEngine.Object res = AssetManager.Get().GetResources("Effect_Birth_Other");
						GameObject go = GameObject.Instantiate (res) as GameObject;
						//go.transform.SetParent (mapShow.GetEffectRoot(pi.tag).transform);
						NGUITools.SetLayer (go, go.transform.parent.gameObject.layer);
						go.transform.localPosition = Vector3.zero;
						go.transform.localScale = Vector3.one * 5f;
						Animator animator = go.GetComponent<Animator> ();
						animator.Play ("Effect_Birth_in");
					}
				}
			}
		}


		/*for (int i = 0; i < map.players.Count; ++i) {
			PlayersItem pi = map.players [i];
			if (pi.camption == (int)BattleSystem.Instance.battleData.currentTeam) {

				// 创建
				UnityEngine.Object res = AssetManager.Get().GetSprite("Effect_Birth");
				GameObject go = GameObject.Instantiate (res) as GameObject;
				go.transform.SetParent (mapShow.GetEffectRoot(pi.id).transform);
				go.transform.localPosition = Vector3.zero;
				Animator animator = go.GetComponent<Animator> ();
				animator.Play ("Effect_Birth_in");
			}
		}*/
	}

	/// <summary>
	/// 更新倒计时窗口
	/// </summary>
	private void UpdateStartCountTime()
	{
		// 倒计时
		-- startCountTime;

		if (startCountTime > 0) {
			// 显示
			timeLabel.text = string.Format ("{0}", startCountTime);
            AudioManger.Get().PlayEffect("click");
			timeLabel.transform.parent.gameObject.SetActive(true);
		} else {
			timeLabel.transform.parent.gameObject.SetActive(false);
		}

		if (startCountTime <= 0) {
			// 进入游戏，先还是不改变之前的流程，全部收到包时就进入游戏页，然后让这个页盖上它，倒计时
			OnCloseClick();
		} else {
			Invoke ("UpdateStartCountTime", 1.0f);
		}
	}

	// 倒计时结束的关闭
	public void OnCloseClick()
	{

		// 进入战斗
		//UISystem.Get ().FadeBattle(true);

		// 当前页面的渐出，透明度
		TweenAlpha ta = gameObject.GetComponent<TweenAlpha> ();
		if (ta == null) {
			ta = gameObject.AddComponent<TweenAlpha> ();
		}
		ta.ResetToBeginning ();
		ta.from = 1;
		ta.to = 0;
		ta.duration = 0.25f;
		ta.SetOnFinished (() => {
			StartGame ();
		});
		ta.Play (true);

		// 当前页面的渐出，放大
		/*
		TweenScale ts = gameObject.GetComponent<TweenScale> ();
		if (ts == null) {
			ts = gameObject.AddComponent<TweenScale> ();
		}
		ts.ResetToBeginning ();
		ts.from = Vector3.one;
		ts.to = Vector3.one * 1.25f;
		ts.duration = 0.3f;
		ts.Play (true);
		*/
	}

	private void StartGame()
	{
		GameType gameType = BattleSystem.Instance.battleData.gameType;
		GameState gameState = BattleSystem.Instance.battleData.gameState;
		if (BattleSystem.Instance.battleData.isReplay) {
			UISystem.Get ().ShowWindow ("ReplayBattleWindow");
		}
        else if (gameType == GameType.PVP || gameType == GameType.League)
        {
			if (BattleSystem.Instance.battleData.gameState == GameState.Watcher) {

				UISystem.Get ().ShowWindow ("ReplayBattleWindow");
				EventSystem.Instance.FireEvent (EventId.ShowForWatchMode);
			}
            else
            {

				// PVP战斗界面
				UISystem.Get ().ShowWindow ("BattleWindow");
			}

		} else if (gameType == GameType.Single || gameType == GameType.PayLevel || gameType == GameType.TestLevel) {
			// 单机；；；虽然单机不走preview了
			UISystem.Get ().ShowWindow ("BattleWidnow_off");
		}

		// 进入战斗
//		UISystem.Get ().FadeBattle(true, new EventDelegate (()=>{

//			BattleSystem.Instance.StartLockStep();
//			// 需要恢复
////			if (BattleSystem.Instance.battleData.resumingFrame > 0)
////			{
////				UISystem.Instance.ShowWindow ("ResumingWindow");
////			}
//		}));

		// 首先隐藏飞船
		//ShipFadeManager.Get().SetShipAlpha( 0.0f );

		UISystem.Get().HideWindow("PreviewWindow");

		EventSystem.Instance.FireEvent (EventId.NoticeSelfTeam);
	}
}

