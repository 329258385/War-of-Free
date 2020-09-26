using System;
using Solarmax;

public class SingleClearWindow : BaseWindow
{
	public UIInput inputField;

	public override bool Init ()
	{
        base.Init();
        RegisterEvent (EventId.OnRename);

		return true;
	}

	public override void OnShow ()
	{
        base.OnShow();
        
	}
	
	public override void OnHide ()
	{

	}

	public override void OnUIEventHandler (EventId eventId, params object[] args)
	{
        //暂时不处理网络消息
		//if (eventId == EventId.OnRename)
		//{
		//	NetMessage.ErrCode code = (NetMessage.ErrCode)args [0];

		//	if (code == NetMessage.ErrCode.EC_Ok) {
		//		// 成功
		//		LocalPlayer.Get ().playerData.name = inputField.value;
  //              LocalAccountStorage.Get().name = inputField.value;
  //              LocalStorageSystem.Instance.SaveLocalAccount();

  //              // 进入PVP界面
  //              UISystem.Get ().HideWindow ("SingleClearWindow");
		//		EventSystem.Instance.FireEvent (EventId.OnRenameFinished);

		//	} else if (code == NetMessage.ErrCode.EC_NameExist) {
		//		Tips.Make (LanguageDataProvider.GetValue(4));
		//	} else if (code == NetMessage.ErrCode.EC_InvalidName) {
		//		Tips.Make (LanguageDataProvider.GetValue(5));
		//	} else if (code == NetMessage.ErrCode.EC_AccountExist) {
		//		Tips.Make (LanguageDataProvider.GetValue(6));
		//	} else {
		//		Tips.Make (LanguageDataProvider.GetValue(7));
		//	}

		//}

	}

	public void OnEnterNameValueChanged()
	{
		string name = inputField.value.Trim ();
		name = name.Replace ('\r', ' ');
		name = name.Replace ('\t', ' ');
		name = name.Replace ('\n', ' ');

#if UNITY_EDITOR
		while (EncodingTextLength (name) > 20) {
#else
		while (EncodingTextLength (name) > 20) {
#endif
			name = name.Substring (0, name.Length - 1);
		}

		inputField.value = name;
	}

	private int EncodingTextLength(string s)
	{
		int ret = 0;
		byte[] b;
		for (int i = 0; i < s.Length; ++i) {
			#if UNITY_EDITOR
			b = System.Text.Encoding.UTF8.GetBytes (s.Substring (i, 1));
			#else
			b = System.Text.Encoding.UTF8.GetBytes (s.Substring (i, 1));
			#endif
			ret += (b.Length > 1 ? 2 : 1);
		}

		return ret;
	}

	public void OnRandNameClick()
	{
		//inputField.value = AIManager.GetAIName(UnityEngine.Time.frameCount);
  //      GuideManager.TriggerGuideEnd(GuildEndEvent.rename);
	}

	public void OnConfirmClick()
	{
		string name = inputField.value;
		if (string.IsNullOrEmpty (name)) {

			Tips.Make (LanguageDataProvider.GetValue(10));

			return;
		}
        //判断是否有敏感字符
        bool nameIllegal = false;
        nameIllegal = NameFilterConfigProvider.Instance.nameCheck(name);
        if (nameIllegal)
        {
            Tips.Make(LanguageDataProvider.GetValue(1114));
            return;
        }
       // GuideManager.TriggerGuideEnd(GuildEndEvent.rename);
        NetSystem.Instance.helper.ChangeName (name);

        // 本地存储玩家名称
        LocalPlayer.Get().playerData.name = inputField.value;
        LocalAccountStorage.Get().name = inputField.value;
        LocalStorageSystem.Instance.SaveLocalAccount();

        // 进入PVP界面
        UISystem.Get().HideWindow("SingleClearWindow");
        EventSystem.Instance.FireEvent(EventId.UpdateChaptersWindow);
    }

	public void OnCloseClick()
	{
		
	}
}
