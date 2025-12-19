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
        /// <summary>
        /// 信源标识
        /// </summary>
        private string url = "http://121.36.38.19:8001/orbit/station/task/ctoeorbit1";
        [XmlElement("URL")]
        public string URL { get => url; set => url = value; }
        /// <summary>
        /// 信源标识
        /// </summary>
        private string originator = "FF000005";
        [XmlElement("Originator")]
        public string Originator { get => originator; set => originator = value; }
        /// <summary>
        /// 信宿标识
        /// </summary>
        private string recipient = "00080000";
        [XmlElement("Recipient")]
        public string Recipient { get => recipient; set => recipient = value; }
        /// <summary>
        /// 卫星ID
        /// </summary>
        private string satellite = "****";
        [XmlElement("Satellite")]
        public string Satellite { get => satellite; set => satellite = value; }
        /// <summary>
        /// 历元时间
        /// </summary>
        private string epoch = "2020-12-08T00:01:04.000Z";
        [XmlElement("Epoch")]
        public string Epoch { get => epoch; set => epoch = value; }
        /// <summary>
        /// 轨道半长径(米)
        /// </summary>
        private double a = 7020663.723772;
        [XmlElement("A")]
        public double A { get => a; set => a = value; }
        /// <summary>
        ///轨道偏心率
        /// </summary>
        private double e = 0.0020749760927743;
        [XmlElement("E")]
        public double E { get => e; set => e = value; }
        /// <summary>
        ///轨道倾角(度)
        /// </summary>
        private double i = 86.295098804415;
        [XmlElement("I")]
        public double I { get => i; set => i = value; }
        /// <summary>
        /// 升交点赤经(度)
        /// </summary>
        private double o = 298.15732426765;
        [XmlElement("O")]
        public double O { get => o; set => o = value; }
        /// <summary>
        /// 近地点幅角(度)
        /// </summary>
        private double w = 159.70269157965;
        [XmlElement("W")]
        public double W { get => w; set => w = value; }
        /// <summary>
        /// 平近点角(度)
        /// </summary>
        private double m = 241.59626117124;
        [XmlElement("M")]
        public double M { get => m; set => m = value; }
        /// <summary>
        /// 大气阻尼系数
        /// </summary>
        private double cd = 0.12323047625371;
        [XmlElement("Cd")]
        public double Cd { get => cd; set => cd = value; }
        private bool isSetCd = false;
        [XmlElement("IsSetCd")]
        public bool IsSetCd { get => isSetCd; set => isSetCd = value; }
        /// <summary>
        /// 光压反射系数
        /// </summary>
        private double cr = 0;
        [XmlElement("Cr")]
        public double Cr { get => cr; set => cr = value; }
        private bool isSetCr = false;
        [XmlElement("IsSetCr")]
        public bool IsSetCr { get => isSetCr; set => isSetCr = value; }

    }
}
