using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;
namespace SMDisLabSys.BLL.SensorKeyValue
{
    public class KeyValue
    {
        public KeyValue()//默认构造函数。
        {
            Initialize();
        }
        public KeyValue(string tableName)//一个参数的构造函数传递的是名称。
        {
            Initialize(tableName);
        }
        void Initialize()
        {
            Initialize("KeyValue");
        }
        void Initialize(string tableName)//初始化。两列。
        {
            _t = new DataTable(tableName);
            _t.Columns.AddRange(new DataColumn[] { 
                new DataColumn( "Key",Type.GetType("System.String"),"",MappingType.Element ),
                new DataColumn( "Value",Type.GetType("System.String"),"",MappingType.Element )
            });
        }
        DataTable _t;
        public DataTable DataTable
        {
            get
            {
                return _t;
            }
        }
        public void Restore(DataTable source)//重新存储数据。
        {
            Clear();
            foreach (DataRow row in source.Rows)
            {
                DataRow r = _t.NewRow();
                r["Key"] = row["Key"].ToString();
                r["Value"] = row["Value"].ToString();
                _t.Rows.Add(r);
            }
        }
        public void Clear()
        {
            _t.Rows.Clear();
        }
        public void Add(string key, string value)
        {
            DataRow r = _t.NewRow();
            r["Key"] = key;
            r["Value"] = value;
            _t.Rows.Add(r);
        }
        public void Replace(string key, string value)//如果没有数据进行存储，如果有则进行覆盖。
        {
            int index = GetIndexByKey(key);
            if (index >= 0)
            {
                DataRow r = _t.Rows[index];
                r["Value"] = value;
            }
            else
            {
                Add(key, value);
            }
        }
        public void Remove(string key)//移除key值那行。
        {
            int index = GetIndexByKey(key);
            while (index >= 0)
            {
                _t.Rows.RemoveAt(index);
                index = GetIndexByKey(key);
            }
        }
        public int Count//行数。
        {
            get
            {
                return _t.Rows.Count;
            }
        }
        public string GetValueByKey(string key)//跟GetIndexByKey的方法一样。
        {
            int index = GetIndexByKey(key);
            string value = string.Empty;
            if (index >= 0)
            {
                value = _t.Rows[index]["Value"].ToString();
            }
            return value;
        }
        public int GetIndexByKey(string key)//查询key键并且返回索引值。
        {
            int index = -1;
            for (int i = 0; i < _t.Rows.Count; i++)
            {
                if (_t.Rows[i]["Key"].ToString() == key)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
        public bool ContainsKey(string key)//是否包含传感器。
        {
            return GetIndexByKey(key) >= 0;
        }
        /// <summary>
        /// 得到DataTable Xml 序列化后的字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return SeiralizeDataTable(_t);
        }
        /// <summary>
        /// 从Xml 序列化的字符串得到DataTable
        /// </summary>
        /// <param name="value"></param>
        public void Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                _t.Clear();
            }
            else
            {
                _t = DeserializeDataTable(value);
            }
        }
        public static string SeiralizeDataTable(DataTable t)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb);
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
            serializer.Serialize(writer, t);
            writer.Close();
            return sb.ToString();
        }

        public static DataTable DeserializeDataTable(string pXml)
        {
            DataTable t;
            try
            {
                System.IO.StringReader sr = new System.IO.StringReader(pXml);
                XmlReader reader = XmlReader.Create(sr);
                XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
                t = serializer.Deserialize(reader) as DataTable;
            }
            catch
            {
                t = new DataTable("KeyValue"); 
                t.Columns.AddRange(new DataColumn[] { 
                    new DataColumn( "Key",Type.GetType("System.String"),"",MappingType.Element ),
                    new DataColumn( "Value",Type.GetType("System.String"),"",MappingType.Element )
                });

            }
            return t;
        }
        public static Color ParseColor(string value)
        {
            Color color = Color.Empty;
            int colorValue;
            if (int.TryParse(value, out colorValue))
            {
                color = Color.FromArgb(colorValue);
            }
            return color;

        }
        public static string ColorToString(Color color)
        {
            return color.ToArgb().ToString();
        }
        public static string ContentAlignmentToString(ContentAlignment alignment)
        {
            return alignment.ToString();
        }
        public static ContentAlignment ParseContentAlignment(string value)
        {
            ContentAlignment alignment = ContentAlignment.MiddleCenter;
            if (string.Equals(ContentAlignment.BottomCenter.ToString(), value))
            {
                alignment = ContentAlignment.BottomCenter;
            }
            else if (string.Equals(ContentAlignment.BottomLeft.ToString(), value))
            {
                alignment = ContentAlignment.BottomLeft;
            }
            else if (string.Equals(ContentAlignment.BottomRight.ToString(), value))
            {
                alignment = ContentAlignment.BottomRight;
            }
            else if (string.Equals(ContentAlignment.MiddleCenter.ToString(), value))
            {
                alignment = ContentAlignment.MiddleCenter;
            }
            else if (string.Equals(ContentAlignment.MiddleLeft.ToString(), value))
            {
                alignment = ContentAlignment.MiddleLeft;
            }
            else if (string.Equals(ContentAlignment.MiddleRight.ToString(), value))
            {
                alignment = ContentAlignment.MiddleRight;
            }
            else if (string.Equals(ContentAlignment.TopCenter.ToString(), value))
            {
                alignment = ContentAlignment.TopCenter;
            }
            else if (string.Equals(ContentAlignment.TopLeft.ToString(), value))
            {
                alignment = ContentAlignment.TopLeft;
            }
            else if (string.Equals(ContentAlignment.TopRight.ToString(), value))
            {
                alignment = ContentAlignment.TopRight;
            }
            return alignment;
        }
        public static string PointToStr(Point point)
        {
            return point.X.ToString() + "_" + point.Y.ToString();
        }
        public static Point ParsePoint(string value)
        {
            int x =0, y =0;
            string[] strList = value.Split('_');
            if (strList.Length == 2)
            {
                int.TryParse(strList[0], out x);
                int.TryParse(strList[1], out y);
            }
            return new Point(x, y);
        }
        public static string SizeToStr(Size size)
        {
            return size.Width.ToString() + "_" + size.Height.ToString();
        }
        public static Size ParseSize(string value)
        {
            int width = 0, height = 0;
            string[] strList = value.Split('_');
            if (strList.Length == 2)
            {
                int.TryParse(strList[0], out width);
                int.TryParse(strList[1], out height);
            }
            return new Size(width, height);
        }
        public static string PointFToStr(PointF point)
        {
            return point.X.ToString() + "_" + point.Y.ToString();
        }
        public static PointF ParsePointF(string value)
        {
            float x = 0, y = 0;
            string[] strList = value.Split('_');
            if (strList.Length == 2)
            {
                float.TryParse(strList[0], out x);
                float.TryParse(strList[1], out y);
            }
            return new PointF(x, y);
        }
        public static string SizeFToStr(SizeF size)
        {
            return size.Width.ToString() + "_" + size.Height.ToString();
        }
        public static SizeF ParseSizeF(string value)
        {
            float width = 0, height = 0;
            string[] strList = value.Split('_');
            if (strList.Length == 2)
            {
                float.TryParse(strList[0], out width);
                float.TryParse(strList[1], out height);
            }
            return new SizeF(width, height);
        }
    }
}
