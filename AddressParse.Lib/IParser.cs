using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressParse.Lib
{
    /// <summary>
    /// 解析器
    /// </summary>
    public interface IParser<T>
    {
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        T Parse(string data);
    }
}
