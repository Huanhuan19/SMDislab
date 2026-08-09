using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SMDisLabSys.Common.ConfigSys
{
    [Serializable]
    public class StartUpParam
    {
        //测试VID
        private string ceShiVID;
        [XmlElement("CeShiVID")]
        public string CeShiVID { get => ceShiVID; set => ceShiVID = value; }
        //测试PID
        private string ceShiPID;
        [XmlElement("CeShiPID")]
        public string CeShiPID { get => ceShiPID; set => ceShiPID = value; }
    }
}
