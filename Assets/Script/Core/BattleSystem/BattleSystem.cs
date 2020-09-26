using System;
using System.Collections.Generic;
using UnityEngine;
using Plugin;
using Solarmax;

/// <summary>
/// 战斗系统
/// fangjun
/// </summary>
public class BattleSystem : Solarmax.Singleton<BattleSystem>, Solarmax.Lifecycle
{
	#if ROBOT
	public RobotBattleManager robotBattle;
	#endif

	/// <summary>
	/// 帧同步
	/// </summary>
	public LockStep lockStep;

	/// <summary>
	/// 场景管理
	/// </summary>
	public SceneManager sceneManager;

	/// <summary>
	/// 战斗数据
	/// </summary>
	public BattleData               battleData;

    public int currentFrameCount { get { return null == lockStep ? 0 : lockStep.messageCount; }}

	public int currentFps { get { return null == lockStep ? 0 : lockStep.FPS; }}

	/// <summary>
	/// 战斗控制器
	/// </summary>
	private IBattleController battleController;
	public  SingleBattleController singleBattleController;
    public PVPBattleController pvpBattleController;

	/// <summary>
	/// 暂停
	/// </summary>
	private bool pause;
    public  bool bStartBattle = false;

    /// <summary>
    /// 游戏未暂停，但不可操作
    /// </summary>
    public bool canOperation = true;

    public delegate void OnPauseDelegate(bool pause);
    public OnPauseDelegate onPauseDelegate;

    public BattleSystem ()
	{
		battleData              = new BattleData ();
		lockStep                = new LockStep ();
		sceneManager            = new SceneManager (battleData);
		battleController        = null;
		pvpBattleController     = new PVPBattleController (battleData, this);
		singleBattleController  = new SingleBattleController (battleData, this );
    }

	public bool Init()
	{
		LoggerSystem.Instance.Debug("BattleSystem    init  begin");

		lockStep.frameThreshold = 3;
		lockStep.AddListennerLogic (FrameTick);
		lockStep.AddListennerPacket (FramePacketRun);

		string produceValue = GameVariableConfigProvider.Instance.GetData (3);
		string[] pros = produceValue.Split (';');
		for (int i= 0; i < pros.Length; ++i)
		{
			List<float> args = Converter.ConvertNumberList<float> (pros [i]);
			sceneManager.AddProduce(args [0], args[1]);
		}

		battleData.Init ();
		sceneManager.Init ();
        #if !SERVER
        AssetManager.Get ().Init ();
		//EffectManager.Get ().Init ();
		GameTimeManager.Get ().Init ();
		#endif

		pause = false;
        bStartBattle = true;
        LoggerSystem.Instance.Debug("BattleSystem    init  end");

		return true;
	}

	public void FrameTick (int frame, float interval)
	{
		if (battleController != null)
			battleController.Tick (frame, interval);

		battleData.Tick (frame, interval);
		sceneManager.Tick(frame, interval);
    }

	public void FramePacketRun (FrameNode frameNode)
	{
		if (battleController != null)
			battleController.OnRunFramePacket (frameNode);
	}

	public void Tick(float interval)
	{
		if (pause)
			return;

		lockStep.Tick (interval);

		#if !SERVER
        float fScaleSpeed = sceneManager.GetbattleScaleSpeed();
        float fesp        = interval * fScaleSpeed;
        //EffectManager.Get().fPlayAniSpeed = fScaleSpeed;
        //EffectManager.Get().Tick(Time.frameCount, fesp);
        #endif

        //ShipFadeManager.Get ().UpdateFadeInOut (interval);
	}

	public void Destroy()
	{
		LoggerSystem.Instance.Debug("BattleSystem    destroy  begin");

		Reset ();

		if (battleController != null)
			battleController.Destroy ();
		
		lockStep.StopLockStep (true);

		#if !SERVER
		AssetManager.Get().UnLoadBattleResources ();
		#endif

		LoggerSystem.Instance.Debug("BattleSystem    destroy  end");
	}

	public void Reset ()
	{
		pause = false;
        bStartBattle = false;
        BeginFadeOut ();
		battleData.Init ();// add new init here by fangjun
		
		if (battleController != null)
			battleController.Reset ();

		lockStep.StopLockStep (false);
		sceneManager.Destroy ();
		battleData.Destroy ();

#if !SERVER
        
		GameTimeManager.Get ().Release ();
		Resources.UnloadUnusedAssets ();
		#endif

		System.GC.Collect ();
	}

	/// <summary>
	/// 战斗开始淡出
	/// </summary>
	public void BeginFadeOut()
	{
		//#if !SERVER
		//EffectManager.Instance.Destroy ();
		//#endif
	}

	public void SetPlayMode (bool pvp, bool single)
	{
		if (pvp) {
			battleController = pvpBattleController;
		} else if (single) {
			battleController = singleBattleController;
		} else {
			battleController = null;
			LoggerSystem.Instance.Error ("PlayMode error!");
		}

		LoggerSystem.Instance.Info ("设置战斗模式为：pvp:{0}, single:{1}", pvp, single);
	}

	public void OnPlayerMove (/*Node from, Node to*/)
	{
		//battleController.OnPlayerMove (from, to, battleData.sliderRate);
	}

	public void OnRecievedFramePacket (NetMessage.SCFrame frame)
	{
		battleController.OnRecievedFramePacket (frame);
	}

    public void OnRecievedScriptFrame(NetMessage.PbSCFrames frame)
    {
        battleController.OnRecievedScriptFrame(frame);
    }

    public void PlayerGiveUp ()
	{
		battleController.PlayerGiveUp ();
	}

	public void OnPlayerGiveUp(TEAM team)
	{
		battleController.OnPlayerGiveUp (team);
	}

	public void OnPlayerDirectQuit()
	{
		battleController.OnPlayerDirectQuit (battleData.currentTeam);
	}

	/// <summary>
	/// 获取当前帧号
	/// </summary>
	/// <returns>The current frame.</returns>
	public int GetCurrentFrame()
	{
		return lockStep.GetCurrentFrame ();
	}

	/// <summary>
	/// 启动lockstep
	/// </summary>
	public void StartLockStep ()
	{
		if (battleController != null)
			battleController.Init ();

        this.pause = false;
        bStartBattle = true;
        lockStep.StarLockStep();
	}

	public void StopLockStep ()
	{
		lockStep.StopLockStep ();
    }

	/// <summary>
	/// 暂停
	/// </summary>
	public void SetPause (bool status)
	{
        pause = status;
        if (onPauseDelegate != null) {
            onPauseDelegate(status);
        }
    }

	public bool IsPause ()
	{
		return pause;
	}
}


