using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMDisLabSys.Common
{
    public class LogMgr
    {
        public static readonly LogMgr Instance = new LogMgr();


        private static  log4net.ILog logger;



        public LogMgr()
        {
            string pathTemp = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(pathTemp, "SysConfig\\LogConfig.txt"); 
            System.IO.FileInfo fin = new System.IO.FileInfo(filePath);
            log4net.Config.XmlConfigurator.ConfigureAndWatch(fin); ////加载并实时监控配置文件的变更
            LogMgr.logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }


        public void Debug(string logMessage)
        {
            LogMgr.logger.Debug(this.DealLog(logMessage));
        }

        public void Info(string logMessage)
        {
            LogMgr.logger.Info(this.DealLog(logMessage));
        }

        public void Warn(string logMessage)
        {
            LogMgr.logger.Warn(this.DealLog(logMessage));
        }


        public void Error(string logMessage)
        {
 
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(logMessage);
            //错误信息，追加程序堆栈，用于后续分析
            sb.AppendLine("=== 堆栈信息：");
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            System.Diagnostics.StackFrame[] sfs = st.GetFrames();
            for (int u = 0; u < sfs.Length && u < 20; ++u)
            {
                System.Reflection.MethodBase mb = sfs[u].GetMethod();
                if (mb != null && mb.DeclaringType != null)
                {
                    sb.AppendLine(string.Format("[STACK][{0}]: {1}.{2}", u, mb.DeclaringType.FullName, mb.Name));
                }
            }
            sb.AppendLine("====");
            //=====
            LogMgr.logger.Error(this.DealLog(sb.ToString()));
        }


        public void Error(string logMessage, Exception ex)
        {
       
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(logMessage);
            //错误信息，追加程序堆栈，用于后续分析
            sb.AppendLine("=== 堆栈信息：");
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            System.Diagnostics.StackFrame[] sfs = st.GetFrames();
            for (int u = 0; u < sfs.Length && u < 10; ++u)
            {
                System.Reflection.MethodBase mb = sfs[u].GetMethod();
                if (mb != null && mb.DeclaringType != null)
                {
                    sb.AppendLine(string.Format("[STACK][{0}]: {1}.{2}", u, mb.DeclaringType.FullName, mb.Name));
                }
            }
            sb.AppendLine("====");
         
            LogMgr.logger.Error(this.DealLog(sb.ToString()), ex);
        }
        public void Fatal(string logMessage)
        {
            LogMgr.logger.Fatal(this.DealLog(logMessage));
        }

        private void Debug(string logMessage, Exception ex)
        {
            LogMgr.logger.Debug(this.DealLog(logMessage), ex);
        }

        private void Info(string logMessage, Exception ex)
        {
            LogMgr.logger.Info(this.DealLog(logMessage), ex);
        }

        private void Warn(string logMessage, Exception ex)
        {
            LogMgr.logger.Warn(this.DealLog(logMessage), ex);
        }

        public void Fatal(string logMessage, Exception ex)
        {
            LogMgr.logger.Fatal(this.DealLog(logMessage), ex);
        }

        //日志内容预处理
        //目前返回原内容，后续如出现多种部署场景，根据部署场景，可在日志信息追加特定标识
        private string DealLog(String log)
        {
            return log;
        }
    }
}
