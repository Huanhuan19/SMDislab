using ICSharpCode.SharpZipLib.Checksum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMDisLabSys.Common.DataConvert
{
    public class Crc16
    {
        public static Crc16 Instance=new Crc16 ();
        private const ushort Polynomial = 0x1021;
        private static readonly ushort[] CrcTable = new ushort[256];
        static Crc16()
        {
            for (ushort i = 0; i < 256; i++)
            {
                ushort crc = (ushort)(i << 8);
                for (byte j = 0; j < 8; j++)
                {
                    crc = (ushort)((crc & 0x8000) != 0 ? (crc << 1) ^ Polynomial : crc << 1);
                }
                CrcTable[i] = crc;
            }
        }
        public byte[] CRCCalc(byte[] data)
        {
            ushort crc16 = 0xffff;
            for (int i = 0; i < data.Length; i++)
            {
                crc16 = (ushort)((crc16 << 8) ^ CrcTable[((crc16 >> 8) ^ data[i]) & 0x00FF]);
            }
            byte[] crc = new byte[2];
            crc[1] = (byte)(crc16 & 0xff);
            crc[0] = (byte)(crc16 >> 8 & 0xff);
            return crc;
        }
    }
}
