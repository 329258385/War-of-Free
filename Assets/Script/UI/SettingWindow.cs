using System;
using UnityEngine;
using Solarmax;
using System.Collections;





public enum OPTIONTYPE
{
    OPT_NULL,
    OPT_DOWNLOAD,
    OPT_UPLOAD,
}


public class SettingWindow : BaseWindow
{
	public Animator         animator;

	public UISprite         musicBg;
	public UISprite         musicOn;
    public GameObject       DownLoad;
    public GameObject       UpLoad;
	public UILabel          musicValue;
	public UISprite         soundBg;
	public UISprite         soundOn;
    public UILabel          soundValue;
    public UISprite         chineseBg;
    public UISprite         englishBg;
    public UISprite         chineseOn;
    public UISprite         englishOn;
    public UILabel          chineseValue;
    public UILabel          englishValue;

    public UIToggle[]       effectToggles;
	public UIToggle[]       fightOptionToggles;

	public UILabel          version;
	public UILabel          ipLabel;

    public int              entryType;


	private Color btn_on_color          = new Color (0, 209 / 255.0f, 1, 1);
	private Color btn_off_color         = new Color (1, 1, 1, 0.5f);
    private OPTIONTYPE      optionType  = OPTIONTYPE.OPT_NULL;

    private void Awake()
	{
		musicBg.gameObject.GetComponent <UIEventListener> ().onClick = OnMusicClick;
		soundBg.gameObject.GetComponent <UIEventListener> ().onClick = OnSoundClick;
        chineseBg.gameObject.GetComponent<UIEventListener>().onClick = OnClickChinese;
        englishBg.gameObject.GetComponent<UIEventListener>().onClick = OnClickEnglish;
    }

    public override bool Init()
    {
        base.Init();
        RegisterEvent(EventId.ReconnectResult);
        RegisterEvent(EventId.OnHttpNotice);
        return true;
    }

    public override void OnShow ()
	{
        base.OnShow();
        SetPage ();
	}

	public override void OnHide ()
	{
        optionType = OPTIONTYPE.OPT_NULL;
    }

    public override void OnUIEventHandler(EventId eventId, params object[] args)
    {
        if( eventId == EventId.ReconnectResult )
        {
           
        }
        else if (eventId == EventId.OnHttpNotice)
        {
            // 获取http公告
            string notice = (string)args[0];
            int len = notice.Length;
            if (len < 2)
            {

            }
            else
            {
                // 有公告，分析公告
                int type = 0;
                int.TryParse(notice.Substring(0, 1), out type);
                string text = notice.Substring(2).Replace("\\n", "\n");
                UISystem.Instance.ShowWindow("CommonNoticeWindow");
                {
                    EventSystem.Instance.FireEvent(EventId.OnCommonDialog, 1, text
                        , new EventDelegate(() =>
                        {
                            ;
                        }));
                }
            }
        }
    }

	public void SetPage()
	{
		Vector3 onPos = Vector3.zero;
		Vector3 valuePos = Vector3.zero;
		// 背景音
		onPos = musicBg.transform.localPosition;
		valuePos = musicBg.transform.localPosition;
		if (LocalSettingStorage.Get ().music)
        {
			onPos.x     += 65.0f / 2; // musicOn.bg / 2;
			valuePos.x  -= 65.0f / 2;
			musicOn.color = musicBg.color = musicValue.color = btn_on_color;
			musicValue.text = LanguageDataProvider.GetValue (401);
		}
        else {
			onPos.x -= 65.0f / 2;
			valuePos.x += 65.0f / 2;
			musicOn.color = musicBg.color = musicValue.color = btn_off_color;
			musicValue.text = LanguageDataProvider.GetValue (402);
		}
		musicOn.transform.localPosition = onPos;
		musicValue.transform.localPosition = valuePos;

		// 音效
		onPos = soundBg.transform.localPosition;
		valuePos = soundBg.transform.localPosition;
		if (LocalSettingStorage.Get ().sound) {
			onPos.x += 65.0f / 2;
			valuePos.x -= 65.0f / 2;
			soundOn.color = soundBg.color = soundValue.color = btn_on_color;
			soundValue.text = LanguageDataProvider.GetValue (401);
		} else {
			onPos.x -= 65.0f / 2;
			valuePos.x += 65.0f / 2;
			soundOn.color = soundBg.color = soundValue.color = btn_off_color;
			soundValue.text = LanguageDataProvider.GetValue (402);
		}
		soundOn.transform.localPosition = onPos;
		soundValue.transform.localPosition = valuePos;

        /// 语言
        if (LocalAccountStorage.Get().localLanguage == (int)SystemLanguage.Chinese )
        {
            //chineseValue.text = LanguageDataProvider.GetValue(401);
            chineseOn.gameObject.SetActive(true);
        }
        else
        {
            //chineseValue.text = LanguageDataProvider.GetValue(402);
            chineseOn.gameObject.SetActive(false);
        }

        if (LocalAccountStorage.Get().localLanguage != (int)SystemLanguage.Chinese)
        {
            //englishValue.text = LanguageDataProvider.GetValue(401);
            englishOn.gameObject.SetActive(true);
        }
        else
        {
            //englishValue.text = LanguageDataProvider.GetValue(402);
            englishOn.gameObject.SetActive(false);
        }

        // 效果
        for (int i = 0; i < effectToggles.Length; ++i) {
			if (i == LocalSettingStorage.Get ().effectLevel) {
				effectToggles [i].Set (true, false);
				SetToggleColor (effectToggles [i].gameObject, true);
			} else {
				effectToggles [i].Set (false, false);
				SetToggleColor (effectToggles [i].gameObject, false);
			}
		}

		// 分兵方式
		for (int i = 0; i < fightOptionToggles.Length; ++i) {
			if (i == LocalSettingStorage.Get ().fightOption) {
				fightOptionToggles [i].Set (true, false);
				SetToggleColor (fightOptionToggles [i].gameObject, true);
			} else {
				fightOptionToggles [i].Set (false, false);
				SetToggleColor (fightOptionToggles [i].gameObject, false);
			}
		}

		// version
		string appVersion = string.Empty;
		//ConfigSystem.Instance.TryGetConfig ("version", out appVersion);
		version.text = Application.version;

		//ip and bundle
		ipLabel.text = NetSystem.Instance.GetConnector().GetHost().ToString();
		//bundleLabel.text = Application.bundleIdentifier;
	}

