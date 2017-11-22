using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
    public class PropertyConfig<TOwner, TType> : ITypeConfig<TOwner, TType>, IPropertyConfig<TOwner>
    {
        private PrintingConfig<TOwner> printingConfig;
        private PropertyInfo propInfo;
        PrintingConfig<TOwner> ITypeConfig<TOwner, TType>.ParentConfig => printingConfig;
        PropertyInfo IPropertyConfig<TOwner>.PropInfo => propInfo;
        public PropertyConfig(PrintingConfig<TOwner> config, PropertyInfo info)
        {
            printingConfig = config;
            propInfo = info;
        }
        public PrintingConfig<TOwner> Using(Func<TType, string> func)
        {
            ((IPrintingConfig) printingConfig).PropertySerialize[propInfo] = f => func((TType)f);
            return printingConfig;
        }
    }

    public interface IPropertyConfig<TOwner>
    {
        PropertyInfo PropInfo { get; }
    }
}
