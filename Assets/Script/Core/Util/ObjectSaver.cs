using System;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// 此类对象生成txt
/// 
/// 使用反射属性写入
/// </summary>

namespace Solarmax
{
	public class Object2Text
	{
		public static string GenerateText<T>(List<T> list)
		{
			if (list.Count == 0)
				return string.Empty;

			T data = list [0];
			System.Type type = data.GetType();
			
			// write title
			StringBuilder sb = new StringBuilder();
			sb.Append ("#");
			foreach (var prop in type.GetProperties()) {
				sb.Append (prop.Name);
				sb.Append ('\t');
			}
			sb.Append ('\r');
			sb.Append ('\n');
			sb.Append ("#");
			foreach (var prop in type.GetProperties()) {
				sb.Append (prop.PropertyType);
				sb.Append ('\t');
			}
			sb.Append ('\r');
			sb.Append ('\n');
	
			// write values
			foreach (var i in list) {
				foreach (var prop in type.GetProperties()) {
					sb.Append (prop.GetValue(i, null));
					sb.Append ('\t');
				}
	
				sb.Append ('\r');
				sb.Append ('\n');
			}
	
			string text = sb.ToString ();

			return text;
		}
	}
}

