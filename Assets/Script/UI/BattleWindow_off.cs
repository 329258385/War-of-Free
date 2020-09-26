using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Solarmax;

public class BattleWindow_off : BaseWindow
{
	public UIButton pauseBtn;
	public GameObject pausePage;

	/// <summary>
	/// 飞船数量Label
	/// </summary>
	public UILabel populationLabel = null;
	public UILabel populationValueLabel = null;

	/// <summary>
	/// 进度条，滑块图，百分比字
	/// </summary>
	public GameObject percentGo = null;
    public GameObject ProcessAram = null;
	public UISprite percentleft = null;
	public UISprite percentright = null;
	public UISprite percetPic = null;
	public UILabel percentLabel = null;

    public GameObject star1Button;
    public GameObject star2Button;
    public GameObject star3Button;
    public GameObject star4Button;

    /// <summary>
    /// 触摸精度
    /// </summary>
    private float sensitive = 1;

	/// <summary>
	/// 新的分兵条，长按
	/// </summary>
	public GameObject newProgressGo = null;
	public UISprite newProgressPic = null;
	public UILabel newProgressLabel = null;
	private int newProgressCount = 0;
	private string newProgressTag = string.Empty;

	/// <summary>
	/// 进度条，滑块图宽
	/// </summary>
	private float lineTotalLength = 0;
	/// <summary>
	/// 进度条，滑块图起始移动位置
	/// </summary>
	private Vector3 percentZeroPos = Vector3.zero;

	/// <summary>
	/// 当前的百分比
	/// </summary>
	private float percent = 0;

	private float percentPicWidth = 0;

	private int reconnectCount;
	private int checkConnectionCount;
	public UILabel popLable1;
	public UILabel popLableValue1;
	public UILabel popLableAdd;

	private int playSpeed = 2;
	public UIButton[] playSpeedButton;
    public UISprite[] playStars;
    public UISprite[] unlockplayStars;
    public UILabel desc;
    public UILabel aims;

    //private LevelConfig levelConfig;
    private int curStar;


    /// <summary>
    /// 动画播放器
    /// </summary>
    public UIPlayAnimation aniPlayer;	// 动画

    private float dropStarAnimationDelta = 0.0f;
    private bool isPlayingDropStarAnimation = false;
    private Queue<int> starQueue = new Queue<int>();
    private Queue<string> dropStarAnimationQueue = new Queue<string>();

    void Start()
	{
		percent = 1;
		SetPercent ();
		// 设置drag参数的像素修正
		UIRoot nguiRoot = UISystem.Get ().GetNGUIRoot ();
		sensitive *= nguiRoot.pixelSizeAdjustment;
	}


	public override bool Init ()
	{
        base.Init();
        RegisterEvent (EventId.OnStartSingleBattle);
        RegisterEvent (EventId.NoticeSelfTeam);
		RegisterEvent (EventId.OnBattleDisconnect);
		RegisterEvent (EventId.RequestUserResult);
		RegisterEvent (EventId.OnTouchBegin);
		RegisterEvent (EventId.OnTouchPause);
		RegisterEvent (EventId.OnTouchEnd);
		RegisterEvent (EventId.OnPopulationUp);
		RegisterEvent (EventId.OnPopulationDown);

		return true;
	}

	/// <summary>
	/// 显示当前飞船数量
	/// </summary>
	public override void OnShow()
	{
        base.OnShow();
        percentPicWidth = percetPic.width;

		// 设置分兵方式
		InitFightProgressOption ();

		Team team = BattleSystem.Instance.sceneManager.teamManager.GetTeam (BattleSystem.Instance.battleData.currentTeam);
		Color color = team.color;
		color.a = 0.7f;

		// 人口颜色
		populationLabel.color = color;
		populationValueLabel.color = color;

		populationValueLabel.text = string.Format ("{0}/{1}", team.current, team.currentMax);

		TimerProc ();

		// 游戏速度
		playSpeed = 2;
		SetPlaySpeedBtnStatus ();

        ProcessAram.GetComponent<UIEventListener>().onClick += OnSelectProcessAram;
        SetLevelStarsAndDesc();
    }


    private void OnSelectProcessAram(GameObject go)
    {
        Vector2 lefscr = UICamera.currentCamera.WorldToScreenPoint(percentleft.gameObject.transform.position);
        Vector2 rigscr = UICamera.currentCamera.WorldToScreenPoint(percentright.gameObject.transform.position);
        Vector2 curpos = UICamera.lastEventPosition;

        percent       = (curpos.x - lefscr.x) / (rigscr.x - lefscr.x);
        if (percent < 0)
            percent = 0;
        if (percent > 1.0f)
            percent = 1.0f;

        percent = Mathf.Round(percent * 100) / 100;
        SetPercent();
    }

