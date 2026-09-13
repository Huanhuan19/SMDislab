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
using ScottPlot;
using Colors = ScottPlot.Colors;

namespace SMDisLabSys.Pages.WL.KL.ViewModels
{
    class LiDeHeChengVM : UIScottPlotBase, IDialogAware
    {
        DateTime dtCreat;
        double xAxis = 0;

        public DelegateCommand ClearSelectCommand { get; private set; }
        public DelegateCommand HeLiCommand { get; private set; }
        public DelegateCommand F1F2Command { get; private set; }

        #region 属性
        double f1;
        public double F1
        {
            get { return f1; }
            set { SetProperty(ref f1, value); }
        }
        double f2;
        public double F2
        {
            get { return f2; }
            set { SetProperty(ref f2, value); }
        }
        double a1;
        public double A1
        {
            get { return a1; }
            set { SetProperty(ref a1, value); }
        }
        double a2;
        public double A2
        {
            get { return a2; }
            set { SetProperty(ref a2, value); }
        }
        double fHe;
        public double FHe
        {
            get { return fHe; }
            set { SetProperty(ref fHe, value); }
        }

        #endregion
        public LiDeHeChengVM()
        {
            InitCommand();
            ConnectDevice();
        }
        void InitCommand()
        {
            RealDataBLE.Instance.BLEDataUpdated += Instance_BLEDataUpdated;

            HeLiCommand = new DelegateCommand(HeLiCommandMethod);
            F1F2Command = new DelegateCommand(F1F2CommandMethod);

            ClearSelectCommand = new DelegateCommand(ClearSelectCommandMethod);

            CreatArrow(0, 0, 0, 3, Colors.Blue, "F′", 0, 3 * 1.1);
        }

        private void Instance_BLEDataUpdated(object? sender, EventArgs e)
        {
            DataParseEventArgs args = (DataParseEventArgs)e;
            if (args.Channel == 1)
            {
                int index = 0;
                foreach (var item in args.ParamListDic)
                {
                    if (index == 0)
                    {
                        Sensor1Value1 = item.Value[0];
                    }
                    if (index == 2)
                    {
                        Sensor1Value2 = item.Value[0];
                    }
                    index++;
                }
            }
            else if (args.Channel == 2)
            {
                int index = 0;
                foreach (var item in args.ParamListDic)
                {
                    if (index == 0)
                    {
                        Sensor2Value1 = item.Value[0];
                    }
                    if (index == 2)
                    {
                        Sensor2Value2 = item.Value[0];
                    }
                    index++;
                }
            }

        }

        void ConnectDevice()
        {
            Thread.Sleep(500);//等有无USB数据
            if (SMDataSource.Instance.HidConnected())
            {
                ConnectItem = "USB 已连接";
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
                            ConnectItem = $"蓝牙{bluetooth.Adresse} 已连接";
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

        }

        void HeLiCommandMethod()
        {
            FHe = Sensor1Value1 > Sensor2Value1 ? Sensor1Value1 : Sensor2Value1;

            CreatArrow(0, 0, 0, FHe, Colors.Blue, "F′", 0, FHe * 1.1);
            CreatArrow(0, 0, 0, -1 * FHe, Colors.Red, "F", 0, -1.1 * FHe);
        }
        void F1F2CommandMethod()
        {
            F1 = Sensor1Value1;
            F2 = Sensor2Value1;

            A1 = Sensor1Value2;
            A2 = Sensor2Value2;
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
            Title = "力的合成与分解";
            Width = 1350;
            Height = 820;
        }
        #endregion
    }

}
