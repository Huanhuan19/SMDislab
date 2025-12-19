using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SMDisLabSys.Common
{
    public class JsonFileMgr
    {
        private static readonly object obj_lock = new object();

        /// <summary>
        /// 对象转换成json文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="type"></param>
        public static void JsonFileCreate<T>(T data, string path, string sname)
        {
            try
            {
                lock (obj_lock)
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                    string json = JsonConvert.SerializeObject(data, settings);
                    //string path = AppDomain.CurrentDomain.BaseDirectory + folderName;
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    path += "\\" + sname + ".json";
                    StreamWriter sw = new StreamWriter(path, false);
                    sw.Write(json);
                    sw.Close();
                }

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 读取json文件并转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T JsonFileReadAndConvert<T>(string folderName, string sname)
        {
            try
            {
                lock (obj_lock)
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + $"{folderName}\\" + sname + ".json";
                    if (File.Exists(path))
                    {
                        string json = File.ReadAllText(path);
                        return JsonConvert.DeserializeObject<T>(json);
                    }
                    else
                    {
                        MessageBox.Show($"没有找到文件  路径{path}");
                        return default(T);
                    }
                }

            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        /// <summary>
        /// 读取json文件并转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T JsonFileReadAndConvert<T>(string path)
        {
            try
            {
                lock (obj_lock)
                {
                    if (File.Exists(path))
                    {
                        string json = File.ReadAllText(path);
                        return JsonConvert.DeserializeObject<T>(json);
                    }
                    else
                    {
                        MessageBox.Show($"没有找到文件  路径{path}");
                        return default(T);
                    }
                }

            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        /// <summary>
        /// 读取json文件并转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static async Task<T> JsonFileReadAndConvertAsync<T>(Type type)
        {
            try
            {
                string sname = type.Name;
                string path = AppDomain.CurrentDomain.BaseDirectory + "OfflineFiles\\Json\\" + sname + ".json";
                if (File.Exists(path))
                {
                    string json = await File.ReadAllTextAsync(path);
                    return JsonConvert.DeserializeObject<T>(json);
                }
                else
                {
                    return default(T);
                }

            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
    }
}