	public override void OnUIEventHandler (EventId eventId, params object[] args)
	{
		//if (eventId == EventId.NoticeSelfTeam) {

		//	// 获取当前战斗中所有己方星球的ID

		//	if (BattleSystem.Instance.battleData.isReplay)
		//		return;	// 如果重播，则不提示这个窗口

		//	MapConfig map = MapConfigProvider.Instance.GetData (BattleSystem.Instance.battleData.matchId);

		//	Team selfTeam =  BattleSystem.Instance.sceneManager.teamManager.GetTeam (BattleSystem.Instance.battleData.currentTeam);

		//	for (int i = 0; i < map.player_count; ++i) {
		//		Team team = BattleSystem.Instance.sceneManager.teamManager.GetTeam ((TEAM)(i + 1));
		//		for (int j = 0; j < map.mpcList.Count; ++j) {
		//			MapPlayerConfig pi = map.mpcList[j];
		//			if (pi.camption == (int)team.team) {
		//				if (team == selfTeam) {

		//					Node n = BattleSystem.Instance.sceneManager.nodeManager.GetNode (pi.tag);
		//					EffectManager.Get ().ShowGuideEffect (n, true);
		//				} else if (selfTeam.IsFriend (team.groupID)) {

		//					Node n = BattleSystem.Instance.sceneManager.nodeManager.GetNode (pi.tag);
		//					EffectManager.Get ().ShowGuideEffect (n, false);
		//				}
		//			}
		//		}
		//	}

		//	TouchHandler.ShowingGuidEffect = true;
  //      }
  //      else if (eventId == EventId.OnStartSingleBattle)
  //      {
  //          OnStartSingleBattle();
  //      }
  //      else if (eventId == EventId.OnTouchBegin) {
		//	Node node = (Node)args [0];
		//	ShowNewProgress1 (true, node);
		//} else if (eventId == EventId.OnTouchPause) {
		//	Node node = (Node)args [0];
		//	PauseNewProgress1 (node);
		//} else if (eventId == EventId.OnTouchEnd) {
		//	ShowNewProgress1 (false);
		//}

		//if (eventId == EventId.OnPopulationUp) {

		//	//设置显示人口数量
		//	int current = (int)args [0];
		//	int max = (int)args [1];
		//	int pop = (int)args [2];

		//	populationValueLabel.text = string.Format ("{0} / {1}", current, max);
		//	popLableValue1.text = populationValueLabel.text;

		//	//设置变色lable
		//	popLable1.color = new Color (0x00, 0xFF, 0x00, 1f);
		//	popLableValue1.color = new Color (0x00, 0xFF, 0x00, 1f);


		//	//设置增加数量
		//	popLableAdd.text = string.Format("+{0}", pop);
		//	popLableAdd.color = new Color (0.2f, 1f, 0.2f, 1f);

		//	// 加字的偏移
		//	popLableAdd.transform.localPosition = popLableValue1.transform.localPosition + new Vector3 (popLableValue1.printedSize.x + 30, 0, 0);

		//}

		//if (eventId == EventId.OnPopulationDown) {

		//	//设置显示人口数量
		//	int current = (int)args [0];
		//	int max = (int)args [1];
		//	int pop = (int)args [2];

		//	populationValueLabel.text = string.Format ("{0} / {1}", current, max);
		//	popLableValue1.text = populationValueLabel.text;

		//	//设置变色lable
		//	popLable1.color = new Color (0xFF, 0x00, 0x00, 1f);
		//	popLableValue1.color = new Color (0xFF, 0x00, 0x00, 1f);

		//	//设置增加数量
		//	popLableAdd.text = string.Format("-{0}", pop);
		//	popLableAdd.color = new Color (1f, 0.2f, 0.2f, 1f);

		//	// 加字的偏移
		//	popLableAdd.transform.localPosition = popLableValue1.transform.localPosition + new Vector3 (popLableValue1.printedSize.x + 30, 0, 0);
		//}
	}

	public override void OnHide()
	{
        isPlayingDropStarAnimation = false;
        dropStarAnimationDelta = 0;
        starQueue.Clear();
        dropStarAnimationQueue.Clear();
    }

