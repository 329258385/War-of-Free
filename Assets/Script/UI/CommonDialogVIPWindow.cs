using System;
using UnityEngine;
using Solarmax;

public class CommonDialogVIPWindow : BaseWindow
{
	// 确认1
	public UIButton         yesBtn1;
	public UILabel          yesBtnLabel1;
	

	// 提示字
	public UILabel          tips;
	private int             type;
    private int             costMoney;
    private string          desc;
    private string          chapterName;
    public UILabel          title;
	private EventDelegate   onYes;


	private int yesCountDown = 0; // 确认按钮倒计时

	public override bool Init ()
	{
        base.Init();
        RegisterEvent (EventId.OnCommonDialog);

		return true;
	}

	public override void OnShow ()
	{
        base.OnShow();
        this.gameObject.SetActive (false);
	}

	public override void OnHide ()
	{
		onYes = null;
		yesBtn1.gameObject.SetActive (false);
	}


	public override void OnUIEventHandler (EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnCommonDialog) {
			type = (int)args [0];
			string str = (string)args [1];

			if (type == 1) {
				// 单个确定按钮
				if (args.Length > 2) {
					onYes = (EventDelegate)args [2];
				}

                if (args.Length > 3)
                {
                    costMoney = (int)args[3];
                    desc = (string)args[4];
                    chapterName = (string)args[5];
                }
                    
                

            } 
			// 设置信息
			SetInfo (type, str, costMoney,desc,chapterName);
		}
	}

	private void SetInfo(int type, string str, int costValue ,string desc,string chapterName)
	{
		yesBtn1.gameObject.SetActive (false);
		if (type == 1) {
			yesBtn1.gameObject.SetActive (true);
		}
		tips.text = str;
        yesBtnLabel1.text = costValue.ToString();
        title.text = chapterName;
        tips.text = desc;
        this.gameObject.SetActive (true);
	}

	public void OnYesClick()
	{
		if (onYes != null) {
			onYes.Execute ();
		}

		if (type != 4) {
			UISystem.Get ().HideWindow ("CommonDialogVIPWindow");
		}
	}

	public void OnNoClick()
	{
		UISystem.Get ().HideWindow ("CommonDialogVIPWindow");
	}

	public void OnClose()
	{
        UISystem.Get().HideWindow("CommonDialogVIPWindow");
    }
}

