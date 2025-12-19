using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SMDisLabSys.Common.ConfigSys;

namespace SMDisLabSys.Common.ConfigMgr
{
    /// <summary>
    /// 配置管理，管理系统全部系统配置的读写
    /// </summary>
    public  class ConfigMgr
    {
        public static ConfigMgr Instance=new ConfigMgr ();
        private XMLFileMgr dal = new XMLFileMgr();


        public SysConfigXml GetSystemConfig()
        {
            string path= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SysConfig");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileName = "SysMainConfig.xml";
            string  fullPath = Path.Combine(path, fileName);
            if (!File.Exists(fullPath))
            {                
                SysConfigXml configXml = new SysConfigXml();
                this.SaveSystemConfig(configXml);
                return configXml;
            }
            SysConfigXml sysConf = (SysConfigXml)this.GetConfig(path, fileName,typeof(SysConfigXml));

            return sysConf;
        }


        public object GetConfig(string path,string fileName,Type fileType)
        {
            if (path == null || path == string.Empty)
            {
                return null;
            }

            return this.dal.DeSerialize(path, fileName, fileType);
        }

        public bool SaveSystemConfig(SysConfigXml config)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SysConfig");
            string fileName = "SysMainConfig.xml";
            string fullPath = Path.Combine(path, fileName);
            try
            {
                this.dal.Serialize(fullPath, config, typeof(SysConfigXml), false);
            }
            catch(Exception e)
            {
                return false;
            }
            return true;
        }

      

    }


}
