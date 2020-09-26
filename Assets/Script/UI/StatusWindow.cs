using System;
using UnityEngine;
using System.Collections;

public class StatusWindow : BaseWindow
{	
	public UILabel userRewardLabel;	//奖励
	public UILabel userNameLabel;	//名称
	public NetTexture userIconTexture;	//头像脚本
	public UILabel championLabel;
	public UILabel goldLabel;
	public UILabel jewelLabel;
	public UIEventListener storeBtn;
	public UIEventListener raceBtn;
	public UIEventListener fightBtn;
	public UIEventListener friendBtn;
	public UIEventListener championBtn;
	public GameObject selectTab;

	private int oldGold;
	private int oldJewel;

	private void Awake()
	{
		storeBtn.onClick = ShowSelectMap;
		raceBtn.onClick = OnRaceClick;
		friendBtn.onClick = ShowRecord;
		fightBtn.onClick = OnFightClick;
		championBtn.onClick = OnChampionClick;
	}

	public override bool Init ()
	{
        base.Init();
        RegisterEvent (EventId.OnStatusWindowTabSelect);
		RegisterEvent (EventId.OnCoinSync);

		return true;
	}

	public override void OnShow ()
	{
        base.OnShow();
        SetPlayerInfo ();


	}

	public override void OnHide ()
	{

	}

	public override void OnUIEventHandler (EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnStatusWindowTabSelect) {
			int id = (int)args [0];
			Transform parent = null;
			if (id == 0)
				parent = storeBtn.transform;
			else if (id == 1)
				parent = raceBtn.transform;
			else if (id == 2)
				parent = fightBtn.transform;
			else if (id == 3)
				parent = friendBtn.transform;
			else if (id == 4)
				parent = championBtn.transform;
				
			selectTab.transform.SetParent (parent, false);
		} else if (eventId == EventId.OnCoinSync) {

			SetCoinInfo ();
		}
	}

	private void SetPlayerInfo()
	{
		// 设置用户名，头像等
		userRewardLabel.text = LocalPlayer.Get().playerData.score.ToString();
		userNameLabel.text = string.Format ("Hi, {0}", LocalPlayer.Get().playerData.name);
		userIconTexture.picUrl = LocalPlayer.Get().playerData.icon;
		oldGold = LocalPlayer.Get ().playerData.pack.GetItem ((int)NetMessage.CoinId.Gold);
		oldJewel = LocalPlayer.Get ().playerData.pack.GetItem ((int)NetMessage.CoinId.Jewel);
		goldLabel.text = oldGold.ToString ();
		jewelLabel.text = oldJewel.ToString ();
	}

	private void ShowSelectMap(GameObject go)
	{
		selectTab.transform.SetParent (go.transform, false);
		UISystem.Get ().HideAllWindow ();
		UISystem.Get ().ShowWindow ("SelectMapWindow");
	}

	private void ShowRecord(GameObject go)
	{
		selectTab.transform.SetParent (go.transform, false);
		UISystem.Get ().HideAllWindow ();
		UISystem.Get ().ShowWindow ("ReplayWindow");
	}
	private void OnRaceClick(GameObject go)
	{
		selectTab.transform.SetParent (go.transform, false);
		UISystem.Get ().HideAllWindow ();
		UISystem.Get ().ShowWindow ("RaceWindow");
	}
	private void OnFightClick(GameObject go)
	{
		selectTab.transform.SetParent (go.transform, false);
		UISystem.Get ().HideAllWindow ();
		UISystem.Get ().ShowWindow ("StartWindow");
	}
	private void OnChampionClick(GameObject go)
	{
		selectTab.transform.SetParent (go.transform, false);
		UISystem.Get ().HideAllWindow ();
		UISystem.Get ().ShowWindow ("CustomSelectWindowNew");
	}

	public void OnSettingClick()
	{
		UISystem.Get ().HideAllWindow ();
		UISystem.Get ().ShowWindow ("SettingWindow");
	}

	public void SetCoinInfo()
	{
		int newGold = LocalPlayer.Get ().playerData.pack.GetItem ((int)NetMessage.CoinId.Gold);
		if (newGold != oldGold) {
			TweenScale ts = goldLabel.gameObject.GetComponent<TweenScale> ();
			ts.ResetToBeginning ();
			ts.from = new Vector3 (1, 1, 1);
			ts.to = new Vector3 (1.3f, 1.3f, 1.3f);
			ts.duration = 0.5f;
			ts.PlayForward ();
			ts.SetOnFinished (() => {
				ts.PlayReverse();	
			});

			CoroutineMono.Start (UpdateGoldCoroutine());
		}

		int oldJewel = int.Parse (jewelLabel.text);
		int newJewel = LocalPlayer.Get ().playerData.pack.GetItem ((int)NetMessage.CoinId.Jewel);
		if (newJewel != oldJewel) {
			TweenScale ts = jewelLabel.gameObject.GetComponent<TweenScale> ();
			ts.ResetToBeginning ();
			ts.from = new Vector3 (1, 1, 1);
			ts.to = new Vector3 (1.2f, 1.2f, 1.2f);
			ts.duration = 0.5f;
			ts.PlayForward ();
			ts.SetOnFinished (() => {
				ts.PlayReverse();	
			});

			CoroutineMono.Start (UpdateJewelCoroutine());
		}

		
	}

	private IEnumerator UpdateGoldCoroutine()
	{
		int newGold = LocalPlayer.Get ().playerData.pack.GetItem ((int)NetMessage.CoinId.Gold);

		while (oldGold < newGold) {
			oldGold += (int)(Math.Ceiling ((newGold - oldGold) * 0.1f));
			goldLabel.text = oldGold.ToString ();
			yield return 1;
		}

		oldGold = newGold;
		goldLabel.text = oldGold.ToString ();
	}
	private IEnumerator UpdateJewelCoroutine()
	{
		int newJewel = LocalPlayer.Get ().playerData.pack.GetItem ((int)NetMessage.CoinId.Jewel);

		while (oldJewel < newJewel) {
			oldJewel += (int)(Math.Ceiling ((newJewel - oldJewel) * 0.1f));
			jewelLabel.text = oldJewel.ToString ();
			yield return 1;
		}
		oldJewel = newJewel;
		jewelLabel.text = oldJewel.ToString ();
	}
}
