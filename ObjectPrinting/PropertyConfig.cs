using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectPrinting
{
    public class PropertyConfig<TOwner, TPropType> : ITypeConfig<TOwner>, IPropertyConfig
    {
        private PrintingConfig<TOwner> printingConfig;
        private PropertyInfo propInfo;
        PrintingConfig<TOwner> ITypeConfig<TOwner>.ParentConfig => printingConfig;
        PropertyInfo IPropertyConfig.PropInfo => propInfo;

        public PropertyConfig(PrintingConfig<TOwner> config, PropertyInfo info)
        {
            printingConfig = config;
            propInfo = info;
        }
        public PrintingConfig<TOwner> Use(Func<TPropType, string> propertySelector)
        {
            ((IPrintingConfig)printingConfig).PropertySerialize[propInfo] = f => propertySelector((TPropType)f);
            return printingConfig;
        }

    }
    public interface IPropertyConfig
    {
        PropertyInfo PropInfo { get; }
    }
}
