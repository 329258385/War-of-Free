using System;
using UnityEngine;

public class MatchWindow : BaseWindow
{
	public UILabel waitTimeLabel;
	public Animator windowAnim;


	/// <summary>
	/// 等待时间匹配时间
	/// </summary>
	private int waitTime = 10;

	public override bool Init ()
	{
        base.Init();
        RegisterEvent (EventId.OnMatch);

		return true;
	}

	public override void OnShow ()
	{
        base.OnShow();
        windowAnim.Play ("MatchWindow");
	}

	/// <summary>
	/// 每次hidewindow均调用
	/// </summary>
	public override void OnHide ()
	{

	}

	/// <summary>
	/// 窗口事件响应处理方法
	/// </summary>
	/// <param name="eventId">Event identifier.</param>
	/// <param name="args">Arguments.</param>
	public override void OnUIEventHandler (EventId eventId, params object[] args)
	{
		switch ((EventId)eventId) {
		case EventId.OnMatch:
			{
				waitTime = (int)args [0];
				waitTime++;
				UpdateWaitUserTime ();
			}
			break;
		}
	}

	/// <summary>
	/// 更新倒计时label
	/// </summary>
	private void UpdateWaitUserTime()
	{
		-- waitTime;

		waitTimeLabel.text = string.Format ("{0:D2}:{1:D2}", waitTime / 60, waitTime % 60);

		if (waitTime <= 0) {
			
		} else {
			Invoke ("UpdateWaitUserTime", 1);
		}
	}
}
