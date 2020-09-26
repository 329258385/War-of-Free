using System;
using Solarmax;








public class LocalAccountStorage : Singleton<LocalAccountStorage>, ILocalStorage
{
    /// <summary>
    /// 帐号
    /// </summary>
    public string               account = string.Empty;
   
    /// <summary>
    /// 角色名
    /// </summary>
    public string               name = string.Empty;

    /// <summary>
    /// 存储上次设置的语言,默认汉语
    /// </summary>
    public int                 localLanguage = -1;

    /// <summary>
    /// 角色头像
    /// </summary>
    public string               icon = string.Empty;


    /// <summary>
    /// 离线时间
    /// </summary>
    public long                 timeStamp = 0;

    /// <summary>
    /// 注册时间
    /// </summary>
    public long                 regtimeStamp = 0;
    public long                 regtimeSaveFile = 0;


    public string               singleCurrentLevel    = string.Empty;
    public string               guideFightLevel       = string.Empty;


	public string Name()
	{
		return "LocalAccountStorage";
	}

	public void Save(LocalStorageSystem manager)
	{
		manager.PutString(account);
        manager.PutString(name);
        manager.PutString(icon);
		manager.PutString (singleCurrentLevel);
        manager.PutString(guideFightLevel);
        manager.PutLong(timeStamp);
        manager.PutLong(regtimeStamp);
        manager.PutInt(localLanguage);
        manager.PutLong(regtimeSaveFile);
    }

	public void Load(LocalStorageSystem manager)
	{
		account             = manager.GetString();
        name                = manager.GetString();
        icon                = manager.GetString();
		singleCurrentLevel  = manager.GetString ();
        guideFightLevel     = manager.GetString();
        timeStamp           = manager.GetLong();
        regtimeStamp        = manager.GetLong();
        localLanguage       = manager.GetInt();
        regtimeSaveFile     = manager.GetLong();
    }


    /// <summary>
    /// 根据注册日期,计算最大可能的奖励
    /// </summary>
    public int GetDailysRewardMoney()
    {
        if (regtimeStamp > 0)
        {
            DateTime dt = new DateTime(1970, 1, 1);
            dt          = dt.AddSeconds(regtimeStamp);
            TimeSpan ts = TimeSystem.Instance.GetServerTime() - dt;
            if (ts.Days > 0)
            {
                return ts.Days * 6;
            }
        }
        return 10000;
    }
}