	void Update()
	{
		if (popLable1.alpha > 0) {
			popLableValue1.alpha = popLable1.alpha = (popLable1.alpha-Time.deltaTime * 0.5f);
			if (popLable1.alpha < 0f)
				popLableValue1.alpha = popLable1.alpha = 0f;
		}

		if (popLableAdd.alpha > 0) {
			popLableAdd.alpha -= Time.deltaTime * 0.5f;
			if (popLableAdd.alpha < 0f)
				popLableAdd.alpha = 0f;
		}

        UpdateStarProc();

        if (isPlayingDropStarAnimation) {
            dropStarAnimationDelta += Time.deltaTime;
            if (dropStarAnimationDelta > 5.5f) {
                isPlayingDropStarAnimation = false;
                dropStarAnimationDelta = 0;
            }
        }
    }

	private void TimerProc()
	{
		Team team                   =  BattleSystem.Instance.sceneManager.teamManager.GetTeam (BattleSystem.Instance.battleData.currentTeam);
		populationValueLabel.text   = string.Format ("{0} / {1}", team.current, team.currentMax);
		popLableValue1.text = populationValueLabel.text;
		Invoke ("TimerProc", 0.5f);
	}


    private void UpdateStarProc()
    {
        
    }

    Vector2 totalDrag;
	private void OnProgressDragStart (GameObject go)
	{
		totalDrag = Vector2.zero;
	}

	//private void OnProgressDragEnd (GameObject go)
	//{
 //       Vector2 lefscr = UICamera.currentCamera.WorldToScreenPoint(percentleft.gameObject.transform.position);
 //       Vector2 rigscr = UICamera.currentCamera.WorldToScreenPoint(percentright.gameObject.transform.position);
 //       Vector2 curpos = UICamera.lastEventPosition;
 //       float x = Input.mousePosition.x - Screen.width / 2f;
 //       Debug.Log(x);
 //       //float x = totalDrag.x * sensitive / lineTotalLength;
 //       x = x / lineTotalLength;
 //       //if (x <= -0.05f || x >= 0.05f)
 //       //      {
 //       //	int a = (int)(x / 0.1f);
 //       //	int b = (int)((x % 0.1f) / 0.05f);
 //       //	x = (a + b) * 0.1f;
 //       //	totalDrag.x -= x * lineTotalLength;
 //       //}
 //       //      else
 //       //      {
 //       //          return;
 //       //      }

 //       float oldPercent = percent;
 //       percent += x;
 //       percent = (curpos.x - lefscr.x) / (rigscr.x - lefscr.x);

 //       if (percent < 0)
	//		percent = 0;

	//	if (percent > 1)
	//		percent = 1;

	//	percent = Mathf.Round(percent * 100) / 100;
 //       if(oldPercent != percent)
 //           totalDrag = Vector2.zero;
 //       if (percent < 0.8f)
 //           GuideManager.TriggerGuideEnd(GuildEndEvent.touch);
	//	SetPercent ();
	//}

	private void OnProgressDrag(GameObject go, Vector2 delta)
	{
        //totalDrag   += delta;
        OnSelectProcessAram(null);
    }


	public void OnCloseClick()
	{
		
    }

	private void SetPercent()
	{
		if (LocalSettingStorage.Get ().fightOption == 1) {
			newProgressPic.fillAmount = percent;
			newProgressLabel.text = string.Format ("{0}%", Mathf.RoundToInt (percent * 100));
		} else {
			float length = lineTotalLength * percent;
			Vector3 pos = percentZeroPos;
			pos.x += length;
			int width = (int)((lineTotalLength * percent) + 2 - percentPicWidth / 2);
			percentleft.width = width;
			width = (int)((lineTotalLength * (1 - percent)) + 2 - percentPicWidth / 2);
			percentright.width = width;
			percentleft.gameObject.SetActive (true);
			percentright.gameObject.SetActive (true);
			if (percent == 0)
				percentleft.gameObject.SetActive (false);
			else if (percent == 1)
				percentright.gameObject.SetActive (false);
			//percentleft.fillAmount = percent - 0.06f;
			//percetright.fillAmount = 1f - percent - 0.06f;
			percetPic.transform.localPosition = pos;
			// 设置百分比数字
			percentLabel.text = string.Format ("{0}%", Mathf.RoundToInt (percent * 100));
		}

		// 设置游戏内飞船移动百分比
		BattleSystem.Instance.battleData.sliderRate = percent;
	}

