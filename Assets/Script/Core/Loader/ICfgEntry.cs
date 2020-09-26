using System.Xml.Linq;

namespace GameCore.Loader
{
    // 通用静态属性接口
    public interface ICfgEntry
    {
        /// <summary>
        ///     接收单个xml元素，用于初始化自身
        /// </summary>
        /// <param name="element">单个xml元素</param>
        /// <returns>加载是否成功</returns>
        bool Load(XElement element);
    }
}