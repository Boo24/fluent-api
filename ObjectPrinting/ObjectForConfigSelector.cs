using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectPrinting
{
    public class ObjectForConfigSelector<TOwner>
    {
        private PrintingConfig<TOwner> printingConfig;
        public ObjectForConfigSelector(PrintingConfig<TOwner> config) => printingConfig = config;
        public TypeConfig<TOwner,TType> ForTarget<TType>() => new TypeConfig<TOwner, TType>(printingConfig);
        public PropertyConfig<TOwner, TPropType> ForTarget<TPropType>(Expression<Func<TOwner, TPropType>> propertySelector)
        {
            if ((propertySelector.Body as MemberExpression)?.Member as PropertyInfo is null)
                throw new ArgumentException("There must be a property of the configurable type");
            var propInfo = (PropertyInfo)((MemberExpression)propertySelector.Body).Member;
            return new PropertyConfig<TOwner, TPropType>(printingConfig, propInfo);
        }
    }
}
