using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace SMDisLabSys.UIServer.Dot
{
    public class CheckBoxViewModel : BindableBase
    {
        /// <summary>
        /// 索引
        /// </summary>
        private string index;
        public string Index
        {
            get { return index; }
            set
            {
                SetProperty(ref index, value);
            }
        }
        /// <summary>
        /// 名称
        /// </summary>
        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                SetProperty(ref text, value);
            }
        }
        /// <summary>
        /// 是否选中
        /// </summary>
        private bool iChecked;
        public bool IsChecked
        {
            get { return iChecked; }
            set
            {
                SetProperty(ref iChecked, value);
            }
        }
    }
}
