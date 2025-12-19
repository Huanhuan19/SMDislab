using NPOI.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SMDisLabSys.Common.DataConvert
{
    public class BufferConvertHelper
    {
        public static uint BytesToUInt(byte[] dataBuffer)
        {
            uint value = 0;
            for (int i = 0; i < dataBuffer.Length; i++)
            {
                value |= (uint)(dataBuffer[i] << (i * 8));
            }
            return value;
        }
        public static int BytesToInt(byte[] dataBuffer)
        {
            int valueZX = 0;
            switch (dataBuffer.Length)
            {
                case 1:
                    valueZX = dataBuffer[0];
                    break;
                case 2:
                    {
                        valueZX = BitConverter.ToInt16(dataBuffer, 0);//有无符号 直接转化;
                    }
                    break;
                case 3:
                    {
                        valueZX = Bytes2Int(dataBuffer[2], dataBuffer[1], dataBuffer[0]);
                    }
                    break;
                case 4:
                    {
                        valueZX = BitConverter.ToInt32(dataBuffer, 0);
                    }
                    break;
            }
            return valueZX;
        }
        private static int Bytes2Int(byte b1, byte b2, byte b3)
        {
            int r = 0;
            byte b0 = 0xff;

            if ((b1 & 0x80) != 0) r |= b0 << 24;
            r |= b1 << 16;
            r |= b2 << 8;
            r |= b3;
            return r;
        }
        public static string BufferTo16Str(byte[] buffer)
        {
            string myresult = "";
            for (int i = 0; i < buffer.Length; i++)
                myresult += buffer[i].ToString("X2") + " ";
            return myresult;
        }
        public static string BufferTo16StrTrim(byte[] buffer)
        {
            string myresult = "";
            for (int i = 0; i < buffer.Length; i++)
                myresult += buffer[i].ToString("X2");
            return myresult;
        }
        public static byte[] HexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException("十六进制字符串的长度必须是偶数。");
            }
            byte[] byteArray = new byte[hexString.Length / 2];
            for (int i = 0; i < byteArray.Length; i++)
            {
                byteArray[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return byteArray;
        }
        public static float Read_float32(byte[] data, ref int index)
        {
            Unions u = new Unions();
            u.b0 = data[0];
            u.b1 = data[1];
            u.b2 = data[2];
            u.b3 = data[3];

            index += 4;
            return u.f;
        }

        public static float Read_float32(byte[] data)
        {
            Unions u = new Unions();
            u.b0 = data[0];
            u.b1 = data[1];
            u.b2 = data[2];
            u.b3 = data[3];

            return u.f;
        }
        public static byte read_Uint8(byte[] data, ref int index)
        {
            Uint8_Byte data_d = new Uint8_Byte();
            data_d.b0 = data[0];

            index += 1;
            return data_d.d;

        }

        public static byte[] VAR2BUFF_FLOAT32(float value)
        {
            byte[] data = new byte[4];
            float[] floatData = new float[1];
            floatData[0] = value;
            Buffer.BlockCopy(floatData, 0, data, 0, 4);
            return data;
        }
        public static byte[] VAR2BUFF_INT16(short value)
        {
            byte[] data = new byte[4];
            data = BitConverter.GetBytes(value);
            return data;
        }
        public static byte[] VAR2BUFF_UINT16(ushort value)
        {
            byte[] data = new byte[4];
            data = BitConverter.GetBytes(value);
            return data;
        }
        public static byte[] VAR2BUFF_INT32(int value)
        {
            byte[] data = new byte[4];
            data = BitConverter.GetBytes(value);
            return data;
        }
        public static byte[] VAR2BUFF_UINT32(uint value)
        {
            byte[] data = new byte[4];
            data = BitConverter.GetBytes(value);
            return data;
        }
        public static byte[] VAR2BUFF_LONG(long value, int byteCount)
        {
            byte[] data = new byte[byteCount];
            data = BitConverter.GetBytes(value);
            return data.Take(byteCount).ToArray();
        }
        public static byte[] VAR2BUFF_Short(short value)
        {
            byte[] data = new byte[2];
            data = BitConverter.GetBytes(value);
            return data.ToArray();
        }
        /// <summary>
        /// 根据buffer 字节数组   得到2进制字符串
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string GetDataBitStr(byte[] buffer)
        {
            BitArray arr = new BitArray(buffer);
            string dataBitStr = "";
            for (int i = 0; i < arr.Length; i++)
            {
                int counti = i / 8;
                dataBitStr += arr[counti * 8 + 7 - i % 8] == true ? "1" : "0";
            }
            return dataBitStr;
        }
        /// <summary>
        /// 根据2进制字符串  和起始位置 长度  获取数据
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="len"></param>
        /// <param name="dataBitStr"></param>
        /// <returns></returns>
        public static byte[] GetValueUseStartIndexAndLenFromBufferBitStr(int startIndex, int len, string dataBitStr)
        {
            List<byte> data = new List<byte>();
            string startb = "";
            //分段  整个字节前面的是startb 从前面开始取  如果是12时  会1234会取成 1203  所以从后面开始取 取完反转
            if (len <= 8)//一个字节的
            {
                startb = dataBitStr.Substring(startIndex, len);
                data.Add(Convert.ToByte(startb, 2));
            }
            else//多字节的
            {
                for (int i = startIndex + len; i > startIndex; i -= 8)
                {
                    //字节开始
                    if (i - 8 >= startIndex)
                    {
                        string b = dataBitStr.Substring(i - 8, 8);
                        data.Add(Convert.ToByte(b, 2));
                    }
                    else
                    {
                        string b = dataBitStr.Substring(startIndex, i - startIndex);
                        data.Add(Convert.ToByte(b, 2));
                    }
                }
            }
            data.Reverse();
            return data.ToArray();
        }

        public static byte[] GetValueUseStartIndexAndLenFromBuffer(int startIndex, int len, byte[] buffer)
        {
            BitArray arr = new BitArray(buffer);
            string dataBitStr = "";
            for (int i = 0; i < arr.Length; i++)
            {
                int counti = i / 8;
                dataBitStr += arr[counti * 8 + 7 - i % 8] == true ? "1" : "0";
            }

            List<byte> data = new List<byte>();
            string startb = "";
            //分段  整个字节前面的是startb 
            for (int i = startIndex; i < startIndex + len; i++)
            {
                //字节的开始处
                if (i % 8 == 0)
                {
                    if (i + 8 > startIndex + len)
                    {
                        string b = dataBitStr.Substring(i, startIndex + len - i);
                        data.Add(Convert.ToByte(b, 2));
                        break;
                    }
                    else
                    {
                        string b = dataBitStr.Substring(i, 8);
                        i = i + 7;//应该是 i+8   循环里面有i++ 所有是i+7;
                        data.Add(Convert.ToByte(b, 2));
                    }
                }
                //整个字节前面的是startb
                else
                {
                    startb += dataBitStr.Substring(i, 1);
                    if ((i + 1) % 8 == 0)//下一个是字节开始处时
                    {
                        data.Add(Convert.ToByte(startb, 2));
                    }
                }
            }
            if (data.Count == 0)//如果是空的  说明是没有跨字节
            {
                data.Add(Convert.ToByte(startb, 2));
            }
            return data.ToArray();
        }
    }



    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public struct Unions
    {
        [FieldOffset(0)]
        public Byte b0;
        [FieldOffset(1)]
        public Byte b1;
        [FieldOffset(2)]
        public Byte b2;
        [FieldOffset(3)]
        public Byte b3;
        [FieldOffset(0)]

        public float f;
    }

    
    [StructLayout(LayoutKind.Explicit, Size = 1)]
    public struct Uint8_Byte
    {
        [FieldOffset(0)]
        public Byte b0;
        [FieldOffset(0)]
        public byte d;
    }
}
