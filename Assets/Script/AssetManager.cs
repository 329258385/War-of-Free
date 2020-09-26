using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Solarmax;

public class AssetManager : Singleton<AssetManager>
{
	const string SOUND_PATH = "sounds/";
	const string SPRITE_PATH = "Sprites/";
	const string SCRIPT_PATH = "lua/";
	const string UI_PATH = "UI/";
	const string PARTICLE_PATH = "Particle/";
    const string EFFECT_PATH = "Effect/Prefab/";
 
	Dictionary<string, Object> resources = new Dictionary<string, Object>();

	public void Init()
	{
		/////////////////////////////////////////
		AddScript ("pvp");
	}
	/// <summary>
	/// 加载战斗用资源
	/// </summary>
	public void LoadBattleResources()
	{
		/////////////////////////////////////////
		/// 建筑
		AddSprite ("Entity_Castle");
		AddSprite ("Entity_Planet");
		AddSprite ("Entity_Ship");
		AddSprite ("Entity_Tower");
		AddSprite ("Entity_Door");
		AddSprite ("Entity_Barrier");
		AddSprite ("Entity_Master");
		AddSprite ("Entity_Defense");
		AddSprite ("Entity_Power");
		AddSprite ("Entity_Dilator");

        /// 新建筑
        AddSprite("Entity_BlackHole");
        AddSprite("Entity_House");
        AddSprite("Entity_Arsenal");
        AddSprite("Entity_AircraftCarrier");
        AddSprite("Entity_Attackship");
        AddSprite("Entity_AntiAttackship");
        AddSprite("Entity_AntiCaptureship");
        AddSprite("Entity_AntiLifeship");
        AddSprite("Entity_AntiSpeedship");
        AddSprite("Entity_Lifeship");
        AddSprite("Entity_Speedship");
        AddSprite("Entity_Lasercannon");
        AddSprite("Entity_Captureship");
        AddSprite("Entity_Magicstar");
        AddSprite("Entity_Hiddenstar");
        AddSprite("Entity_FixedWarpDoor");
        AddSprite("Entity_Clouds");
        AddSprite("Entity_UnknownStar");
        AddSprite("Entity_Mirror");

        /////////////////////////////////////////
        /// 特效
        AddSprite ("Entity_Pulse");
		AddSprite ("Entity_Halo");
		AddSprite ("Entity_LaserLine");
        AddSprite ("Entity_LaserLineNew");
        AddSprite("Entity_LaserLineNew0.1");
        AddSprite ("Entity_BarrierLine");
		AddSprite ("Entity_Warp");
		AddSprite ("Entity_Select");
		AddSprite ("Entity_Warning");
		AddSprite ("Effect_Door");
		AddSprite ("Effect_Birth");
		AddSprite ("Effect_Birth_Other");
		AddSprite ("Effect_Explosion");
		AddSprite ("Effect_ShipBirth");
		AddSprite ("Entity_UI_Halo");
		AddSprite ("Entity_Selected");
		AddSprite ("Entity_Line");
		AddSprite ("Skill_ChongJiBo");
        AddSprite ("Effect_Aims");


        ////////////////////////////////////////
        /// 声音
        AddSound ("explosion01");
		AddSound ("explosion02");
		AddSound ("explosion03");
		AddSound ("explosion04");
		AddSound ("explosion05");
		AddSound ("explosion06");
		AddSound ("explosion07");
		AddSound ("explosion08");
		AddSound ("jumpCharge");
		AddSound ("jumpStart");
		AddSound ("jumpEnd");
		AddSound ("capture");
		AddSound ("laser");
		AddSound ("warp_charge");
		AddSound ("warp");
		AddSound ("Swish05");
		AddSound ("TrailerHit05");
		AddSound ("TrailerHit14");
		AddSound ("onPVPdefeated");
		AddSound ("onPVPvictory");
        AddSound("onClick");
        AddSound("LevelEntry");
        AddSound("LevelExit");
        AddSound("onMyRace");
        AddSound("moveClick");
        AddSound("unlock");
        AddSound("starSound");
        AddSound("startBattle");
        AddSound("click_down");
        AddSound("onOpen");
        AddSound("click");
        AddSound("PlanetExplosion");
        ////////////////////////////////////////
        /// ui
        AddUI("TXT");

		///////////////////////////////////////
		/// 粒子
		AddParticle ("shifang");
		AddParticle ("shifang_1");
        ///////////////////////////////////////
        /// 通用特效
        AddEffect("Effect_Jiguangpao");
        AddEffect("EFF_XJ_Boom_1");
        AddEffect("Eff_XJ_Djs");
        AddEffect("EFF_XJ_XiShou");
        AddEffect("EFF_XJ_XiShou_1");
        AddEffect("EFF_XJ_XiShou_2");
        AddEffect("EFF_XJ_XiShou_3");
        AddEffect("XJ_Glow_2");
        AddEffect("EFF_XJ_BianHuan");
    }

