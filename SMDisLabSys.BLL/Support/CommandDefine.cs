using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMDisLabSys.BLL
{
    public enum CommandDefine : byte
    {
        None = 0x00, QueryType = 0x80, Stop = 0x81, Start = 0x82, ChannelControl = 0x83, ShiftSet = 0x84, QueryShift = 0x85, Wave = 0x86, OpenClose = 0xa8//增加的波形类型的命令0x86；
    }
}
