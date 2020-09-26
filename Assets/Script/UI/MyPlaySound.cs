using UnityEngine;

public class MyPlaySound : MonoBehaviour
{
	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
		Custom,
		OnEnable,
		OnDisable,
	}

	public AudioClip audioClip;
	public Trigger trigger = Trigger.OnClick;

	[Range(0f, 1f)] public float volume = 1f;

	public bool forcePlay = true;// 强制播放，忽视当前节点是否被销毁

	bool mIsOver = false;

	bool canPlay
	{
		get
		{
			if (!enabled) return false;
			UIButton btn = GetComponent<UIButton>();
			return (btn == null || btn.isEnabled || forcePlay);
		}
	}

	void OnEnable ()
	{
		if (trigger == Trigger.OnEnable)
			AudioManger.Get ().PlayEffect (audioClip, volume);
	}

	void OnDisable ()
	{
		if (trigger == Trigger.OnDisable)
			AudioManger.Get ().PlayEffect (audioClip, volume);
	}

	void OnHover (bool isOver)
	{
		if (trigger == Trigger.OnMouseOver)
		{
			if (mIsOver == isOver) return;
			mIsOver = isOver;
		}

		if (canPlay && ((isOver && trigger == Trigger.OnMouseOver) || (!isOver && trigger == Trigger.OnMouseOut)))
			AudioManger.Get ().PlayEffect (audioClip, volume);
	}

	void OnPress (bool isPressed)
	{
		if (trigger == Trigger.OnPress)
		{
			if (mIsOver == isPressed) return;
			mIsOver = isPressed;
		}

		if (canPlay && ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease)))
			AudioManger.Get ().PlayEffect (audioClip, volume);
	}

	void OnClick ()
	{
		if (canPlay && trigger == Trigger.OnClick)
			AudioManger.Get ().PlayEffect (audioClip, volume);
	}

	public void Play ()
	{
		AudioManger.Get ().PlayEffect (audioClip, volume);
	}
}
