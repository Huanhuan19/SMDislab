using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMDisLabSys.BLL.RealData
{
    public class RealDataBLE
    {
        public static RealDataBLE Instance = new RealDataBLE();

        //接收消息页面回调事件
        public event EventHandler BLEDataUpdated;
        public void UpdataBLEData(DataParseBLEEventArgs arg)
        {
            if (BLEDataUpdated != null)
            {
                BLEDataUpdated?.Invoke(this, arg);
            }
        }

        public class DataParseBLEEventArgs : EventArgs
        {
            public string BLEAddress { get; set; }
            public List<double> ParamList;
            public DataParseBLEEventArgs()
            {
                ParamList = new List<double>();
            }
        }
    }
}
