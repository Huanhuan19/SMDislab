using SMDisLabSys.Common;
using SMDisLabSys.Common.ConfigMgr;
using SMDisLabSys.Common.ExactTime;
using SMDisLabSys.Common.Excel;
using FATP.BackEnd.Common.ExcelHelper;
using Newtonsoft.Json;
using NPOI.HSSF.Record;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Resolution;
using Unity;
using System.Reflection;
using System.Runtime.InteropServices;
using SMDisLabSys.Common.DataConvert;
using NPOI.SS.Formula.Functions;
using System.Windows.Documents;
using System.Windows;
using MathNet.Numerics.Distributions;
using SMDisLabSys.BLL;

namespace SMDisLabSys
{
    internal class SystemInit
    {
        public static readonly SystemInit Instance = new SystemInit();
        public void Init()
        {
            SMDataSource.Instance.BLEStart();
        }
    }

}
