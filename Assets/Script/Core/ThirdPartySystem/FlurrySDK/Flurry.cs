using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Profiling;





/// <summary>
/// Flurry multiplatform implementation
/// </summary>
public sealed class Flurry : MonoSingleton<Flurry>, IAnalytics
{

    /// <summary>
    /// 打点需要变量
    /// </summary>
    Dictionary<string, string> flurry_Battle    = new Dictionary<string, string>();
    Dictionary<string, string> flurry_Login     = new Dictionary<string, string>();
    Dictionary<string, string> flurry_Money     = new Dictionary<string, string>();
    Dictionary<string, string> flurry_Match     = new Dictionary<string, string>();
    Dictionary<string, string> flurry_Pvp       = new Dictionary<string, string>();

	
    protected override void OnDestroy()
    {
        EndSession();
        FlurryAndroid.Dispose();
		base.OnDestroy();
    }

    /// <summary>
    /// 初始化Flurry
    /// </summary>
    public void FlurryInit( )
    {
        FlurryAndroid.SetLogEnabled(true);
        StartSession("", "62PW8WPH9836DPPJ96VP");           
    }

    /// <summary>
    /// 事件打点,玩家登陆登出, "Login" "Logout"
    /// </summary>
    public void FlurryLoginEvent(string logintype = "Login", string szFrist = "false" )
    {
        flurry_Login.Clear();
        flurry_Login.Add("Account",     LocalAccountStorage.Get().account);
        flurry_Login.Add("LoginType",   logintype);
        flurry_Login.Add("Frist",       szFrist);
        flurry_Login.Add("DeviceModel",             SystemInfo.deviceModel.ToString());
        flurry_Login.Add("DeviceUniqueIdentifier",  SystemInfo.deviceUniqueIdentifier.ToString());
        flurry_Login.Add("OperatingSystem",         SystemInfo.operatingSystem);
        flurry_Login.Add("ProcessorType",           SystemInfo.processorType);
        LogEvent("LoginEvent", flurry_Login);
    }


    public void FlurryMoneyCostEvent(string costType, string szReason, string strValue )
    {
        flurry_Money.Clear();
        flurry_Money.Add("Account",     LocalAccountStorage.Get().account);
        flurry_Money.Add("CostType",    costType);
        flurry_Money.Add("CostReason",  szReason);
        flurry_Money.Add("CostValue",   strValue);
        LogEvent("MoneyCostEvent",      flurry_Money);
    }

    public void FlurryBattleEndEvent(string strLevel, string szRet, string score, string star, string destroy, string lost, string totalTime )
    {
        flurry_Battle.Clear();
        flurry_Battle.Add("Account",    LocalAccountStorage.Get().account);
        flurry_Battle.Add("Level",      strLevel);
        flurry_Battle.Add("Type",       szRet);
        flurry_Battle.Add("Score",      score);
        flurry_Battle.Add("Star",       star);
        flurry_Battle.Add("Destroy",    destroy);
        flurry_Battle.Add("Lost",       lost);
        flurry_Battle.Add("TotalTime",  totalTime);
        LogEvent("EndbattleEvent",      flurry_Battle);
    }


    public void FlurryPVPBattleEndEvent(string matchType, string strLevel, string score, string destroy, string lost, string totalTime)
    {
        flurry_Pvp.Clear();
        flurry_Pvp.Add("Account",       LocalAccountStorage.Get().account);
        flurry_Pvp.Add("MatchType",     matchType);
        flurry_Pvp.Add("Level",         strLevel);
        flurry_Pvp.Add("Score",         score);
        flurry_Pvp.Add("Destroy",       destroy);
        flurry_Pvp.Add("Lost",          lost);
        flurry_Pvp.Add("TotalTime",     totalTime);
        LogEvent("EndbattlePvpEvent",   flurry_Pvp);
    }


    public void FlurryPVPBattleMatchEvent(string matchType, string strLevel, string matchState, string matchTime, string roomID)
    {
        flurry_Match.Clear();
        flurry_Match.Add("Account",       LocalAccountStorage.Get().account);
        flurry_Match.Add("MatchType",     matchType);
        flurry_Match.Add("Level",         strLevel);
        flurry_Match.Add("MatchState",    matchState);
        flurry_Match.Add("MatchTime",     matchTime);
        flurry_Match.Add("RoomID",        roomID);
        LogEvent("MatchBattlePvpEvent", flurry_Match);
    }

    public void EndSession()
    {
        FlurryAndroid.OnEndSession();
    }


    /// <summary>
    /// Start or continue a Flurry session for the project denoted by apiKey.
    /// 开始或者继承一个由apiKey表示的flurry
    /// </summary>
    /// <param name="apiKey">The API key for this project.</param>
    public void StartSession(string apiKeyIOS, string apiKeyAndroid)
    {
        FlurryAndroid.Init(apiKeyAndroid);
		FlurryAndroid.OnStartSession();
    }
    


    /// <summary>
    /// Explicitly specifies the App Version that Flurry will use to group Analytics data.
    /// 指定特定的App版本，用于获取分析数据
    /// </summary>
    /// <param name="version">The custom version name.</param>
    public void LogAppVersion(string version)
    {
        FlurryAndroid.SetVersionName(version);
    }

    /// <summary>
    /// Generates debug logs to console.
    /// 将调试日志显示在控制台
    /// </summary>
    /// <param name="level">Log level.</param>
    public void SetLogLevel(LogLevel level)
	{
		FlurryAndroid.SetLogLevel(level);
    }
   

    public EventRecordStatus LogEvent(string eventName)
    {
        return FlurryAndroid.LogEvent(eventName);
    }


    public EventRecordStatus LogEvent(string eventName, Dictionary<string, string> parameters)
    {
        return FlurryAndroid.LogEvent(eventName, parameters);
    }


    public EventRecordStatus LogEvent(string eventName, bool timed)
    {
        return FlurryAndroid.LogEvent(eventName, timed);
    }


    public EventRecordStatus BeginLogEvent(string eventName)
    {
		return FlurryAndroid.LogEvent(eventName, true);
    }


    public EventRecordStatus BeginLogEvent(string eventName, Dictionary<string, string> parameters)
	{
        
		return FlurryAndroid.LogEvent(eventName, parameters, true);
    }


    public void EndLogEvent(string eventName)
    {
        FlurryAndroid.EndTimedEvent(eventName);
    }


    public void EndLogEvent(string eventName, Dictionary<string, string> parameters)
	{
        FlurryAndroid.EndTimedEvent(eventName, parameters);
	}


	public void LogError(string errorID, string message, object target)
	{
		FlurryAndroid.OnError(errorID, message, target.GetType().Name);
    }



    public void LogUserID(string userID)
    {
        FlurryAndroid.SetUserId(userID);
    }


    public void LogUserAge(int age)
    {
        
        FlurryAndroid.SetAge(age);
    }


	public void LogUserGender(UserGender gender)
    {
        FlurryAndroid.SetGender((byte)(gender == UserGender.Male ? 1 : gender == UserGender.Female ? 0 : -1));
    }
}