	private void GetBattleMapIndex (string mapId, out int index, out int total)
	{
		List<string> mapList = new List<string> ();
		Dictionary<string, MapConfig> mapDict = MapConfigProvider.Instance.GetAllData ();
		foreach (var i in mapDict) {
			if (i.Value.vertical == true) {
				mapList.Add ((string)i.Key);
			}
		}
		index = 0;
		total = mapList.Count;
		for (int i = 0; i < total; ++i) {
			if (mapId.Equals (mapList [i])) {
				index = i;
				break;
			}
		}
		mapList.Clear ();
	}

	/// <summary>
	/// 设置背景图
	/// </summary>
	/// <param name="mapId">Map identifier.</param>
	public static void SetBattleBg (int mapIndex, int mapTotalCount)
	{
		GameObject bgParent = GameObject.Find ("Battle/BG");
		float x = (-78.66f) / mapTotalCount;
		x *= mapIndex;
		bgParent.transform.localPosition = new Vector3 (x, 0, 0);
	}


	/// <summary>
	/// 初始化分兵状态
	/// </summary>
	private void InitFightProgressOption()
	{
		percentGo.SetActive (false);
		newProgressGo.SetActive (false);


		int option = LocalSettingStorage.Get ().fightOption;
		if (option == 1) {
			// 长按
			ShowNewProgress1 (false);

		} else {
			// 普通进度条
			// 进度条滑块绑定事件
			UIEventListener listener = percetPic.gameObject.GetComponent<UIEventListener> ();
			listener.onDrag         = OnProgressDrag;
			listener.onDragStart    = OnProgressDragStart;
            //listener.onDragEnd      = OnProgressDragEnd;

            // line total Length
            lineTotalLength = 1200;

			percentZeroPos = new Vector3 (percetPic.transform.localPosition.x - (lineTotalLength/2),
				percetPic.transform.localPosition.y, 0);

			percentGo.SetActive (true);
		}
	}

	/// <summary>
	/// 新的分兵方式，长按
	/// </summary>
	private void ShowNewProgress1(bool show/*, Node node = null*/)
	{
		if (LocalSettingStorage.Get ().fightOption != 1)
			return;

		if (newProgressGo == null)
			return;

		if (show) {
			//newProgressGo.gameObject.SetActive (true);
			//Vector3 pos = Camera.main.WorldToScreenPoint (node.GetPosition ());
			//pos.x -= Screen.width / 2;
			//pos.y -= Screen.height / 2;
			//newProgressGo.transform.localPosition = pos;
			//newProgressPic.transform.localScale = Vector3.one * node.GetScale ();

   //         SetPercent ();

			//if (!IsInvoking ("UpdateNewProgress1")) {
			//	InvokeRepeating ("UpdateNewProgress1", 0.1f, 0.05f);
			//}

			//newProgressTag = node.tag;

		} else {
			newProgressGo.SetActive (false);
			CancelInvoke ("UpdateNewProgress1");
			newProgressTag = string.Empty;
			newProgressCount = 0;
			percent = 1.0f;
		}
	}

	//private void PauseNewProgress1 (Node node)
	//{
	//	if (LocalSettingStorage.Get ().fightOption != 1)
	//		return;
	//	if (newProgressTag.Equals (node.tag) && IsInvoking ("UpdateNewProgress1")) {
	//		CancelInvoke ("UpdateNewProgress1");
	//	}
	//}

	private void UpdateNewProgress1()
	{
		if (LocalSettingStorage.Get ().fightOption != 1)
			return;

		if (newProgressCount >= 40) {
			newProgressCount = 0;
		}

		percent = 1 - newProgressCount * 0.025f;

		SetPercent ();

		newProgressCount++;
	}

	public void GiveUpOnClicked()
	{
        //Pause();
        if (!BattleSystem.Instance.IsPause())
        {
            BattleSystem.Instance.SetPause(true);
        }
        UISystem.Get().ShowWindow("CommonDialogWindow");
        UISystem.Get().OnEventHandler((int)EventId.OnCommonDialog, "CommonDialogWindow",
            2, LanguageDataProvider.GetValue(1104), new EventDelegate(GiveUp), new EventDelegate(PauseCancel));
       
	}

	void GiveUp()
	{
		TweenAlpha ta = gameObject.GetComponent<TweenAlpha> ();
		if (ta == null) {
			ta = gameObject.AddComponent<TweenAlpha> ();
		}
		ta.ResetToBeginning ();
		ta.from = 1;
		ta.to = 0;
		ta.duration = 0.2f;
		ta.SetOnFinished (() => {
			BattleSystem.Instance.PlayerGiveUp ();
		});
		ta.Play (true);
	}

