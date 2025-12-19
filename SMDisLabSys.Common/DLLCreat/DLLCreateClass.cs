using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMDisLabSys.Common.DLLCreat
{
    public class DLLCreateClass
    {
        public static bool Creat(string path)
        {
            try
            {
                string[] Name = path.ToString().Split(' ');
                string str = "";
                // 打开终端
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "cmd.exe\n";
                p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                p.Start();//启动程序

                //string str = "devenv \"" + path.ToString() + "\" /ReBuild \"Debug|x86\" /project \"DynamicSimulation\\DynamicSimulation.vcxproj\"";
                //string str = "devenv \"" + path.ToString() + "\" /ReBuild \"Debug|x86\" /project \"DynamicPackage\\Dynamic_Simulation.vcxproj\"";
                if (Name.Length == 1)
                    str = "devenv \"" + Name[0] + "\" /ReBuild \"Debug|x86\"";
                else if (Name.Length == 2)
                    str = "devenv \"" + Name[0] + "\" /ReBuild \"Debug|x86\" /project \"" + Name[1] + "\"";
                p.StandardInput.WriteLine(str + "&exit");//向cmd窗口发送输入信息
                p.StandardInput.AutoFlush = true;
                string output = p.StandardOutput.ReadToEnd();//获取cmd窗口的输出信息

                p.WaitForExit();//等待程序执行完退出进程
                p.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static string CreatReturnMsg(string path)
        {
            try
            {
                string[] Name = path.ToString().Split(' ');
                string str = "";
                // 打开终端
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "cmd.exe\n";
                p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                p.Start();//启动程序

                //string str = "devenv \"" + path.ToString() + "\" /ReBuild \"Debug|x86\" /project \"DynamicSimulation\\DynamicSimulation.vcxproj\"";
                //string str = "devenv \"" + path.ToString() + "\" /ReBuild \"Debug|x86\" /project \"DynamicPackage\\Dynamic_Simulation.vcxproj\"";
                if (Name.Length == 1)
                    str = "devenv \"" + Name[0] + "\" /ReBuild \"Debug|x86\"";
                else if (Name.Length == 2)
                    str = "devenv \"" + Name[0] + "\" /ReBuild \"Debug|x86\" /project \"" + Name[1] + "\"";
                p.StandardInput.WriteLine(str + "&exit");//向cmd窗口发送输入信息
                p.StandardInput.AutoFlush = true;
                string output = p.StandardOutput.ReadToEnd();//获取cmd窗口的输出信息

                p.WaitForExit();//等待程序执行完退出进程
                p.Close();
                return output;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
