using System.Collections.Generic;





#region [Enums]
/// <summary>
/// User gender
/// </summary>
public enum UserGender
{
	None = 0,
	Male,
	Female
}

/// <summary>
/// Log level
/// </summary>
public enum LogLevel
{
	/// <summary>
	/// No output
	/// </summary>
	None = 0,
	/// <summary>
	/// Default, outputs only critical log events
    /// 仅输出关键日志
	/// </summary>
	CriticalOnly,
	/// <summary>
	/// Debug level, outputs critical and main log events
    /// 输出关键日志和主要日志事件
	/// </summary>
	Debug,
	/// <summary>
	/// Highest level, outputs all log events
    /// 输出所有日志
	/// </summary>
	All
}

/// <summary>
/// Event record status
/// 事件记录状态
/// </summary>
public enum EventRecordStatus
{
	Failed = 0,//失败
	Recorded,//记录
	UniqueCountExceeded,
	ParamsCountExceeded,
	LogCountExceeded,
	LoggingDelayed
}
#endregion

/// <summary>
/// I analytics interace.
/// </summary>
public interface IAnalytics
{
	#region [Session Calls]
	/// <summary>
	/// Start a Flurry session for the project.
    /// 为项目启动一个flurry
	/// </summary>
	void StartSession(string apiKeyIOS, string apiKeyAndroid);
	#endregion

	#region [Pre-Session Calls]
	/// <summary>
	/// Explicitly specifies the App Version that Flurry will use to group Analytics data.
    /// 指定Flurry将用于分析数据的应用版本
	/// </summary>
	/// <param name="version">The custom version name.</param>
	void LogAppVersion(string version);

	/// <summary>
	/// Generates debug logs to console.
	/// </summary>
	/// <param name="level">Log level.</param>
	void SetLogLevel(LogLevel level);
    #endregion

    #region [Event and Error Logging]
    /// <summary>
    /// Records a custom event specified by eventName.
    /// </summary>
    /// <param name="eventName">
    /// Name of the event. For maximum effectiveness, we recommend using a naming scheme 
    /// that can be easily understood by non-technical people in your business domain.
    /// </param>
    EventRecordStatus LogEvent(string eventName);

    /// <summary>
    /// Records a custom parameterized event specified by eventName with parameters.
    /// </summary>
    /// <param name="eventName">
    /// Name of the event. For maximum effectiveness, we recommend using a naming scheme 
    /// that can be easily understood by non-technical people in your business domain.
    /// </param>
    /// <param name="parameters">
    /// An immutable copy of map containing Name-Value pairs of parameters.
    /// </param>
    EventRecordStatus LogEvent(string eventName, Dictionary<string, string> parameters);

    /// <summary>
    /// Records a timed event specified by eventName.
    /// </summary>
    /// <param name="eventName">
    /// Name of the event. For maximum effectiveness, we recommend using a naming scheme 
    /// that can be easily understood by non-technical people in your business domain.
    /// </param>
    EventRecordStatus BeginLogEvent(string eventName);

    /// <summary>
    /// Records a custom parameterized timed event specified by eventName with parameters.
    /// </summary>
    /// <param name="eventName">
    /// Name of the event. For maximum effectiveness, we recommend using a naming scheme 
    /// that can be easily understood by non-technical people in your business domain.
    /// </param>
    /// <param name="parameters">
    /// An immutable copy of map containing Name-Value pairs of parameters.
    /// </param>
    EventRecordStatus BeginLogEvent(string eventName, Dictionary<string, string> parameters);

    /// <summary>
    /// Ends a timed event specified by eventName and optionally updates parameters with 
    /// parameters.
    /// </summary>
    /// <param name="eventName">
    /// Name of the event. For maximum effectiveness, we recommend using a naming scheme 
    /// that can be easily understood by non-technical people in your business domain.
    /// </param>
    void EndLogEvent(string eventName);

	/// <summary>
	/// Ends a timed event specified by eventName and optionally updates parameters with 
	/// parameters.
	/// </summary>
	/// <param name="eventName">
	/// Name of the event. For maximum effectiveness, we recommend using a naming scheme 
	/// that can be easily understood by non-technical people in your business domain.
	/// </param>
	/// <param name="parameters">
	/// An immutable copy of map containing Name-Value pairs of parameters.
	/// </param>
	void EndLogEvent(string eventName, Dictionary<string, string> parameters);

	/// <summary>
	/// Records an app exception. Commonly used to catch unhandled exceptions.
	/// </summary>
	/// <param name="errorID">Name of the error.</param>
	/// <param name="message">The message to associate with the error.</param>
    /// <param name="target">The error source to report.</param>
	void LogError(string errorID, string message, object target);
	#endregion

	#region [User Info]
	/// <summary>
	/// Assign a unique id for a user in your app.
	/// </summary>
	/// <param name="userID">The app id for a user.</param>
	void LogUserID(string userID);

	/// <summary>
	/// Set your user's age in years.
	/// </summary>
	/// <param name="age">Reported age of user.</param>
	void LogUserAge(int age);

	/// <summary>
	/// Set your user's gender.
	/// </summary>
	/// <param name="gender">
	/// Reported gender of user. Allowable values are 'm' or 'c' 'f'
	/// </param>
	void LogUserGender(UserGender gender);
	#endregion
}