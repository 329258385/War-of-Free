using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;






public class Json  {

	public static void Test()
	{
		
	}


	public static byte[] EnCodeBytes(object msg)
	{
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter fmt = new BinaryFormatter();
            fmt.Serialize(ms, msg);
            return ms.GetBuffer();
        }
	}

	public static T DeCode<T> (byte[] bytes)
	{
        using(MemoryStream ms = new MemoryStream( bytes ))
        {
            BinaryFormatter fmt = new BinaryFormatter();
            return (T)fmt.Deserialize(ms);
        }
	}
}

/// <summary>
/// Json帮助类,  IOS 平台不支持下列接口
/// </summary>
public class JsonHelper
{
	/// <summary>
	/// 将对象序列化为JSON格式
	/// </summary>
	/// <param name="o">对象</param>
	/// <returns>json字符串</returns>
	//public static string SerializeObject(object o)
	//{
 //       //string json = JsonConvert.SerializeObject(o);
	//	return json;
	//}
}