using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SMDisLabSys.Common
{
    public class FileHelper
    {
        public static void FileSave(string path, string contents)
        {
            if (!File.Exists(path))
            {
                File.Create(path);
            }
            System.IO.File.WriteAllText(path, contents, Encoding.GetEncoding("gb2312"));
        }
        public static string FileRead(string path)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show($"文件不存在 {path}");
            }
            return System.IO.File.ReadAllText(path, Encoding.GetEncoding("gb2312"));
        }
    }
}