	private void SetToggleColor(GameObject go, bool on)
	{
		UISprite kuang = go.GetComponent <UISprite> ();
		if (on) {
			kuang.color = btn_on_color;
		} else {
			kuang.color = btn_off_color;
		}
	}

	public void OnMusicClick(GameObject go)
	{
		LocalSettingStorage.Get ().music = !LocalSettingStorage.Get ().music;
		SetPage ();
        AudioManger.Get().PlayEffect("click_down");
        // 播放/停止背景音
        if (LocalSettingStorage.Get ().music) {
			AudioManger.Get ().MuteBGAudio (false);
			AudioManger.Get ().PlayAudioBG ("Empty");
		} else {
			AudioManger.Get ().MuteBGAudio (true);
		}
	}

	public void OnSoundClick(GameObject go)
	{
		LocalSettingStorage.Get ().sound = !LocalSettingStorage.Get ().sound;
		SetPage ();

		// 播放/停止音效
		if (LocalSettingStorage.Get ().sound) {
            AudioManger.Get().PlayEffect("click_down");
            AudioManger.Get ().MuteEffectAudio (false);
			
		} else {
			AudioManger.Get ().MuteEffectAudio (true);
		}
	}


   
    public void OnToggleValueChanged()
	{
		for (int i = 0; i < effectToggles.Length; ++i) {
			if (effectToggles [i].value) {
				LocalSettingStorage.Get ().effectLevel = i;
			}

			SetToggleColor (effectToggles [i].gameObject, effectToggles [i].value);
		}
	}

	public void OnFightToggleValueChanged()
	{
		for (int i = 0; i < fightOptionToggles.Length; ++i) {
			if (fightOptionToggles [i].value) {
				LocalSettingStorage.Get ().fightOption = i;
			}

			SetToggleColor (fightOptionToggles [i].gameObject, fightOptionToggles [i].value);
		}
	}

	public void OnCloseClick()
	{
		UISystem.Get ().ShowWindow ("LobbyWindowView");
		UISystem.Get ().HideWindow ("SettingWindow");
        if (LocalSettingStorage.Get().lobbyWindowType == 0)
        {
            EventSystem.Instance.FireEvent(EventId.UpdateChaptersWindow);
        }
        else if (LocalSettingStorage.Get().lobbyWindowType == 1)
        {
            EventSystem.Instance.FireEvent(EventId.UpdateChapterWindow,3);
        }        
    }


    private SystemLanguage SelectLanguage = SystemLanguage.English;
    public void OnClickChinese( GameObject go )
	{
        AudioManger.Get().PlayEffect("click_down");
        SystemLanguage curLanguage = (SystemLanguage)LocalAccountStorage.Get().localLanguage;
        if( curLanguage == SystemLanguage.Chinese )
        {
            return;
        }

        SelectLanguage = SystemLanguage.Chinese;
        UISystem.Get().ShowWindow("CommonDialogWindow");
        UISystem.Get().OnEventHandler((int)EventId.OnCommonDialog, "CommonDialogWindow",
                                       2, LanguageDataProvider.GetValue(208), new EventDelegate(ModifyLanguange));
    }

    public void OnClickEnglish(GameObject go )
    {
        AudioManger.Get().PlayEffect("click_down");
        SystemLanguage curLanguage = (SystemLanguage)LocalAccountStorage.Get().localLanguage;
        if (curLanguage == SystemLanguage.English)
        {
            return;
        }

        SelectLanguage = SystemLanguage.English;
        UISystem.Get().ShowWindow("CommonDialogWindow");
        UISystem.Get().OnEventHandler((int)EventId.OnCommonDialog, "CommonDialogWindow",
                                       2, LanguageDataProvider.GetValue(208), new EventDelegate(ModifyLanguange));
    }


    private void ModifyLanguange()
    {
        LocalAccountStorage.Get().localLanguage = (int)SelectLanguage;
        LanguageDataProvider.Get().LoadLanguage(SelectLanguage);
        ReFreshLanguage();
        LocalStorageSystem.Get().SaveLocalAccount();
        SetPage();
    }

    public void OnClickFeedback( )
    {
        
    }

    public void OnClickNoticeback()
    {
        Coroutine.Start(UpdateSystem.Instance.RequestHttpNotice());
    }
}
