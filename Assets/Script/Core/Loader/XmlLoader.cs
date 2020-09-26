using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;





namespace GameCore.Loader
{
    public static class XmlLoader
    {
       
        /// <summary>
        ///     XElement扩展方法 - 得到某个xml节点的属性
        /// </summary>
        /// <param name="em">xml节点</param>
        /// <param name="att">属性名称</param>
        /// <param name="def">可选的默认值</param>
        /// <returns></returns>
        public static string GetAttribute(this XElement em, string att, string def = null)
        {
            System.Diagnostics.Debug.Assert(null != em && !string.IsNullOrEmpty(att));

            var attribute = em.Attribute(att);

            if (null != attribute)
            {
                return attribute.Value;
            }
            if (null != def)
            {
                return def;
            }
            var str = string.Format("em:{0}  att:{1} not found", em, att);
            throw new KeyNotFoundException(str);
        }

        public static bool HasAttribute(this XElement em, string att)
        {
            System.Diagnostics.Debug.Assert(null != em && !string.IsNullOrEmpty(att));
            return em.Attribute(att) != null;
        }

        /// <summary>
        ///     解析INT数组
        /// </summary>
        /// <param name="em"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int[] GetIntArray(this XElement em, string att, char separator = ',')
        {
            var str = GetAttribute(em, att, "");
            if (string.IsNullOrEmpty(str))
            {
                return new int[0];
            }
            var sarr = str.Split(separator);
            var list = new List<int>();
            for (var i = 0; i < sarr.Length; ++i)
            {
                if (string.IsNullOrEmpty(sarr[i]))
                {
                    continue;
                }
                int n;
                if (!int.TryParse(sarr[i], out n))
                {
                    continue;
                }
                list.Add(n);
            }
            return list.ToArray();
        }
    }
}