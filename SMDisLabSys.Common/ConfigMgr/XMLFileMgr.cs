using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SMDisLabSys.Common.ConfigMgr
{
    
    /// <summary>
    /// 
    /// </summary>
    public class XMLFileMgr
    {
        /// <summary>
        /// Serializes the specified full path.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="type">The type.</param>
        /// <param name="append">if set to <c>true</c> [append].</param>
        public void Serialize(string fullPath, object obj, Type type, bool append)
        {
            if (!File.Exists(fullPath) && append)
            {
                return;
            }

            try
            {
                XmlSerializerNamespaces sn = new XmlSerializerNamespaces();
                sn.Add(string.Empty, string.Empty);

                XmlSerializer seri = new XmlSerializer(type);

                TextWriter sw = new StreamWriter(fullPath);
                seri.Serialize(sw, obj, sn);

                sw.Close();
                sw.Dispose();
            }
            catch (Exception ex)
            {
                //LogMgr.Instance.Error("XMLFileDAL序列化XML文件Serialize-4异常", ex);
            }
        }

        /// <summary>
        /// Serializes the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="type">The type.</param>
        /// <param name="append">if set to <c>true</c> [append].</param>
        /// <returns></returns>
        public bool Serialize(string path, string fileName, object obj, Type type, bool append)
        {
            if (fileName == null || fileName.Equals(string.Empty))
            {
                return false;
            }

            string fullPath = string.Empty;

            try
            {
                fullPath = Path.Combine(path, fileName);
            }
            catch (Exception ex)
            {
                //LogMgr.Instance.Error("XMLFileDAL文件路径组合异常", ex);
                return false;
            }

            if (!File.Exists(fullPath) && append)
            {
                return false;
            }

            try
            {
                XmlSerializerNamespaces sn = new XmlSerializerNamespaces();
                sn.Add(string.Empty, string.Empty);

                XmlSerializer seri = new XmlSerializer(type);

                TextWriter sw = new StreamWriter(fullPath, append);
                seri.Serialize(sw, obj, sn);
                sw.Close();
                sw.Dispose();
            }
            catch (Exception ex)
            {
                //LogMgr.Instance.Error("XMLFileDAL序列化XML文件Serialize-5异常", ex);
                return false;
            }

            return true;
        }

        public void SerializeArrayList<T>(string fullPath, object obj, Type type, bool append) where T : class
        {
            if (!File.Exists(fullPath) && append)
            {
                return;
            }

            try
            {
                XmlSerializerNamespaces sn = new XmlSerializerNamespaces();
                sn.Add(string.Empty, string.Empty);

                XmlSerializer seri = new XmlSerializer(typeof(ArrayList), new Type[] { type });

                TextWriter sw = new StreamWriter(fullPath);
                seri.Serialize(sw, obj, sn);

                sw.Close();
                sw.Dispose();
            }
            catch (Exception ex)
            {
                //LogMgr.Instance.Error("XMLFileDAL序列化ArrayList至XML文件SerializeArrayList异常", ex);
            }
        }

        /// <summary>
        /// Deserializes the specified full path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fullPath">The full path.</param>
        /// <param name="typeofT">The typeof T.</param>
        /// <returns></returns>
        public T Deserialize<T>(string fullPath, Type typeofT) where T : class
        {
            if (!File.Exists(fullPath))
            {
                return null;
            }

            XmlSerializerNamespaces sn = new XmlSerializerNamespaces();
            sn.Add(string.Empty, string.Empty);

            XmlSerializer seri = new XmlSerializer(typeofT);
            TextReader sw = null;
            T obj = null;
            try
            {
                sw = new StreamReader(fullPath);
                obj = seri.Deserialize(sw) as T;
            }
            catch (Exception ex)
            {
                //LogMgr.Instance.Error("XMLFileDAL反序列化XML文件T-Deserialize异常", ex);
                sw.Close();
                sw = new StreamReader(fullPath, Encoding.Default);
                obj = seri.Deserialize(sw) as T;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }

            return obj;
        }

        public T DeserializeArrayList<T>(string fullPath, Type typeofT) where T : class
        {
            if (!File.Exists(fullPath))
            {
                return null;
            }

            XmlSerializerNamespaces sn = new XmlSerializerNamespaces();
            sn.Add(string.Empty, string.Empty);

            XmlSerializer seri = new XmlSerializer(typeof(ArrayList), new Type[] { typeofT });
            TextReader sw = null;
            T obj = null;
            try
            {
                sw = new StreamReader(fullPath);
                obj = seri.Deserialize(sw) as T;
            }
            catch (Exception ex)
            {
                //LogMgr.Instance.Error("XMLFileDAL反序列化XML文件至ArrayList的Y-DeserializeArrayList异常", ex);
                sw.Close();
                sw = new StreamReader(fullPath, Encoding.Default);
                obj = seri.Deserialize(sw) as T;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }

            return obj;
        }

        /// <summary>
        /// Des the serialize.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileType">Type of the file.</param>
        /// <returns></returns>
        public object DeSerialize(string path, string fileName, Type fileType)
        {
            if (fileName == null)
            {
                return null;
            }

            try
            {
                path = Path.Combine(path, fileName);
            }
            catch (Exception ex)
            {
                //LogMgr.Instance.Error("XMLFileDAL的DeSerialize文件路径组合异常", ex);
                return false;
            }

            return this.Deserialize<object>(path, fileType);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public T Deserialize<T>(string s)
        {
            XmlDocument xdoc = new XmlDocument();
            try
            {
                xdoc.LoadXml(s);
                XmlNodeReader reader = new XmlNodeReader(xdoc.DocumentElement);
                XmlSerializer ser = new XmlSerializer(typeof(T));
                object obj = ser.Deserialize(reader);
                return (T)obj;
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Serialize(Type type, object obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(type); //为序列化器指定序列化类型
            MemoryStream memStream = new MemoryStream();
            xmlSerializer.Serialize(memStream, obj); //为序列化器指定要被序列化的数据
            string xmlString = System.Text.Encoding.UTF8.GetString(memStream.ToArray());
            return xmlString;
        }

        /// <summary>
        /// Loads the XML.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public XmlDocument LoadXmlDoc(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(filePath);
            }
            catch (Exception ex)
            {
                //LogMgr.Instance.Error("XMLFileDAL的LoadXmlDoc加载XML文件异常", ex);
                return null;
            }

            return xmlDoc;
        }

        public bool SaveXmlDoc(XmlDocument xmlDoc, string filePath)
        {
            try
            {
                xmlDoc.Save(filePath);
            }
            catch (Exception ex)
            {
               // LogMgr.Instance.Error("XMLFileDAL的SaveXmlDoc保存XML文件异常", ex);
                return false;
            }

            return true;
        }
    }
}
