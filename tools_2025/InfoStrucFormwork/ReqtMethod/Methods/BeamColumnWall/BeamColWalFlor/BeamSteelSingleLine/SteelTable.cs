using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoStrucFormwork
{
    class SteelTable
    {
        /// <summary>
        /// 字母型号
        /// </summary>
        public string TypeName { get; internal set; }
        /// <summary>
        /// 尺寸型号
        /// </summary>
        public string Specifications { get; internal set; }
        /// <summary>
        /// 字母+尺寸型号
        /// </summary>
        public string DetailedName { get; internal set; }
    }
}
