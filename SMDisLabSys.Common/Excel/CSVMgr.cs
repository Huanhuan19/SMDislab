using CsvHelper;
using NPOI.HSSF.Record;
using NPOI.OpenXmlFormats.Vml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace SMDisLabSys.Common.Excel
{
    public class CSVMgr
    {
        public static void WriteCSV<T>(List<string>Titles,List<T> records,string path)
        {

            //写CSV文件
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var writer = new StreamWriter(path,true, Encoding.GetEncoding("GB2312")))

            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                foreach (var item in Titles)
                {
                    csv.WriteField(item);
                }
                csv.NextRecord();
                foreach (var record in records)
                {
                    csv.WriteRecord(record);
                    csv.NextRecord();
                }
            }
            MessageBox.Show("文件导出完成");
        }
        public static List<T> ReadCSV<T>(string filePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var reader = new StreamReader(filePath, Encoding.GetEncoding("GB2312")))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var foos = csv.GetRecords<T>().ToList();
                return foos;
            }
            return null;
        }
        public static void WriteTitleCSV(List<string> Titles, string path)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //写CSV文件
            using (var writer = new StreamWriter(path, true, Encoding.GetEncoding("GB2312")))

            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                foreach (var item in Titles)
                {
                    csv.WriteField(item);
                }
                csv.NextRecord();
            }
        }
        public static void WriteDataCSV<T>(T record, string path)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //写CSV文件

            using (var writer = new StreamWriter(path, true, Encoding.GetEncoding("GB2312")))

            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecord(record);
                csv.NextRecord();
            }
        }
    }
}
