using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner> : IPrintingConfig
    {
        private HashSet<Type> notConfigurableTypes = new HashSet<Type>()
        { typeof(int), typeof(double), typeof(float), typeof(string), typeof(DateTime), typeof(TimeSpan) };
        private HashSet<Type> excludeTypes = new HashSet<Type>();
        private HashSet<PropertyInfo> excludeProperties = new HashSet<PropertyInfo>();
        private Dictionary<Type, Func<object, string>> typeSerialize = new Dictionary<Type, Func<object, string>>();
        private Dictionary<Type, Func<object, string>> cultureInfo = new Dictionary<Type, Func<object, string>>();
        private Dictionary<PropertyInfo, Func<object, string>> propertySerialize = new Dictionary<PropertyInfo, Func<object, string>>();
        private Dictionary<PropertyInfo, int> cutSymblosCount = new Dictionary<PropertyInfo, int>();

        public PrintingConfig<TOwner> ExcludeType<T>()
        {
            excludeTypes.Add(typeof(T));
            return this;
        }
        public PrintingConfig<TOwner> SetPropertySerialization(Func<ObjectForConfigSelector<TOwner>, PrintingConfig<TOwner>> propertySelector) 
            => propertySelector(new ObjectForConfigSelector<TOwner>(this));
        public PrintingConfig<TOwner> SetTypeSerialization(Func<ObjectForConfigSelector<TOwner>, PrintingConfig<TOwner>> typeSelector)
            => typeSelector(new ObjectForConfigSelector<TOwner>(this));
        public PrintingConfig<TOwner> ExcludeProperty<TPropType>(Expression<Func<TOwner, TPropType>> func)
        {
            if ((func.Body as MemberExpression)?.Member as PropertyInfo is null)
                throw new ArgumentException("There must be a property of the configurable type");
            var propInfo = (PropertyInfo) ((MemberExpression) func.Body).Member;
                excludeProperties.Add(propInfo);
                return this;
        }
        Dictionary<Type, Func<object, string>> IPrintingConfig.TypeSerialize => typeSerialize;
        Dictionary<Type, Func<object, string>> IPrintingConfig.SerializeWithCulture => cultureInfo;
        Dictionary<PropertyInfo, Func<object, string>> IPrintingConfig.PropertySerialize => propertySerialize;
        Dictionary<PropertyInfo, int> IPrintingConfig.CutSymblosCount => cutSymblosCount;
        public string PrintToString(TOwner obj) => PrintToString(obj, 0);

        private string PrintToString(object obj, int nestingLevel)
        {
            if (obj == null)
                return "null" + Environment.NewLine;
            if (notConfigurableTypes.Contains(obj.GetType()))
                return PrintNotConfigurableType(obj);
            var sb = new StringBuilder(); 
            var type = obj.GetType();
            if (obj is IEnumerable)
                return HandleCollection(obj, nestingLevel, sb);
            sb.AppendLine(type.Name);
            var identation = new string('\t', nestingLevel + 1);
            foreach (var propertyInfo in type.GetProperties())
            {
                var formatStringStart = identation + propertyInfo.Name + " = ";
                if (CheckSpecialSettings(propertyInfo, sb, nestingLevel, obj))
                    continue;
                sb.Append(formatStringStart + PrintToString(propertyInfo.GetValue(obj), nestingLevel + 1));
            }
            return sb.ToString();
        }

        private string PrintNotConfigurableType(object obj)
        {
            var objType = obj.GetType();
            if (typeSerialize.ContainsKey(objType))
                return typeSerialize[objType](obj);
            if (cultureInfo.ContainsKey(objType))
                return cultureInfo[objType](obj);
            return obj + Environment.NewLine;
        }

        private string HandleCollection(object obj, int nestingLevel, StringBuilder sb)
        {
            sb.Append($"{new string('\t', nestingLevel)}{obj.GetType().Name}");
            HandleCollection((IEnumerable) obj, nestingLevel + 1, sb);
            return sb.ToString();
        }

        private bool CheckSpecialSettings(PropertyInfo propertyInfo, StringBuilder sb, int nestingLevel, object obj)
        {
            return excludeProperties.Contains(propertyInfo) ||
                   excludeTypes.Contains(propertyInfo.PropertyType) ||
                   CheckSpecialSettingsForProperty(propertyInfo, sb, nestingLevel, obj) ||
                   CheckSpecialSettingsForType(propertyInfo, sb, nestingLevel, obj);
        }
        private void HandleCollection<T>(T obj, int nestingLevel, StringBuilder sb) where T: IEnumerable
        {
            foreach (var elem in obj)
                if(!excludeTypes.Contains(elem.GetType()))
                    sb.Append(new string('\t', nestingLevel) + PrintToString(elem, nestingLevel));
        }
        private bool CheckSpecialSettingsForProperty(PropertyInfo propInfo, StringBuilder sb, int nestingLvl, object obj)
        {
            var propValue = propInfo.GetValue(obj);
            if (propertySerialize.ContainsKey(propInfo))
            {
                sb.Append(FormStringForProperty(nestingLvl, propInfo, propertySerialize[propInfo](propValue)));
                return true;
            }
            if (!cutSymblosCount.ContainsKey(propInfo)) return false;
            sb.Append(FormStringForProperty(nestingLvl, propInfo, TrimString(propValue, cutSymblosCount[propInfo])));
            return true;
        }
        private bool CheckSpecialSettingsForType(PropertyInfo propInfo, StringBuilder sb, int nestingLvl, object obj)
        {
            var propValue = propInfo.GetValue(obj);
            var propType = propInfo.PropertyType;
            if (typeSerialize.ContainsKey(propType))
            {
                sb.Append(FormStringForProperty(nestingLvl, propInfo, typeSerialize[propType](propValue)));
                return true;
            }
            if (!cultureInfo.ContainsKey(propType)) return false;
            sb.Append(FormStringForProperty(nestingLvl, propInfo, ApplyCultureInfo(propInfo, obj)));
            return true;
        }
        private string ApplyCultureInfo(PropertyInfo propertyInfo, object obj) =>
            cultureInfo[propertyInfo.PropertyType](propertyInfo.GetValue(obj));
        private string TrimString(object objValue, int len) => objValue.ToString().Substring(0, len);
        private string FormStringForProperty(int nestedLvl, PropertyInfo propInfo, string val) =>
            $"{new string('\t', nestedLvl + 1)}{propInfo.Name} = {val}{Environment.NewLine}";
    }
}