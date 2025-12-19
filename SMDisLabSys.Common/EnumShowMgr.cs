using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMDisLabSys.Common
{
    public class EnumShow
    {
        private int enumValue;
        private string enumString;

        public int EnumValue { get => enumValue; set => enumValue = value; }
        public string EnumString { get => enumString; set => enumString = value; }
    }

    public class EnumShowMgr
    {
        public static List<EnumShow> GetEnumShowList(Type enumT)
        {
            List<EnumShow> lst = new List<EnumShow>();
            FieldInfo[] fields = enumT.GetFields(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static);
            foreach (FieldInfo fi in fields)
            {
                DescriptionAttribute da = Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (da != null)
                {
                    EnumShow show = new EnumShow();
                    show.EnumValue = (int)fi.GetValue(null);
                    show.EnumString = da.Description;
                    lst.Add(show);
                }
            }
            return lst;
        }

        /// <summary>
        /// 获取枚举Description值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetEnumShowInfo<T>(T enumValue)
        {
            FieldInfo field = typeof(T).GetField(enumValue.ToString());
            if (field == null) return "";
            DescriptionAttribute da = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (da == null) return "";
            else return da.Description;
        }

        //
        /// <summary>
        /// 根据值获取枚举方法名称
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Enum GetEnumByValue(Type enumType, string value)
        {
            return Enum.Parse(enumType, value) as Enum;
        }

        #region C# enum 根据 Description内容找到对应的枚举类型
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }
        public static T GetEnumValueFromDescription<T>(string description) where T : Enum
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException($"No matching description found for {description}");
        }
        #endregion

        public static Dictionary<int, string> EmunToDictionary<T>()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            var enumDictionary = new Dictionary<int, string>();
            var enumValues = Enum.GetValues(typeof(T));
            var enumNames = Enum.GetNames(typeof(T));
            for (int i = 0; i < enumValues.Length; i++)
            {
                enumDictionary.Add((int)enumValues.GetValue(i), enumNames[i]);
            }
            return enumDictionary;
        }

    }
}
