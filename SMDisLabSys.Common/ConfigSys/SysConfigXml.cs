using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace SMDisLabSys.Common.ConfigSys
{
    [Serializable]
    public  class SysConfigXml
    {
        private StartUpParam startUpConfig = new StartUpParam();
        //系统启动相关配置
        [XmlElement("StartUpParam")]
        public StartUpParam StartUpConfig { get => startUpConfig; set => startUpConfig = value; }
    }
}
