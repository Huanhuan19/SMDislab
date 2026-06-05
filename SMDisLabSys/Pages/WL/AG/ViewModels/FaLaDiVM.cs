using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using NPOI.SS.Formula;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SMDisLabSys.BLL;
using SMDisLabSys.BLL.RealData;
using Windows.UI.Input.Inking;
using static SMDisLabSys.BLL.RealData.RealDataBLE;
using MessageBox = System.Windows.MessageBox;
using SMDisLabSys.BLL.Formulas;
using SMDisLabSys.UIServer.Caculator;
using SMDisLabSys.Model;
using SMDisLabSys.UIServer.Dot.WL;
using SMDisLabSys.UIServer;

namespace SMDisLabSys.Pages.WL.AG.ViewModels
{
    class FaLaDiVM : UIChartBase, IDialogAware
    {
        #region 属性
        double spead;
        public double Spead
        {
            get { return spead; }
            set { SetProperty(ref spead, value); }
        }
        double voltage = 0;
        public double Voltage
        {
            get { return voltage; }
            set { SetProperty(ref voltage, value); }
        }

        #endregion
        public FaLaDiVM()
        {
            XFormatter = XFormatters;
            YFormatter = YFormatters;

            InitCommand();
            InitLine();
            ConnectDevice();
        }
        void InitCommand()
        {
            RealDataBLE.Instance.BLEDataUpdated += Instance_BLEDataUpdated;
        }

        private void Instance_BLEDataUpdated(object? sender, EventArgs e)
        {
            DataParseEventArgs args = (DataParseEventArgs)e;
            List<double> value = new List<double>();
            args.ParamListDic.TryGetValue(20, out value);//传感器编号
            if (value != null && value.Count > 0)
            {
                Spead = value[0];
            }
            args.ParamListDic.TryGetValue(21, out value);
            if (value != null && value.Count > 0)
            {
                Voltage = value[0];
            }
            if (args.ConnectType==ConnectTypeEnum.USB)
            {
                haveUSBData = true;
            }
        }

        void InitLine()
        {
            
        }

        void ConnectDevice()
        {
            Thread.Sleep(500);//等有无USB数据
            if (haveUSBData)
            {
                return;
            }
            Task.Run(() =>
            {
                int loop = 100;
                while (loop > 0)//
                {
                    if (SMDataSource.Instance.BluetoothList.Count > 0)
                    {
                        List<BluetoothInfo> selectList = new List<BluetoothInfo>();
                        selectList.Add(SMDataSource.Instance.BluetoothList[0]);
                        SMDataSource.Instance.BluetoothConnect(selectList);
                        break;
                    }
                    Thread.Sleep(500);//50s

                    loop--;
                }
            });
        }

        #region IDialogAware接口实现
        string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
        int height;
        public int Height
        {
            get { return height; }
            set { SetProperty(ref height, value); }
        }
        int width;
        public int Width
        {
            get { return width; }
            set { SetProperty(ref width, value); }
        }
        public event Action<IDialogResult> RequestClose;
        public bool CanCloseDialog()
        {
            return true;
        }
        public void OnDialogClosed()
        {

        }
        public void OnDialogOpened(IDialogParameters parameters)
        {
            //if (parameters.ContainsKey("Title"))
            //{
            //    Title = (parameters.GetValue<string>("Title"));
            //}
            Title = "单摆运动规律演示器";
            Width = 1350;
            Height = 820;
        }
        #endregion
    }

}
