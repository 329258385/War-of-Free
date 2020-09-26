using System;
using UnityEngine;
using Solarmax;

public class GMWindow : BaseWindow
{
	public UIInput input;
	
	public override void OnShow ()
	{
        base.OnShow();
    }

	public override void OnHide ()
	{

	}

	public override void OnUIEventHandler (EventId eventId, params object[] args)
	{

	}

	public void OnGoldClick()
	{
		SendCMD ("add 1 10000");
	}

	public void OnJewelClick()
	{
		SendCMD ("add 2 100");
	}

	public void OnCMDClick()
	{
		if (string.IsNullOrEmpty (input.value)) {
			return;
		}

		SendCMD (input.value);
	}

	public void OnStopAIClick()
	{
		BattleSystem.Instance.battleData.useAI = false;
		NetSystem.Instance.helper.StartMatch3 ();
		OnCloseClick ();
	}

	public void OnCloseClick()
	{
		UISystem.Get ().HideWindow ("GMWindow");
	}

	private void SendCMD(string cmd)
	{
		NetSystem.Instance.helper.RequestGMCommand (cmd);
	}
}

