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
using SMDisLabSys.UIServer.Dot;

namespace SMDisLabSys.Pages.WL.AG.ViewModels
{
    class DanBaiNewVM : BindableBase, IDialogAware
    {
        public DelegateCommand RecordDataCommand { get; private set; }
        public DelegateCommand ClearSelectCommand { get; private set; }
        public DelegateCommand TAndLCommand { get; private set; }
        public DelegateCommand TTAndLCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }
        public DelegateCommand ExplainCommand { get; private set; }

        public DelegateCommand LToGCommand { get; private set; }
        public DelegateCommand GToLCommand { get; private set; }

        int recordIndex = 0;

        ChartValues<ObservablePoint> Points = new ChartValues<ObservablePoint>();
        ChartValues<ObservablePoint> Points2 = new ChartValues<ObservablePoint>();

        #region 属性
        private SeriesCollection seriesDic = new SeriesCollection();
        public SeriesCollection SeriesDic
        {
            get { return seriesDic; }
            set
            {
                SetProperty(ref seriesDic, value);
            }
        }
        public Func<double, string> XFormatter { get; set; }
        private string XFormatters(double value)
        {
            return string.Format($"{Math.Round(value, 2)}");
        }

        public Func<double, string> YFormatter { get; set; }
        private string YFormatters(double value)
        {
            return string.Format($"{Math.Round(value, 2)}");
        }

        double circle;
        public double Circle
        {
            get { return circle; }
            set { SetProperty(ref circle, value); }
        }
        double angle = 0;
        public double Angle
        {
            get { return angle; }
            set { SetProperty(ref angle, value); }
        }


        private ObservableCollection<LineDataDOT> lineDataSource;
        public ObservableCollection<LineDataDOT> LineDataSource
        {
            get { return lineDataSource; }
            set { SetProperty(ref lineDataSource, value); }
        }
        private LineDataDOT lineDataSelect;
        public LineDataDOT LineDataSelect
        {
            get { return lineDataSelect; }
            set { SetProperty(ref lineDataSelect, value); }
        }
        string yLineTitle = "周期(s)";
        public string YLineTitle
        {
            get { return yLineTitle; }
            set { SetProperty(ref yLineTitle, value); }
        }

        Visibility isVisibleGridLToG = Visibility.Visible;
        /// <summary>
        /// Grid的显示
        /// </summary>
        public Visibility IsVisibleGridLToG
        {
            get { return isVisibleGridLToG; }
            set
            {
                SetProperty(ref isVisibleGridLToG, value);
            }
        }


        double min = 0;
        public double Min
        {
            get { return min; }
            set { SetProperty(ref min, value); }
        }
        double max = 1;
        public double Max
        {
            get { return max; }
            set { SetProperty(ref max, value); }
        }

        string chartTitle = "单摆周期与摆长的关系";
        public string ChartTitle
        {
            get { return chartTitle; }
            set { SetProperty(ref chartTitle, value); }
        }
        #endregion
        public DanBaiNewVM()
        {
            XFormatter = XFormatters;
            YFormatter = YFormatters;
            InitCommand();
            InitLine();
            InitDataGrid();
            ConnectDevice();
        }
        void InitCommand()
        {
            RecordDataCommand = new DelegateCommand(RecordDataCommandMethod);
            ClearSelectCommand = new DelegateCommand(ClearSelectCommandMethod);
            TAndLCommand = new DelegateCommand(TAndLCommandMethod);
            TTAndLCommand = new DelegateCommand(TTAndLCommandMethod);
            ClearCommand = new DelegateCommand(ClearCommandMethod);
            ExplainCommand = new DelegateCommand(ExplainCommandMethod);

            LToGCommand = new DelegateCommand(LToGCommandMethod);
            GToLCommand = new DelegateCommand(GToLCommanddMethod);

            RealDataBLE.Instance.BLEDataUpdated += Instance_BLEDataUpdated;
        }

        private void Instance_BLEDataUpdated(object? sender, EventArgs e)
        {
            DataParseBLEEventArgs args = (DataParseBLEEventArgs)e;
            if (args.ParamList.Count >= 2)
            {
                Circle = args.ParamList[0];
                Angle = args.ParamList[1];
            }
        }

