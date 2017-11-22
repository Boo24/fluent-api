using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
    public static class Extensions
    {
        public static PrintingConfig<TOwner> Cut<TOwner>(this PropertyConfig<TOwner, string> propertyConfig, int length)
        {
            var propInfo = ((IPropertyConfig<TOwner>) propertyConfig).PropInfo;
           ((IPrintingConfig)((ITypeConfig<TOwner, string>) propertyConfig).ParentConfig).CutSymblosCount[propInfo] = length;
            return ((ITypeConfig<TOwner, string>) propertyConfig).ParentConfig;
        }

        public static string PrintToString<T>(this T obj)
        {
            var printer = ObjectPrinter.For<T>();
            return printer.PrintToString(obj);
        }

        public static string PrintToString<T>(this T obj, Func<PrintingConfig<T>, PrintingConfig<T>> func) =>
            func(ObjectPrinter.For<T>()).PrintToString(obj);

    }

}