    public void PauseCancel()
    {
        if (pausePage.active == false)
        {
            if (BattleSystem.Instance.IsPause())
            {
                BattleSystem.Instance.SetPause(false);
            }
        }
    }

    public void OnPauseClick ()
	{
        if (BattleSystem.Instance.IsPause())
        {
            BattleSystem.Instance.SetPause(false);
            pausePage.SetActive(false);
            pauseBtn.normalSprite = "level_pause";

            pauseBtn.gameObject.GetComponent<UISprite>().MakePixelPerfect();
        }
        else
        {
            BattleSystem.Instance.SetPause(true);
            pausePage.SetActive(true);
            pauseBtn.normalSprite = "level_start";

            pauseBtn.gameObject.GetComponent<UISprite>().MakePixelPerfect();
        }
    }

    public void OnRetryClick()
    {
        //ShipFadeManager.Get().SetFadeType(ShipFadeManager.FADETYPE.OUT, 0.25f);
        //UISystem.Get().FadeOutBattle(false, new EventDelegate(() =>
        //{
        //    ReStartLevelBattle();
        //}));

        // 设置分兵方式
        Start();

        // 游戏速度
        playSpeed = 2;
        SetPlaySpeedBtnStatus();
       
        PlayAnimation("Stop");
        for (int i=0;i< playStars.Length;i++)
        {
            Color color = playStars[i].color;
            color.a = 154 / 255f;
            playStars[i].color = color;
        }
        SetLevelStarsAndDesc();
    }

    
   
    public void OnSpeedClick1 ()
	{
		if (playSpeed == 1)
			return;
        //if (!BattleSystem.Instance.bStartBattle)
        //    return;
        BattleSystem.Instance.lockStep.playSpeed = 0.5f;

        playSpeed = 1;
        SetPlaySpeedBtnStatus ();
    }

	public void OnSpeedClick2 ()
	{
		if (playSpeed == 2)
			return;

        //if (!BattleSystem.Instance.bStartBattle)
        //    return;
        BattleSystem.Instance.lockStep.playSpeed = 1;

		playSpeed = 2;
		SetPlaySpeedBtnStatus ();
	}

	public void OnSpeedClick3 ()
	{
        if (playSpeed == 3)
        	return;
        //if (!BattleSystem.Instance.bStartBattle)
        //    return;
        if (BattleSystem.Instance.IsPause())
            return;
        BattleSystem.Instance.lockStep.playSpeed = 2;

        playSpeed = 3;
        SetPlaySpeedBtnStatus ();
    }

	private void SetPlaySpeedBtnStatus ()
	{
		for (int i = 0; i < playSpeedButton.Length; ++i) {
			playSpeedButton [i].isEnabled = i != (playSpeed - 1);
		}
	}


    private void SetLevelStarsAndDesc()
    {
         
    }

    private void SetDescText(int star, float value) {
       
    }

    /// <summary>  
    /// 播放动画
    /// </summary>
    private void PlayAnimation(string strAni, float speed = 1.0f)
    {
        aniPlayer.clipName = strAni;
        aniPlayer.resetOnPlay = true;
        aniPlayer.Play(true, false);
    }

    private int difficultyLevel;
    private void ReStartLevelBattle()
    {
        // 如果不选难度，这个难度是多少？同时界面上的星星和难度选择重合，当点了一个星而不进入关卡时，这个界面表示的意思是已经得到了一颗星还是选择难度为1了呢？
       
    }

    public void OnStartSingleBattle()
    {
        // 首先隐藏飞船
        //ShipFadeManager.Get().SetShipAlpha(0f);
        TweenAlpha ta = gameObject.GetComponent<TweenAlpha>();
        if (ta == null)
        {
            ta = gameObject.AddComponent<TweenAlpha>();
        }

        ta.ResetToBeginning();
        ta.from     = 1f;
        ta.to       = 1;
        ta.duration = 0.5f;
        ta.SetOnFinished(() => {
            StartSingleBattle();
        });
        ta.Play(true);
    }

    public void StartSingleBattle()
    {
        // 进入战斗
        //UISystem.Get().FadeBattle(true, new EventDelegate(() => {
        //    BattleSystem.Instance.StartLockStep();
        //}));
    }
}

