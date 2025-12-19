using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMDisLabSys.Common.DataConvert
{
    public class DataCheck
    {
        public static DataCheck Instance = new DataCheck();
        public byte CheckSum(byte[] value)
        {
            byte sum = 0;
            for (int i = 0; i < value.Length; i++)
            {
                sum += value[i];
            }
            return sum;
        }
    }
    public class DataBuilding
    {
        public static DataBuilding Instance = new DataBuilding();
        public void SetBitValue(ref byte value, int index, byte writevalue)
        {
            if (index > 7) throw new ArgumentOutOfRangeException("index"); //索引出错
                                                                           //获取数据窗口
            ushort writeWindow = 0;
            writeWindow += (ushort)(1 << index);//第一位

            writevalue = (byte)(writevalue << index);
            byte valueWindow = (byte)~writeWindow;
            writevalue = (byte)(writevalue & writeWindow);
            value = (byte)(value & valueWindow);
            value = (byte)(value | writevalue);
        }
        /// <summary>
        /// 3字节数据转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public byte[] ConvertToTwosComplement(int value)
        {
            if (value < -0x01000000 || value > 0x00FFFFFF)
            {
                throw new ArgumentException("Input value is out of range for 3-byte twos complement");
            }
            byte[] bytes = new byte[3];
            bytes[0] = (byte)((value >> 16) & 0xFF);
            bytes[1] = (byte)((value >> 8) & 0xFF);
            bytes[2] = (byte)(value & 0xFF);
            return bytes;
        }
    }
}