        void InitLine()
        {
            LineSeries line = new LineSeries();
            //line.Title = item.ParamName;
            line.PointGeometry = DefaultGeometries.Circle;
            line.PointGeometrySize = 12;
            line.LineSmoothness = 0;
            line.StrokeThickness = 2;
            line.Stroke = Brushes.Transparent;
            line.Fill = Brushes.Transparent;
            line.PointForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#02c0fa"));

            //line.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a759"));
            SeriesDic.Add(line);

            LineSeries line2 = new LineSeries();
            line2.PointGeometry = null;
            line2.PointGeometrySize = 12;
            line2.LineSmoothness = 0;
            line2.StrokeThickness = 2;
            line2.Stroke = Brushes.Transparent;
            line2.Fill = Brushes.Transparent;

            line2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00a759"));
            SeriesDic.Add(line2);

            SeriesDic[0].Values = Points;
            SeriesDic[1].Values = Points2;
        }
        void InitDataGrid()
        {
            LineDataSource = new ObservableCollection<LineDataDOT>();
            for (int i = 0; i < 4; i++)
            {
                LineDataDOT lineData = new LineDataDOT();
                lineData.Index = i;
                LineDataSource.Add(lineData);
            }
        }

        void ConnectDevice()
        {
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
        void RecordDataCommandMethod()
        {
            if (recordIndex >= 4)
            {
                MessageBox.Show($"已完成实验");
                return;
            }
            if (IsVisibleGridLToG == Visibility.Visible)//已知摆长 求重力加速度
            {
                if (string.IsNullOrEmpty(LineDataSource[recordIndex].DanBaiLinelong))
                {
                    MessageBox.Show($"请先在第 {recordIndex + 1} 行输入单摆长度");
                    return;
                }
                if (recordIndex >= 4)
                {
                    MessageBox.Show($"已完成实验");
                    return;
                }
                while (true)
                {
                    if (!string.IsNullOrEmpty(LineDataSource[recordIndex].DanBaiCircle))
                    {
                        recordIndex++;
                        if (recordIndex > 3)//记录完成
                        {
                            recordIndex = 3;
                            MessageBox.Show($"已完成实验");
                            return;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                LineDataSource[recordIndex].DanBaiCircle = Circle.ToString();
                LineDataSource[recordIndex].DanBaiCircle2 = Math.Round(Circle * Circle, 4).ToString();
                LineDataSource[recordIndex].Zhonglig = Math.Round(4 * Math.PI * Math.PI * Convert.ToDouble(LineDataSource[recordIndex].DanBaiLinelong) / Convert.ToDouble(LineDataSource[recordIndex].DanBaiCircle2), 2).ToString();

                recordIndex++;
            }
            else// 已知重力加速度 求摆长
            {
                if (string.IsNullOrEmpty(LineDataSource[recordIndex].Zhonglig))
                {
                    MessageBox.Show($"请先在第 {recordIndex + 1} 行输入重力加速度");
                    return;
                }
                if (recordIndex >= 4)
                {
                    MessageBox.Show($"已完成实验");
                    return;
                }
                while (true)
                {
                    if (!string.IsNullOrEmpty(LineDataSource[recordIndex].DanBaiCircle))
                    {
                        recordIndex++;
                        if (recordIndex > 3)//记录完成
                        {
                            recordIndex = 3;
                            MessageBox.Show($"已完成实验");
                            return;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                LineDataSource[recordIndex].DanBaiCircle = Circle.ToString();
                LineDataSource[recordIndex].DanBaiCircle2 = Math.Round(Circle * Circle, 4).ToString();
                lineDataSource[recordIndex].DanBaiLinelong = Math.Round(Convert.ToDouble(LineDataSource[recordIndex].DanBaiCircle2) * Convert.ToDouble(LineDataSource[recordIndex].Zhonglig) / (4 * Math.PI * Math.PI), 2).ToString();
                recordIndex++;
            }

        }
        void ClearSelectCommandMethod()
        {
            if (IsVisibleGridLToG == Visibility.Visible)//已知摆长 求重力加速度
            {
                ClearLToGGridData(LineDataSelect);
            }
            else
            {
                ClearGToLGridData(LineDataSelect);
            }
            recordIndex = LineDataSelect.Index;
        }
        void ClearLToGGridData(LineDataDOT lineDataDOT)
        {
            if (lineDataDOT != null)
            {
                lineDataDOT.DanBaiCircle = "";
                lineDataDOT.Zhonglig = "";
                lineDataDOT.DanBaiCircle2 = "";
            }
        }
        void ClearGToLGridData(LineDataDOT lineDataDOT)
        {
            if (lineDataDOT != null)
            {
                lineDataDOT.DanBaiCircle = "";
                lineDataDOT.DanBaiLinelong = "";
                lineDataDOT.DanBaiCircle2 = "";
            }
        }
        void TAndLCommandMethod()
        {
            try
            {
                SeriesDic[0].Values.Clear();
                SeriesDic[1].Values.Clear();
                YLineTitle = "周期(s)";
                ChartTitle = "单摆周期与摆长的关系";
                for (int i = 0; i < 4; i++)
                {
                    if (string.IsNullOrEmpty(LineDataSource[i].DanBaiLinelong))
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(LineDataSource[i].DanBaiCircle))
                    {
                        continue;
                    }
                    ObservablePoint point = new ObservablePoint();
                    point.X = Convert.ToDouble(LineDataSource[i].DanBaiLinelong);
                    point.Y = Convert.ToDouble(LineDataSource[i].DanBaiCircle);
                    Points.Add(point);
                }
                Max = LineRange.Instance.GetLineMax(Points);
                Min = LineRange.Instance.GetLineMin(Points);
            }
            catch (Exception)
            {
                MessageBox.Show("数据转换有错误");
            }
        }
        void TTAndLCommandMethod()
        {
            try
            {
                SeriesDic[0].Values.Clear();
                SeriesDic[1].Values.Clear();
                YLineTitle = "T²";
                ChartTitle = "单摆周期的平方与摆长的关系";
                for (int i = 0; i < 4; i++)
                {
                    if (string.IsNullOrEmpty(LineDataSource[i].DanBaiLinelong))
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(LineDataSource[i].DanBaiCircle2))
                    {
                        continue;
                    }
                    ObservablePoint point = new ObservablePoint();
                    point.X = Convert.ToDouble(LineDataSource[i].DanBaiLinelong);
                    point.Y = Convert.ToDouble(LineDataSource[i].DanBaiCircle2);
                    Points.Add(point);
                }

                bool success = false;
                double[] para = new double[] { 0, 0, 0, 0 };
                double[] newYList = new double[Points.Count];
                var num_x = (from o in Points select o.X).ToArray();
                var num_y = (from o in Points select o.Y).ToArray();

                success = FormulaNH.Instance.CalcLineFitting(num_x, num_y, ref para, ref newYList);
                if (success == true)
                {
                    for (int i = 0; i < num_x.Length; i++)
                    {
                        Points2.Add(new ObservablePoint() { X = num_x[i], Y = newYList[i] });
                    }
                }
                Max = LineRange.Instance.GetLineMax(Points);
                Min = LineRange.Instance.GetLineMin(Points);
            }
            catch (Exception)
            {
                MessageBox.Show("数据转换有错误");
            }
        }
        void ClearCommandMethod()
        {
            Points.Clear();
            Points2.Clear();
            recordIndex = 0;
            for (int i = 0; i < 4; i++)
            {
                if (IsVisibleGridLToG == Visibility.Visible)//已知摆长 求重力加速度
                {
                    ClearLToGGridData(LineDataSource[i]);
                }
                else
                {
                    ClearGToLGridData(LineDataSource[i]);
                }
            }
        }
        void ExplainCommandMethod()
        {

        }
        void LToGCommandMethod()
        {
            IsVisibleGridLToG = Visibility.Visible;
        }
        void GToLCommanddMethod()
        {
            IsVisibleGridLToG = Visibility.Hidden;
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
