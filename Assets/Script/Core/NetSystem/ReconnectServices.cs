using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Solarmax;






/// <summary>
/// 无界面重连,作用只处理简单数据请求, 不处理角色创建等等
/// </summary>
public class ReconnectServices : MonoSingleton<ReconnectServices>
{
    private int                 connectedNum = 0;


    public bool Init ()
	{
        return true;
	}


    /// <summary>
    /// 开始重连
    /// </summary>
	public void StartReconnect ()
	{
        // 0.5s后开始重连
        connectedNum = 0;
        Invoke ("Reconnect", 0.5f);
	}


	private void Reconnect ()
	{
		Coroutine.Start (LoginServer());
	}


    private IEnumerator LoginServer()
	{
        connectedNum++;
        yield return NetSystem.Instance.helper.ConnectServer (false);
		if (NetSystem.Instance.GetConnector ().GetConnectStatus () == ConnectionStatus.CONNECTED)
        {
			NetSystem.Instance.helper.RequestUser ();
            CancelInvoke("Reconnect");
        }
        else
        {
			yield return new WaitForSeconds (3.0f);
            if (connectedNum <= 5)
            {
                Coroutine.Start(LoginServer());
            }
            else
            {
                /// 取消重连
                CancelInvoke("Reconnect");
            }
		}
	}
}
