using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMDisLabSys.Common.DataConvert
{
    public class ClassDataConvert
    {
        public static ClassDataConvert Instance=new ClassDataConvert ();
        // 假定输入类于与返回类具有完全相同的字段
        public T CopySameFieldsObject<T>(Object source)
        {
            Type _SrcT = source.GetType();
            Type _DestT = typeof(T);

            // 构造一个要转换对象实例
            Object _Instance = _DestT.InvokeMember("", BindingFlags.CreateInstance, null, null, null);

            // 这里指定搜索所有公开和非公开的字段
            FieldInfo[] _SrcFields = _SrcT.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // 将源头对象的每个字段的值分别赋值到转换对象里，因为假定字段都一样，这里就不做容错处理了
            foreach (FieldInfo field in _SrcFields)
            {
                _DestT.GetField(field.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).
                    SetValue(_Instance, field.GetValue(source));
            }

            return (T)_Instance;
        }

        // 假定输入List 类于与返回类具有完全相同的字段

        public List<T> CopySameClassList<T>(List<object> sourceList)
        {
            List<T> instanceList = new List<T>();
            foreach (object item in sourceList)
            {
                instanceList.Add(CopySameFieldsObject<T>(item));
            }
            return instanceList;
        }
    }
}
