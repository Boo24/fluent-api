using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
    public class TypeConfig<TOwner,TType> : ITypeConfig<TOwner, TType>
    {
        private PrintingConfig<TOwner> printingConfig;
        public TypeConfig(PrintingConfig<TOwner> config) => printingConfig = config;
        public PrintingConfig<TOwner> Using(Func<TType, string> func)
        {
            ((IPrintingConfig) printingConfig).TypeSerialize[typeof(TType)] = p => func((TType) p);
            return printingConfig;
        }
        public PrintingConfig<TOwner> Using(CultureInfo cultureInfo)
        {
            ((IPrintingConfig)printingConfig).CultureInfo[typeof(TType)] = cultureInfo;
            return printingConfig;
        }
        PrintingConfig<TOwner> ITypeConfig<TOwner, TType>.ParentConfig => printingConfig;
    }
    public interface ITypeConfig<TOwner, TType>
    {
        PrintingConfig<TOwner> ParentConfig { get; }
    }
}
