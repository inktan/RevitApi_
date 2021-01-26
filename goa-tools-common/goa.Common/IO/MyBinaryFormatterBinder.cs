/*****************************
 * Copyright © Liyi Zhu 2018 *
 *****************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace goa.Common
{
    public sealed class MyBinaryFormatterBinder
        : System.Runtime.Serialization.SerializationBinder
    {
        public override System.Type BindToType(
          string assemblyName,
          string typeName)
        {
            return Type.GetType(string.Format("{0}, {1}",
              typeName, assemblyName));
        }
    }
}
