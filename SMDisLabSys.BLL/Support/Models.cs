using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMDisLabSys.BLL
{
    public class SenserMsg
    {
        public byte index;
        public byte sensorID;
        public string connectName;
        public float Power;
        public BluetoothInfo bluetooth;
    }
}
