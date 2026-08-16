using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMDisLabSys.Common.ConfigMgr;
using SMDisLabSys.Common.ConfigSys;

namespace SMDisLabSys.BLL
{
    public class PublicStaticInfo
    {
        public static PublicStaticInfo Instance = new PublicStaticInfo();
        public List<string> VIDPIDList = new List<string>();
        public SysConfigXml SysConfig;//配置
        public void Init()
        {
            InitSysConfig();
            InitVidPid();
        }
        void InitSysConfig()
        {
            SysConfig = ConfigMgr.Instance.GetSystemConfig();
        }
        void InitVidPid()
        {
            VIDPIDList.Add("0x0483:0x5710");
            VIDPIDList.Add("0x1FC9:0xB");
            VIDPIDList.Add("0x17EF:0x60FF");
        }
    }
}
