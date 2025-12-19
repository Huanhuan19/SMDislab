using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    //ulong LpId = SetCpuID((int)lpIdx);
    //SetUseCUPNo.SetThreadAffinityMask(SetUseCUPNo.GetCurrentThread(), new UIntPtr(LpId));
    public class SetUseCUPNo
    {
        //  /获取系统运行时间毫秒级别
        [DllImport("kernel32.dll")]
        public static extern uint GetTickCount();


        //SetThreadAffinityMask 指定hThread 运行在 核心 dwThreadAffinityMask
        [DllImport("kernel32.dll")]
        public static extern UIntPtr SetThreadAffinityMask(IntPtr hThread,
        UIntPtr dwThreadAffinityMask);

        //得到当前线程的handler
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentThread();


        static ulong SetCpuID(int lpIdx)
        {
            ulong cpuLogicalProcessorId = 0;
            if (lpIdx < 0 || lpIdx >= System.Environment.ProcessorCount)
            {
                lpIdx = 0;
            }
            cpuLogicalProcessorId |= 1UL << lpIdx;
            return cpuLogicalProcessorId;
        }

    }
}