	/// <summary>
	/// 释放战斗资源
	/// </summary>
	public void UnLoadBattleResources()
	{
		RemoveResources ("Entity_Castle");
		RemoveResources ("Entity_Planet");
		RemoveResources ("Entity_Ship");
		RemoveResources ("Entity_Tower");
		RemoveResources ("Entity_Door");
		RemoveResources ("Entity_Barrier");
		RemoveResources ("Entity_Master");
		RemoveResources ("Entity_Defense");
		RemoveResources ("Entity_Power");
		RemoveResources ("Entity_BlackHole");
		RemoveResources ("Entity_Dilator");

        RemoveResources("Entity_House");
        RemoveResources("Entity_Arsenal");
        RemoveResources("Entity_AircraftCarrier");
        RemoveResources("Entity_Attackship");
        RemoveResources("Entity_AntiAttackship");
        RemoveResources("Entity_AntiCaptureship");
        RemoveResources("Entity_AntiLifeship");
        RemoveResources("Entity_AntiSpeedship");
        RemoveResources("Entity_Lifeship");
        RemoveResources("Entity_Speedship");
        RemoveResources("Entity_Lasercannon");
        RemoveResources("Entity_Captureship");
        RemoveResources("Entity_Magicstar");
        RemoveResources("Entity_Hiddenstar");
        RemoveResources("Entity_FixedWarpDoor");
        RemoveResources("Entity_Clouds");
        RemoveResources("Entity_UnknownStar");
        RemoveResources("Entity_Mirror");

        RemoveResources ("Entity_Pulse");
		RemoveResources ("Entity_Halo");
		RemoveResources ("Entity_LaserLine");
        RemoveResources ("Entity_LaserLineNew0.1");
        RemoveResources ("Entity_BarrierLine");
		RemoveResources ("Entity_Warp");
		RemoveResources ("Entity_Select");
		RemoveResources ("Entity_Warning");
		RemoveResources ("Effect_Door");
		RemoveResources ("Effect_Birth");
		RemoveResources ("Effect_Birth_Other");
		RemoveResources ("Effect_Explosion");
		RemoveResources ("Effect_ShipBirth");
		RemoveResources ("Entity_UI_Halo");
		RemoveResources ("Entity_Selected");
		RemoveResources ("Entity_Line");
		RemoveResources ("Skill_ChongJiBo");
		RemoveResources ("EFF_XJ_Boom_1");
        RemoveResources ("Eff_XJ_Djs");
        RemoveResources ("Effect_Aims");

        RemoveResources ("explosion01");
		RemoveResources ("explosion02");
		RemoveResources ("explosion03");
		RemoveResources ("explosion04");
		RemoveResources ("explosion05");
		RemoveResources ("explosion06");
		RemoveResources ("explosion07");
		RemoveResources ("explosion08");
		RemoveResources ("jumpCharge");
		RemoveResources ("jumpStart");
		RemoveResources ("jumpEnd");
		RemoveResources ("capture");
		RemoveResources ("laser");
		RemoveResources ("warp_charge");
		RemoveResources ("warp");
        RemoveResources ("onClick");
        RemoveResources("LevelEntry");
        RemoveResources("LevelExit");
        RemoveResources("onMyRace");
        RemoveResources("moveClick");
        RemoveResources("unlock");
        RemoveResources("starSound");
        RemoveResources("startBattle");
        RemoveResources("click_down");
        RemoveResources("onOpen");
        RemoveResources("click");
        RemoveResources("PlanetExplosion");

        RemoveResources ("shifang");
		RemoveResources ("shifang_1");

        RemoveResources("Effect_Jiguangpao");
        RemoveResources("EFF_XJ_XiShou");
        RemoveResources("EFF_XJ_XiShou_1");
        RemoveResources("EFF_XJ_XiShou_2");
        RemoveResources("EFF_XJ_XiShou_3");
        RemoveResources("XJ_Glow_2");
        RemoveResources("EFF_XJ_BianHuan");

        RemoveResources("TXT");

		Resources.UnloadUnusedAssets ();
		System.GC.Collect ();
	}

