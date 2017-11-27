using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class Extensions
    {
        public static PrintingConfig<TOwner> Cut<TOwner>(this PropertyConfig<TOwner, string> propertyConfig, int length)
        {
            var propInfo = ((IPropertyConfig)propertyConfig).PropInfo;
            ((IPrintingConfig)((ITypeConfig<TOwner>)propertyConfig).ParentConfig).CutSymblosCount[propInfo] = length;
            return ((ITypeConfig<TOwner>)propertyConfig).ParentConfig;
        }
        public static  PrintingConfig<TOwner> UsingCulture<TOwner>(this TypeConfig<TOwner, int> typeConfig, CultureInfo cultureInfo)
        {
            var func = new Func<int, string>(x => x.ToString(cultureInfo));
           ((IPrintingConfig)((ITypeConfig<TOwner>)typeConfig).ParentConfig).SerializeWithCulture[typeof(int)] = f=>func((int)f);
            return ((ITypeConfig<TOwner>)typeConfig).ParentConfig;
        }
        public static PrintingConfig<TOwner> UsingCulture<TOwner>(this TypeConfig<TOwner, double> typeConfig, CultureInfo cultureInfo)
        {
            var func = new Func<double, string>(x => x.ToString(cultureInfo));
            ((IPrintingConfig)((ITypeConfig<TOwner>)typeConfig).ParentConfig).SerializeWithCulture[typeof(double)] = f => func((double)f);
            return ((ITypeConfig<TOwner>)typeConfig).ParentConfig;
        }
        public static string PrintToString<T>(this T obj)
        {
            var printer = ObjectPrinter.For<T>();
            return printer.PrintToString(obj);
        }

        public static string PrintToString<T>(this T obj, Func<PrintingConfig<T>, PrintingConfig<T>> func)
            => func(ObjectPrinter.For<T>()).PrintToString(obj);
    }

}
