using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Pro.Extension
{
    public static class EnumHelper
    {
        /// <summary>
        /// 获取枚举的所有类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] GetEnumArray<T>()
        {
            List<T> list = new List<T>();
            foreach (var e in Enum.GetNames(typeof(T)))
            {
                var a = (T)Enum.Parse(typeof(T), e);
                list.Add(a);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 获取枚举的描述文本
        /// </summary>
        /// <param name="value">枚举成员</param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi == null) return null;

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if ((attributes != null) && (attributes.Length > 0))
                return attributes[0].Description;
            else
                return value.ToString();

        }

        public static string GetDescriptionOrDefault(this Enum value, string defualtDesc = null)
        {
            if (value == null) return defualtDesc;

            return GetDescription(value) ?? defualtDesc;
        }

        /// <summary>
        /// 获取枚举的描述文本
        /// </summary>
        /// <param name="e">枚举成员</param>
        /// <returns></returns>
        public static string GetDescription(object e)
        {
            //获取字段信息
            System.Reflection.FieldInfo[] ms = e.GetType().GetFields();
            Type t = e.GetType();
            foreach (System.Reflection.FieldInfo f in ms)
            {
                //判断名称是否相等
                if (f.Name != e.ToString()) continue;
                //反射出自定义属性
                foreach (Attribute attr in f.GetCustomAttributes(true))
                {
                    //类型转换找到一个Description，用Description作为成员名称
                    DescriptionAttribute dscript = attr as DescriptionAttribute;
                    if (dscript != null)
                        return dscript.Description;
                }
            }
            //如果没有检测到合适的注释，则用默认名称
            return e.ToString();
        }
        /// <summary>
        /// 根据名字得到描述文本
        /// </summary>
        public static string GetDescription(Type enumType, string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                foreach (object etype in Enum.GetValues(enumType))
                {
                    if (etype.ToString() == name) return GetDescription(etype);
                }
            }
            return name;
        }

        /// <summary>
        /// 根据枚举值名字 获取此枚举的描述文本
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="name">枚举值</param>
        /// <param name="isMatchCase">是否匹配大小写</param>
        /// <returns></returns>
        public static string GetDescription(Type enumType, string enumName, bool isMatchCase)
        {
            if (!string.IsNullOrWhiteSpace(enumName))
            {
                // 遍历每一项枚举值
                foreach (object etype in Enum.GetValues(enumType))
                {
                    // 是否匹配大小写
                    if (isMatchCase)
                    {
                        // 完全相等，返回描述文本
                        if (etype.ToString() == enumName) return GetDescription(etype);
                    }
                    // 不匹配大小写
                    else
                    {
                        // 统一转小写后再比较
                        if (etype.ToString()?.ToLower() == enumName?.ToLower()) return GetDescription(etype);
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 根据描述文本验证是否存在枚举项，并返回枚举值名字
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="validationDescription">要验证的描述文本</param>
        /// <param name="resultEnumValue">枚举值名字</param>
        /// <returns></returns>
        public static bool IsDefinedByDescription(Type enumType, string validationDescription, out string resultEnumValue)
        {
            // 默认不存在
            bool isDefined = false;

            resultEnumValue = string.Empty;

            if (!string.IsNullOrWhiteSpace(validationDescription))
            {
                // 遍历每一项枚举值
                foreach (object etype in Enum.GetValues(enumType))
                {
                    // 获取枚举字段信息
                    FieldInfo fi = etype.GetType().GetField(etype.ToString());
                    if (fi == null)
                    {
                        return false;
                    }

                    // 获取枚举项的描述信息特性
                    DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (attributes == null || attributes.Length < 1)
                    {
                        return false;
                    }
                    // 取出描述信息
                    string description = attributes[0].Description;
                    // 如果匹配
                    if (description == validationDescription)
                    {
                        resultEnumValue = etype.ToString();
                        isDefined = true;
                        return isDefined;
                    }
                }
            }
            return isDefined;
        }


        /// <summary>
        /// 根据值得到描述文本
        /// </summary>
        public static string GetDescription(Type enumType, int? value)
        {
            if (value.HasValue)
            {
                foreach (object etype in Enum.GetValues(enumType))
                {
                    if ((int)etype == value.Value) return GetDescription(etype);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 根据值得到名字
        /// </summary>
        public static string GetValueToName(Type enumType, int? value)
        {
            if (value.HasValue)
            {
                foreach (object etype in Enum.GetValues(enumType))
                {
                    if ((int)etype == value.Value) return etype.ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 拼接的字符串获取对应的枚举描述
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <param name="split">分割字符默认是','</param>
        /// <returns></returns>
        public static string GetDescription(Type enumType, string value, char split = ',')
        {
            List<string> str = new List<string>();
            if (!string.IsNullOrWhiteSpace(value))
            {
                var valueList = value.Split(split);
                foreach (var item in valueList)
                {
                    int newItem = 0;
                    foreach (object etype in Enum.GetValues(enumType))
                    {
                        if (int.TryParse(item, out newItem))
                        {
                            if ((int)etype == newItem) str.Add(GetDescription(etype));
                        }
                        else
                        {
                            if (etype.ToString() == item) str.Add(GetDescription(etype));
                        }
                    }
                }
            }

            return string.Join(split.ToString(), str);

        }
        /// <summary>
        /// 根据值得到描述文本
        /// </summary>
        public static string GetDescription(Type enumType, long? value)
        {
            if (value.HasValue)
            {
                foreach (object etype in Enum.GetValues(enumType))
                {
                    if (Convert.ToInt64(etype) == value.Value) return GetDescription(etype);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 根据描述文本得到值
        /// </summary>
        /// <param name="enumType">typeof()</param>
        /// <param name="description">描述文本</param>
        /// <param name="isValidityCheck">是否验证描述包含在枚举内</param>
        /// <param name="checkEmpty">是否检查空</param>
        /// <returns></returns>
        public static int? GetValue(Type enumType, string description, bool isValidityCheck = false, bool checkEmpty = false)
        {
            var topAttr = ((DescriptionAttribute)Attribute.GetCustomAttribute(enumType, typeof(DescriptionAttribute)));
            string topDescription = (topAttr == null ? enumType.Name : topAttr.Description);

            if (string.IsNullOrWhiteSpace(description))
            {
                if (checkEmpty) throw new Exception(string.Format("{0}不能为空", topDescription));
            }
            else
            {
                foreach (object etype in Enum.GetValues(enumType))
                {
                    if (GetDescription(etype) == description || etype.ToString().Equals(description, StringComparison.CurrentCultureIgnoreCase) || ((int)etype).ToString() == description)
                        return (int)etype;
                }

                if (isValidityCheck) throw new Exception(string.Format("{0}错误[{1}]", topDescription, description));
            }
            return null;
        }
        /// <summary>
        /// 根据描述文本得到名字
        /// </summary>
        public static string GetName(Type enumType, string description, bool isValidityCheck = false, bool checkEmpty = false)
        {
            var topAttr = ((DescriptionAttribute)Attribute.GetCustomAttribute(enumType, typeof(DescriptionAttribute)));
            string topDescription = (topAttr == null ? enumType.Name : topAttr.Description);

            if (string.IsNullOrWhiteSpace(description))
            {
                if (checkEmpty) throw new Exception(string.Format("{0}不能为空", topDescription));
            }
            else
            {
                foreach (object etype in Enum.GetValues(enumType))
                {
                    if (GetDescription(etype) == description || etype.ToString().Equals(description, StringComparison.CurrentCultureIgnoreCase) || ((int)etype).ToString() == description)
                        return etype.ToString();
                }

                if (isValidityCheck) throw new Exception(string.Format("{0}错误[{1}]", topDescription, description));
            }
            return description;
        }
        /// <summary>
        ///  把枚举的描述和值绑定到DropDownList
        /// </summary>
        /// <param name="isNameValue">是否使用枚举名做为值(value)</param>
        /// <param name="isNameText">是否使用枚举名做为显示值(text)</param>
        /// <param name="isDescriptionValue">是否使用描述做为值(value)</param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetList(Type enumType, string emptyKey, string emptyValue, bool isNameValue = false, bool isNameText = false, bool isDescriptionValue = false)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

            if (emptyKey != null) result.Add(new KeyValuePair<string, string>(emptyKey, emptyValue));

            foreach (object e in Enum.GetValues(enumType))
            {
                string valueStr = isDescriptionValue ? GetDescription(e) : (isNameValue ? e.ToString() : ((int)e).ToString());
                result.Add(new KeyValuePair<string, string>(valueStr, (isNameText ? e.ToString() : GetDescription(e))));
            }
            return result;
        }

        /// <summary>
        /// 下拉
        /// </summary>
        /// <param name="enumType">枚举</param>
        /// <param name="emptyKey">key</param>
        /// <param name="emptyValue">value</param>
        /// <param name="isNameValue">默认值value</param>
        /// <param name="isNameText">默认值text</param>
        /// <param name="onlyNormal">是否查询过期的枚举</param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> NotGetObsoleteAttributeList(Type enumType, string emptyKey, string emptyValue, bool isNameValue = false, bool isNameText = false, bool? onlyNormal = null)
        {
            var result = new List<KeyValuePair<string, string>>();
            if (emptyKey != null) result.Add(new KeyValuePair<string, string>(emptyKey, emptyValue));

            if (!enumType.IsEnum) return result;

            var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                var descAttr = field.GetCustomAttribute<DescriptionAttribute>();
                var desc = descAttr?.Description;
                var value = (isNameValue ? field.Name : ((int)field.GetValue(null)).ToString());

                if (onlyNormal == null)
                {
                    result.Add(new KeyValuePair<string, string>(value, (isNameText ? field.Name : desc)));
                }
                else
                {
                    var isNormal = field.GetCustomAttribute<ObsoleteAttribute>() == null;
                    if ((onlyNormal.Value && isNormal) ||
                       (!onlyNormal.Value && !isNormal))
                    {
                        result.Add(new KeyValuePair<string, string>(value, (isNameText ? field.Name : desc)));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// /把枚举的描述和值绑定到DropDownList  主要用于产品大分类
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="emptyKey"></param>
        /// <param name="emptyValue"></param>
        /// <param name="isNameValue"></param>
        /// <param name="isNameText"></param>
        /// <param name="isDescriptionValue"></param>
        /// <param name="onlyNormal"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetMainCategaryList(Type enumType, string emptyKey, string emptyValue, bool isNameValue = false, bool isNameText = false, bool isDescriptionValue = false, bool? onlyNormal = null)
        {
            var result = new List<KeyValuePair<string, string>>();
            if (emptyKey != null) result.Add(new KeyValuePair<string, string>(emptyKey, emptyValue));

            if (!enumType.IsEnum) return result;

            var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                var descAttr = field.GetCustomAttribute<DescriptionAttribute>();
                var desc = descAttr?.Description;
                var value = isDescriptionValue ? desc : (isNameValue ? field.Name : ((int)field.GetValue(null)).ToString());

                if (onlyNormal == null)
                {
                    result.Add(new KeyValuePair<string, string>(value, (isNameText ? field.Name : desc)));
                }
                else
                {
                    var isNormal = field.GetCustomAttribute<ObsoleteAttribute>() == null;
                    if ((onlyNormal.Value && isNormal) ||
                       (!onlyNormal.Value && !isNormal))
                    {
                        result.Add(new KeyValuePair<string, string>(value, (isNameText ? field.Name : desc)));
                    }
                }
            }
            return result;
        }
        public static List<KeyValuePair<string, string>> GetList(Type enumType, bool includeEmpty = false, bool isNameValue = false)
        {
            if (includeEmpty)
            {
                return GetList(enumType, "", "", isNameValue);
            }
            else
            {
                return GetList(enumType, null, null, isNameValue);
            }
        }

        /// <summary>
        ///  把枚举的描述和值绑定到DropDownList 下拉框text显示为 text(value)
        ///  TQ200109005：要求站点下拉框text显示为 text(value)
        /// </summary>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetTextValueList(Type enumType, string emptyKey, string emptyValue)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

            if (emptyKey != null) result.Add(new KeyValuePair<string, string>(emptyKey, emptyValue));

            foreach (object e in Enum.GetValues(enumType))
            {
                result.Add(new KeyValuePair<string, string>(e.ToString(), $"{GetDescription(e)}({ e.ToString() })"));
            }
            return result;
        }


        public static TEnum Parse<TEnum>(string value)
            where TEnum : struct
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsEnum) throw new InvalidOperationException("不是枚举类型");

            return (TEnum)Enum.Parse(enumType, value);
        }

        public static TEnum ParseOrDefaultIfFail<TEnum>(string value, TEnum defaultValue)
            where TEnum : struct
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsEnum) throw new InvalidOperationException("不是枚举类型");

            if (Enum.TryParse(value, out TEnum @enum))
            {
                return @enum;
            }

            return defaultValue;
        }

        public static TEnum? ParseOrNullIfFail<TEnum>(string value)
           where TEnum : struct
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsEnum) throw new InvalidOperationException("不是枚举类型");

            if (Enum.TryParse(value, out TEnum @enum))
            {
                return @enum;
            }

            return null;
        }

        public static Dictionary<int, string> GetEnumValuesAndDescriptions<T>()
        {
            Type enumType = typeof(T);
            if (enumType.BaseType != typeof(Enum))
            {
                throw new ArgumentException("T is not System.Enum");
            }

            var enumValDic = new Dictionary<int, string>();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                var fi = e.GetType().GetField(e.ToString());
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                enumValDic.Add((int)e, (attributes.Length > 0) ? attributes[0].Description : e.ToString());
            }

            return enumValDic;
        }

        /// <summary>
        /// 获取枚举的描述信息集合
        /// </summary>
        /// <param name="enumType">枚举</param>
        /// <returns></returns>
        public static List<string> GetDescriptionList(Type enumType)
        {
            List<string> desctiptionList = new List<string>();
            foreach (object etype in Enum.GetValues(enumType))
            {
                desctiptionList.Add(GetDescription(etype));
            }
            return desctiptionList;
        }

        /// <summary>
        /// 位枚举
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static TEnum[] ParseBit<TEnum>(int value) where TEnum : struct
        {
            var type = typeof(TEnum);
            var enumList = new List<TEnum>();
            var enumValues = Enum.GetValues(type);
            foreach (var enumValue in enumValues)
            {
                if (((int)enumValue & value) == (int)enumValue)
                {
                    var enumItem = Parse<TEnum>(enumValue.ToString());
                    enumList.Add(enumItem);
                }
            }

            return enumList.ToArray();
        }

        /// <summary>
        /// 获取枚举的值信息集合
        /// </summary>
        /// <param name="enumType">枚举</param>
        /// <returns></returns>
        public static List<int> GetValueList(Type enumType)
        {
            List<int> valueList = new List<int>();
            foreach (object etype in Enum.GetValues(enumType))
            {
                valueList.Add(((int)etype));
            }
            return valueList;
        }

        /// <summary>
        /// 枚举是否相等
        /// </summary>
        /// <param name="source">枚举</param>
        /// <param name="enumValue">枚举值</param>
        /// <returns></returns>
        public static bool IsEquals(this Enum source,int? enumValue)=> Convert.ToInt64(source) == enumValue;
    }
}
