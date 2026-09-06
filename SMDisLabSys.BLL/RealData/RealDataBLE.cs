using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using SMDisLabSys.Model;

namespace SMDisLabSys.BLL.RealData
{
    public class RealDataBLE
    {
        public static RealDataBLE Instance = new RealDataBLE();

        //接收消息页面回调事件
        public event EventHandler BLEDataUpdated;
        public void UpdataBLEData(DataParseEventArgs arg)
        {
            if (BLEDataUpdated != null)
            {
                BLEDataUpdated?.Invoke(this, arg);
            }
        }

        public class DataParseEventArgs : EventArgs
        {
            public string BLEAddress { get; set; }
            public byte Channel { get; set; } = 1;
            public ConnectTypeEnum ConnectType { get; set; }
            public Dictionary<int, List<double>> ParamListDic;
            public DataParseEventArgs()
            {
                ParamListDic = new Dictionary<int, List<double>>();
            }
        }
    }
}