	public void FakeLoadBattleResources()
	{
		AddSprite ("Entity_Planet");
		AddSprite ("Entity_Ship");
		AddSprite ("Entity_Door");


		/////////////////////////////////////////
		/// 特效
		AddSprite ("Entity_Warp");
		AddSprite ("Entity_Select");
		AddSprite ("Effect_Door");
		AddSprite ("Entity_Selected");
		AddSprite ("Entity_Line");
		AddSprite ("Effect_Explosion");
		AddSprite ("Effect_ShipBirth");
		AddSprite ("Entity_UI_Halo");

		////////////////////////////////////////
		/// 声音
		AddSound ("jumpCharge");
		AddSound ("jumpStart");
		AddSound ("jumpEnd");
		AddSound ("warp_charge");
		AddSound ("warp");
        
		////////////////////////////////////////
		/// ui
		AddUI("TXT");
	}

	/// <summary>
	/// 删除资源
	/// </summary>
	/// <param name="key">Key.</param>
	void RemoveResources(string key)
	{
		if (resources.ContainsKey (key)) {
			resources.Remove (key);
		}
	}

	/// <summary>
	/// 获取资源
	/// </summary>
	/// <returns>The resources.</returns>
	/// <param name="key">Key.</param>
	public Object GetResources(string key)
	{
		Object res = null;

		resources.TryGetValue (key, out res);

		return res;
	}

	/// <summary>
	/// 添加脚步资源
	/// </summary>
	/// <param name="key">Key.</param>
	public void AddScript(string key)
	{
		if (resources.ContainsKey (key))
			return;

		resources.Add (key, LoadResource(SCRIPT_PATH + key));
	}

	/// <summary>
	/// 添加图片资源
	/// </summary>
	/// <param name="key">Key.</param>
	public void AddSprite(string key)
	{
		if (resources.ContainsKey (key))
			return;

		resources.Add (key, LoadResource(SPRITE_PATH + key));
	}

	/// <summary>
	/// 添加声音资源
	/// </summary>
	/// <param name="key">Key.</param>
	public void AddSound(string key)
	{
		if (resources.ContainsKey (key))
			return;

		resources.Add (key, LoadResource(SOUND_PATH + key));
	}

	/// <summary>
	/// 添加ui资源
	/// </summary>
	/// <param name="key">Key.</param>
	public void AddUI(string key)
	{
		if (resources.ContainsKey (key))
			return;

		resources.Add (key, LoadResource(UI_PATH + key));
	}

	/// <summary>
	/// 添加粒子资源
	/// </summary>
	/// <param name="key">Key.</param>
	public void AddParticle(string key)
	{
		if (resources.ContainsKey (key))
			return;

		resources.Add (key, LoadResource(PARTICLE_PATH + key));
	}

    /// <summary>
    /// 添加通用特效资源
    /// </summary>
    /// <param name="key">Key.</param>
    public void AddEffect(string key)
    {
        if (resources.ContainsKey(key))
            return;

        resources.Add(key, LoadResource(EFFECT_PATH + key));
    }

    /// <summary>
    /// 读取资源
    /// </summary>
    /// <returns>The resource.</returns>
    /// <param name="path">Path.</param>
    public Object LoadResource(string path)
	{
		object asset = null;
		#if !SERVER
		if (UpdateSystem.Get ().HaveAssetBundle ()) {
			UpdateSystem.Get ().LoadAsset (path, out asset);
		}
		
		if (asset == null)
		{
			asset = Resources.Load (path);
		}
		#endif
		
		return (Object)asset;
	}

	/// <summary>
	/// 加载streaming资源
	/// 仅仅是获取了正确的资源路径，仍然使用外部读取手段
	/// </summary>
	/// <param name="relativePath">Relative path.</param>
	public string LoadStramingAsset (string relativePath)
	{
		object asset = null;
		#if !SERVER
		if (UpdateSystem.Get ().HaveAssetBundle ()) {
			UpdateSystem.Get ().LoadStreamingAssets (relativePath, out asset);
		}
		#endif

		if (asset == null) {
			return DataProviderSystem.FormatDataProviderPath (relativePath);
		}

		return (string)asset;
	}
}
