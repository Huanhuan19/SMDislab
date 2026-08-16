using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using NPOI.SS.Formula;
using NPOI.SS.Formula.Functions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SMDisLabSys.BLL;
using SMDisLabSys.BLL.Formulas;
using SMDisLabSys.BLL.RealData;
using SMDisLabSys.Common.DataConvert;
using SMDisLabSys.Model;
using SMDisLabSys.UIServer;
using SMDisLabSys.UIServer.Caculator;
using SMDisLabSys.UIServer.Dot;
using SMDisLabSys.UIServer.Dot.WL;
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
using System.Windows.Threading;
using Windows.UI.Input.Inking;
using static SMDisLabSys.BLL.RealData.RealDataBLE;
using MessageBox = System.Windows.MessageBox;

namespace SMDisLabSys.Pages.WL.YZ.ViewModels
{
    class DataTestVM : UIChartBase, IDialogAware
    {
        DateTime dtCreat;
        double xAxis = 0;

        public DelegateCommand ClearSelectCommand { get; private set; }
        public DelegateCommand SendCommand { get; private set; }
        string BLEName = "SHM:200";

        #region 属性
        double data1;
        public double Data1
        {
            get { return data1; }
            set { SetProperty(ref data1, value); }
        }
        double data2;
        public double Data2
        {
            get { return data2; }
            set { SetProperty(ref data2, value); }
        }
        double data3;
        public double Data3
        {
            get { return data3; }
            set { SetProperty(ref data3, value); }
        }
        double data4;
        public double Data4
        {
            get { return data4; }
            set { SetProperty(ref data4, value); }
        }
        double data5;
        public double Data5
        {
            get { return data5; }
            set { SetProperty(ref data5, value); }
        }
        double data6;
        public double Data6
        {
            get { return data6; }
            set { SetProperty(ref data6, value); }
        }
        string sendStr;
        public string SendStr
        {
            get { return sendStr; }
            set { SetProperty(ref sendStr, value); }
        }



        #endregion
        public DataTestVM()
        {
            InitCommand();
            ConnectDevice();
        }
        void InitCommand()
        {
            RealDataBLE.Instance.BLEDataUpdated += Instance_BLEDataUpdated;

            ClearSelectCommand = new DelegateCommand(ClearSelectCommandMethod);
            SendCommand = new DelegateCommand(SendCommandMethod);

            //Dictionary<int, List<double>> ParamListDic = new Dictionary<int, List<double>>();
            //ParamListDic.Add(300, new List<double>() { 1 });
            //ParamListDic.Add(301, new List<double>() { 2 });
            //ParamListDic.Add(302, new List<double>() { 3 });
            //ParamListDic.Add(303, new List<double>() { 4 });
            //ParamListDic.Add(304, new List<double>() { 5 });
            //ParamListDic.Add(305, new List<double>() { 6 });
            //int index = 0;

            //foreach (var item in ParamListDic)
            //{
            //    if (index == 0)
            //    {
            //        Data1 = item.Value[0];
            //    }
            //    if (index == 1)
            //    {
            //        Data2 = item.Value[0];
            //    }
            //    if (index == 2)
            //    {
            //        Data3 = item.Value[0];
            //    }
            //    if (index == 3)
            //    {
            //        Data4 = item.Value[0];
            //    }
            //    if (index == 4)
            //    {
            //        Data5 = item.Value[0];
            //    }
            //    if (index == 5)
            //    {
            //        Data6 = item.Value[0];
            //    }
            //    index++;
            //}
        }

        private void Instance_BLEDataUpdated(object? sender, EventArgs e)
        {
            DataParseEventArgs args = (DataParseEventArgs)e;
            int index = 0;
            foreach (var item in args.ParamListDic)
            {
                if (index==0)
                {
                    Data1 = item.Value[0];
                }
                if (index == 1)
                {
                    Data2 = item.Value[0];
                }
                if (index == 2)
                {
                    Data3 = item.Value[0];
                }
                if (index == 3)
                {
                    Data4 = item.Value[0];
                }
                if (index == 4)
                {
                    Data5 = item.Value[0];
                }
                if (index == 5)
                {
                    Data6 = item.Value[0];
                }
                index++;
            }
            
        }

        void ConnectDevice()
        {
            Thread.Sleep(500);//等有无USB数据
            if (SMDataSource.Instance.HidConnected())
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
                        var bluetooth = SMDataSource.Instance.BluetoothList[0];
                        if (bluetooth != null)
                        {
                            List<BluetoothInfo> selectList = new List<BluetoothInfo>();
                            selectList.Add(bluetooth);
                            SMDataSource.Instance.BluetoothConnect(selectList);
                            break;
                        }
                        
                    }
                    Thread.Sleep(500);//50s

                    loop--;
                }
            });
        }
        
        void ClearSelectCommandMethod()
        {
            Data1 = 0;
            Data2 = 0;
            Data3 = 0;
            Data4 = 0;
            Data5 = 0;
            Data6 = 0;
        }

        void SendCommandMethod()
        {
            var buffer = BufferConvertHelper.HexStringToByteArray(SendStr.Replace(" ", ""));
            if (SMDataSource.Instance.HidConnected())
            {
                SMDataSource.Instance.hid1.SendBuffer(buffer);
            }
            else
            {
                if (SMDataSource.Instance.BluetoothList.Count > 0)
                {
                    SMDataSource.Instance.SendCommandBle(buffer);
                }
            }
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
            Title = "数据测试";
            Width = 1350;
            Height = 820;
        }
        #endregion
    }

}
