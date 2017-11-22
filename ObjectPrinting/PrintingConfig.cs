using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner> : IPrintingConfig
    {
        private Type[] FinalTypes = {typeof(int), typeof(double), typeof(float), typeof(string), typeof(DateTime), typeof(TimeSpan) };
        private HashSet<Type> excludeTypes = new HashSet<Type>();
        private HashSet<PropertyInfo> excludeProperties = new HashSet<PropertyInfo>();
        private Dictionary<Type, Func<object, string>> typeSerialize = new Dictionary<Type, Func<object, string>>();
        private Dictionary<Type, CultureInfo> cultureInfo = new Dictionary<Type, CultureInfo>();
        private Dictionary<PropertyInfo, Func<object, string>> propertySerialize = new Dictionary<PropertyInfo, Func<object, string>>();
        private Dictionary<PropertyInfo, int> cutSymblosCount = new Dictionary<PropertyInfo, int>();
        public PrintingConfig<TOwner> ExcludeType<T>()
        { 
            excludeTypes.Add(typeof(T));
            return this;
        }
        public TypeConfig<TOwner, T> Printing<T>() => new TypeConfig<TOwner, T>(this);
        public PropertyConfig<TOwner, TPropType> Printing<TPropType>(Expression<Func<TOwner, TPropType>> func)
        {
            var propInfo = (PropertyInfo) ((MemberExpression) func.Body).Member;
            return new PropertyConfig<TOwner, TPropType>(this, propInfo);
        }
        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> func)
        {
            var propInfo = (PropertyInfo)((MemberExpression)func.Body).Member;
            excludeProperties.Add(propInfo);
            return this;
        }
        Dictionary<Type, Func<object, string>> IPrintingConfig.TypeSerialize => typeSerialize;
        Dictionary<Type, CultureInfo> IPrintingConfig.CultureInfo => cultureInfo;
        Dictionary<PropertyInfo, Func<object, string>> IPrintingConfig.PropertySerialize => propertySerialize;
        Dictionary<PropertyInfo, int> IPrintingConfig.CutSymblosCount => cutSymblosCount;
        public string PrintToString(TOwner obj) => PrintToString(obj, 0);
        private string PrintToString(object obj, int nestingLevel)
        {
            if (obj == null)
                return "null" + Environment.NewLine;
            if (FinalTypes.Contains(obj.GetType()))
                return obj + Environment.NewLine;
            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties())
            {
                var formatStringStart = identation + propertyInfo.Name + " = ";
                if (excludeProperties.Contains(propertyInfo) || excludeTypes.Contains(propertyInfo.PropertyType)) continue;
                if(CheckSpecialSettingsForProperty(propertyInfo, sb, nestingLevel, obj) ||
                    CheckSpecialSettingsForType(propertyInfo, sb, nestingLevel, obj))
                    continue;
                sb.Append(formatStringStart + PrintToString(propertyInfo.GetValue(obj), nestingLevel + 1));
            }
            return sb.ToString();
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
            sb.Append(FormStringForProperty(nestingLvl,propInfo, TrimString(propValue, cutSymblosCount[propInfo])));
            return true;
        }

        public bool CheckSpecialSettingsForType(PropertyInfo propInfo, StringBuilder sb, int nestingLvl, object obj)
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
        private string ApplyCultureInfo(PropertyInfo propertyInfo, object obj)
        {
            var toStringMeth = propertyInfo.PropertyType.GetMethod("ToString", new[] { typeof(CultureInfo) });
            return toStringMeth?.Invoke(propertyInfo.GetValue(obj), new object[] {cultureInfo[propertyInfo.PropertyType]}).ToString();
        }
        private string TrimString(object objValue, int len) => objValue.ToString().Substring(0, len);
        private string FormStringForProperty(int nestedLvl, PropertyInfo propInfo, string val) =>
            $"{new string('\t', nestedLvl + 1)}{propInfo.Name} = {val}{Environment.NewLine}";
    }
}