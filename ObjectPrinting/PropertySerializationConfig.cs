using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectPrinting
{
    public class PropertySerializationConfig<TOwner>
    {
        private PrintingConfig<TOwner> printingConfig;
        public PropertySerializationConfig(PrintingConfig<TOwner> config) => printingConfig = config;

        public PropertyConfig<TOwner, TPropType> ForTarget<TPropType>(Expression<Func<TOwner, TPropType>> propertySelector)
        {
            try
            {
                var propInfo = (PropertyInfo) ((MemberExpression) propertySelector.Body).Member;
                return new PropertyConfig<TOwner, TPropType>(printingConfig, propInfo);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException();
            }
        }
    }
}
