using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMDisLabSys.BLL.RealData;
using SMDisLabSys.BLL.SensorOper;
using SMDisLabSys.Common.DataConvert;
using SMDisLabSys.Model;
using Windows.AI.MachineLearning;
using static SMDisLabSys.BLL.RealData.RealDataBLE;

namespace SMDisLabSys.BLL.Protocol
{
    public class ParseHelper
    {
        public static ParseHelper Instance = new ParseHelper();
        byte[] header = new byte[2] { 0xCE, 0xCB };
        public void Parse(byte[] buffer, ConnectTypeEnum connectType, byte channel, string belAddr = "")
        {
            try
            {
                if (Enumerable.SequenceEqual(buffer.Take(2).ToArray(), header))//协议识别码
                {
                    DataParseEventArgs dataParseArgs = new DataParseEventArgs();
                    dataParseArgs.ConnectType = connectType;

                    int sensorId = BufferConvertHelper.BytesToInt(buffer.Skip(2).Take(2).Reverse().ToArray());//产品编号
                    byte sensors = buffer[10];
                    byte[] sensorPlaceholder = new byte[sensors];//传感器数据占有多少位

                    List<List<double>> sensorValue = new List<List<double>>();
                    var placeSensorData = BufferConvertHelper.BytesToInt(buffer.Skip(11).Take(2).Reverse().ToArray());//传感器数据占有多少位
                    short win = 0x3;
                    for (byte i = 0; i < sensors; i++)
                    {
                        if (i != 0)//第一次不移位
                        {
                            placeSensorData = (short)(placeSensorData >> (2));
                        }
                        sensorPlaceholder[i] = (byte)((placeSensorData & win) + 1);

                        sensorValue.Add(new List<double>());//初始化数据维度
                    }
                    var oneDataLen = sensorPlaceholder.Sum(b => (int)b);

                    int bufferskip = 0;
                    for (int i = 13; i < buffer.Length; i += oneDataLen)
                    {
                        for (int j = 0; j < sensorPlaceholder.Length; j++)
                        {
                            var K = GetSensorK(sensorId + j);
                            var skipj = j == 0 ? 0 : sensorPlaceholder[j - 1];//第一个数据没有 组内skip
                            bufferskip += skipj;
                            var data = buffer.Skip(i + bufferskip).Take(sensorPlaceholder[j]).Reverse().ToArray();
                            var value = BufferConvertHelper.BytesToInt(data);
                            if (K != 0)
                            {
                                var round = (int)Math.Log10((int)(1 / K));
                                sensorValue[j].Add(Math.Round(value * K, round));
                            }
                        }
                    }
                    for (int j = 0; j < sensorPlaceholder.Length; j++)
                    {
                        dataParseArgs.ParamListDic.Add(sensorId + j, sensorValue[j]);
                    }
                    dataParseArgs.BLEAddress = belAddr;
                    dataParseArgs.Channel = channel;

                    RealDataBLE.Instance.UpdataBLEData(dataParseArgs);
                }
            }
            catch (Exception exc)
            {

            }
        }

        double GetSensorK(int sensorId)
        {
            var sensor = SensorCollection.Instance.SensorDefines.Where(o => o.SensorID == sensorId).FirstOrDefault();
            if (sensor != null)
            {
                return sensor.K;
            }
            return 1;
        }
    }
}
