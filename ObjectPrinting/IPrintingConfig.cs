using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace ObjectPrinting
{
    interface IPrintingConfig
    {
        Dictionary<Type, Func<object, string>> TypeSerialize { get; }
        Dictionary<Type, Func<object, string>> SerializeWithCulture { get; }
        Dictionary<PropertyInfo, Func<object, string>> PropertySerialize { get; }
        Dictionary<PropertyInfo, int> CutSymblosCount { get; }

    }
}
