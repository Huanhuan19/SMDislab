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
using Newtonsoft.Json.Linq;

namespace SMDisLabSys.Pages.WL.KL.ViewModels
{
    class LiDeHeChengVM : UIScottPlotBase, IDialogAware
    {
        DateTime dtCreat;
        double xAxis = 0;

        public DelegateCommand ClearSelectCommand { get; private set; }
        public DelegateCommand HeLiCommand { get; private set; }
        public DelegateCommand F1F2StartCommand { get; private set; }
        public DelegateCommand F1F2Command { get; private set; }

        public DelegateCommand LiLunHeCommand { get; private set; }

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
        string fHe;
        public string FHe
        {
            get { return fHe; }
            set { SetProperty(ref fHe, value); }
        }
        string fHePie;
        public string FHePie
        {
            get { return fHePie; }
            set { SetProperty(ref fHePie, value); }
        }

        #endregion

        CancellationTokenSource cts = new CancellationTokenSource();
        public LiDeHeChengVM()
        {
            InitCommand();
            ConnectDevice();

            SetAxis(-14, 14, -5, 5);
        }
        void InitCommand()
        {
            RealDataBLE.Instance.BLEDataUpdated += Instance_BLEDataUpdated;

            HeLiCommand = new DelegateCommand(HeLiCommandMethod);
            F1F2StartCommand = new DelegateCommand(F1F2StartCommandMethod);
            F1F2Command = new DelegateCommand(F1F2CommandMethod);
            LiLunHeCommand = new DelegateCommand(LiLunHeCommandMethod);

            ClearSelectCommand = new DelegateCommand(ClearSelectCommandMethod);

            Sensor2Value1 = 3;
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
            var he = Sensor1Value1 > Sensor2Value1 ? Sensor1Value1 : Sensor2Value1;
            FHe = he.ToString();

            FHe = "4";
            CreatArrow(0, 0, 0, he, Colors.Red, "F′", 0, he * 1.1);
            CreatArrow(0, 0, 0, -1 * he, Colors.Blue, "F=mg", 0, -1.02 * he);
            CreatMarker(0, 0);

        }
        void F1F2StartCommandMethod()
        {

            Task.Run(() =>
            {
                try
                {
                    CancellationToken token = cts.Token;
                    while (true)
                    {
                        if (token.IsCancellationRequested)//停止任务使用
                        {
                            token.ThrowIfCancellationRequested();
                        }
                        RemoveArrowAndTextOver(2);

                        Sensor1Value1 = 3;
                        Sensor2Value1 = 3;

                        Sensor1Value2 = 30;
                        Sensor2Value2 = -60;

                        var x1 = Sensor1Value1 * Math.Cos((90 - Sensor1Value2) * Math.PI / 180);
                        var y1 = Sensor1Value1 * Math.Sin((90 - Sensor1Value2) * Math.PI / 180);

                        var x2 = Sensor2Value1 * Math.Cos((90 - Sensor2Value2) * Math.PI / 180);
                        var y2 = Sensor2Value1 * Math.Sin((90 - Sensor2Value2) * Math.PI / 180);

                        CreatArrow(0, 0, x1, y1, Colors.Green, "F1", x1 * 1.1, y1 * 1.05);
                        CreatArrow(0, 0, x2, y2, Colors.Orange, "F2", x2 * 1.1, y2 * 1.05);
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception)
                {
                }
            });
        }
        void F1F2CommandMethod()
        {
            cts.Cancel();

            F1 = Sensor1Value1;
            F2 = Sensor2Value1;

            A1 = Sensor1Value2;
            A2 = Sensor2Value2;

            RemoveArrowAndTextOver(2);

            var x1 = F1 * Math.Cos((90 - A1) * Math.PI / 180);
            var y1 = F1 * Math.Sin((90 - A1) * Math.PI / 180);

            var x2 = F2 * Math.Cos((90 - A2) * Math.PI / 180);
            var y2 = F2 * Math.Sin((90 - A2) * Math.PI / 180);

            CreatArrow(0, 0, x1, y1, Colors.Green, "F1", x1 * 1.1, y1 * 1.05);
            CreatArrow(0, 0, x2, y2, Colors.Orange, "F2", x2 * 1.1, y2 * 1.05);



            Thread.Sleep(1000);
        }

        void LiLunHeCommandMethod()
        {
            var x1 = F1 * Math.Cos((90 - A1) * Math.PI / 180);
            var y1 = F1 * Math.Sin((90 - A1) * Math.PI / 180);

            var x2 = F2 * Math.Cos((90 - A2) * Math.PI / 180);
            var y2 = F2 * Math.Sin((90 - A2) * Math.PI / 180);

            var xhe = x1 + x2;
            var yhe = y1 + y2;
            CreatLineDashed(x1, y1, xhe, yhe);
            CreatLineDashed(x2, y2, xhe, yhe);
            CreatArrow(0, 0, xhe, yhe, Colors.Brown, "F", x2 * 1.1, y2 * 1.05);
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
