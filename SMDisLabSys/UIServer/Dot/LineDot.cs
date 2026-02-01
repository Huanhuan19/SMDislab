using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace SMDisLabSys.UIServer.Dot
{
    public class LineDataDOT : BindableBase
    {
        public int Index { get; set; }
        string danBaiLinelong;
        /// <summary>
        /// 摆长
        /// </summary>
        /// 
        public string DanBaiLinelong
        {
            get { return danBaiLinelong; }
            set { SetProperty(ref danBaiLinelong, value); }
        }
        string danBaiCircle;
        /// <summary>
        /// 周期
        /// </summary>
        /// 
        public string DanBaiCircle
        {
            get { return danBaiCircle; }
            set { SetProperty(ref danBaiCircle, value); }
        }
        string zhonglig;
        /// <summary>
        /// 重力g
        /// </summary>
        /// 
        public string Zhonglig
        {
            get { return zhonglig; }
            set { SetProperty(ref zhonglig, value); }
        }
        string danBaiCircle2;
        /// <summary>
        /// 周期的平方
        /// </summary>
        /// 
        public string DanBaiCircle2
        {
            get { return danBaiCircle2; }
            set { SetProperty(ref danBaiCircle2, value); }
        }
    }
}
