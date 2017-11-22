using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
    interface IPrintingConfig
    {
        Dictionary<Type, Func<object, string>> TypeSerialize { get; }
        Dictionary<Type, CultureInfo> CultureInfo { get; }
        Dictionary<PropertyInfo, Func<object, string>> PropertySerialize { get; }
        Dictionary<PropertyInfo, int> CutSymblosCount { get; }

    }
}
