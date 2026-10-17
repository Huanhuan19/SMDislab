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
using Windows.Management.Deployment;

namespace SMDisLabSys.Pages.WL.KL.ViewModels
{
    class LiDeHeChengVM : UIScottPlotBase, IDialogAware
    {
        DateTime dtCreat;
        double xAxis = 0;

        public DelegateCommand ClearSelectCommand { get; private set; }
        public DelegateCommand HeLiCommand { get; private set; }
        public DelegateCommand F1F2StartCommand { get; private set; }

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

        double xhe;
        double yhe;

        string BLEName = "SHM:1";

        CancellationTokenSource cts = new CancellationTokenSource();
        public LiDeHeChengVM()
        {
            InitCommand();
            ConnectDevice(BLEName);
        }
        void InitCommand()
        {
            RealDataBLE.Instance.BLEDataUpdated += Instance_BLEDataUpdated;

            HeLiCommand = new DelegateCommand(HeLiCommandMethod);
            F1F2StartCommand = new DelegateCommand(F1F2StartCommandMethod);
            LiLunHeCommand = new DelegateCommand(LiLunHeCommandMethod);

            ClearSelectCommand = new DelegateCommand(ClearSelectCommandMethod);
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

        void ClearSelectCommandMethod()
        {
            cts.Cancel();
            Clear();


            FHe = "";
            FHePie = "";
        }

        void HeLiCommandMethod()
        {
            var he = Sensor1Value1 > Sensor2Value1 ? Sensor1Value1 : Sensor2Value1;
            //he = 3;
            FHePie = he.ToString();

            double maxY = he * 1.15;
            SetAxis(-2.8 * maxY, 2.8 * maxY, -maxY, maxY);

            CreatDashArrow(0, 0, 0, he, Colors.Red, "F′", 0, he * 1.1);
            CreatArrow(0, 0, 0, -1 * he, Colors.Blue, "F=mg", 0, -1.02 * he);
            CreatMarker(0, 0);


        }
        void F1F2StartCommandMethod()
        {
            int count = 0;
            Task.Run(() =>
            {
                try
                {
                    CancellationToken token = cts.Token;
                    while (true)
                    {
                        //count++;
                        if (token.IsCancellationRequested)//停止任务使用
                        {
                            cts = new CancellationTokenSource();
                            token.ThrowIfCancellationRequested();
                        }
                        RemoveArrowTextLine();

                        //Sensor1Value1 = 2.1 + count * 0.01;
                        //Sensor2Value1 = 2.1 + count * 0.01;

                        //Sensor1Value2 = 45;
                        //Sensor2Value2 = -45;

                        var x1 = -Sensor1Value1 * Math.Cos((90 - Sensor1Value2) * Math.PI / 180);
                        var y1 = Sensor1Value1 * Math.Sin((90 - Sensor1Value2) * Math.PI / 180);

                        var x2 = -Sensor2Value1 * Math.Cos((90 - Sensor2Value2) * Math.PI / 180);
                        var y2 = Sensor2Value1 * Math.Sin((90 - Sensor2Value2) * Math.PI / 180);



                        var min = Sensor1Value1 > Sensor2Value1 ? Sensor2Value1 : Sensor1Value1;
                        #region 标注角度
                        if (Sensor1Value2 > 0)
                        {
                            CreatArc(90, Sensor1Value2, "θ₁", 0.4 * min);
                            CreatArrow(0, 0, x1, y1, Colors.Brown, "F₁", x1 * 1.25, y1 * 1.05);
                        }
                        else
                        {
                            CreatArc(90, Sensor1Value2, "θ₁", 0.4 * min);
                            CreatArrow(0, 0, x1, y1, Colors.Brown, "F₁", x1 * 1.15, y1 * 1.05);
                        }
                        if (Sensor2Value2 > 0)
                        {
                            CreatArc(90, Sensor2Value2, "θ₂", 0.45 * min);
                            CreatArrow(0, 0, x2, y2, Colors.Orange, "F₂", x2 * 1.25, y2 * 1.05);
                        }
                        else
                        {
                            CreatArc(90, Sensor2Value2, "θ₂", 0.45 * min);
                            CreatArrow(0, 0, x2, y2, Colors.Orange, "F₂", x2 * 1.15, y2 * 1.05);
                        }
                        #endregion
                        //
                        //CreatPlotText("θ₁", x1 * 0.22, y1 * 0.33, Colors.DarkRed, 22);
                        //CreatPlotText("θ₂", x2 * 0.12, y2 * 0.33, Colors.DarkRed, 22);

                        F1 = Sensor1Value1;
                        F2 = Sensor2Value1;

                        A1 = Sensor1Value2;
                        A2 = Sensor2Value2;

                        xhe = x1 + x2;
                        yhe = y1 + y2;
                        CreatLineDashed(x1, y1, xhe, yhe, Colors.DarkGray);
                        CreatLineDashed(x2, y2, xhe, yhe, Colors.DarkGray);

                        CreatArrow(0, 0, xhe, yhe, Colors.Green, "F", xhe * 1.1, yhe * 1.05);
                        CreatPlotText("合", xhe * 1.1 + 0.05, yhe * 1.05 - 0.06, Colors.Green, 13);
                        FHe = Math.Round(Math.Sqrt(xhe * xhe + yhe * yhe), 2).ToString();
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception)
                {
                }
            });
        }
        void RemoveArrowTextLine()
        {
            RemoveArrowOver(2);
            RemoveTextOver(2);
            RemoveLineOver(1);
            ClearArc();

        }

        void LiLunHeCommandMethod()
        {
            cts.Cancel();

            FHe = Math.Round(Math.Sqrt(xhe * xhe + yhe * yhe), 2).ToString();
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
