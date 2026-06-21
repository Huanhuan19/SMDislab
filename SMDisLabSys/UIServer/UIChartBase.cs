using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using SMDisLabSys.UIServer.Caculator;
using SMDisLabSys.UIServer.Dot;

namespace SMDisLabSys.UIServer
{
    public class UIChartBase : BindableBase
    {
        public DelegateCommand<string> SelectChecked { get; set; }
        public DelegateCommand<string> SelectUnchecked { get; set; }

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

        string chartTitle = "";
        public string ChartTitle
        {
            get { return chartTitle; }
            set { SetProperty(ref chartTitle, value); }
        }

        string yLineTitle = "";
        public string YLineTitle
        {
            get { return yLineTitle; }
            set { SetProperty(ref yLineTitle, value); }
        }
        string xLineTitle = "";
        public string XLineTitle
        {
            get { return xLineTitle; }
            set { SetProperty(ref xLineTitle, value); }
        }

        public Func<double, string> XFormatter { get; set; }
        public string XFormatters(double value)
        {
            return string.Format($"{Math.Round(value, 2)}");
        }

        public Func<double, string> YFormatter { get; set; }
        public string YFormatters(double value)
        {
            return string.Format($"{Math.Round(value, 2)}");
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

        private ObservableCollection<CheckBoxViewModel> checkBoxItems = new ObservableCollection<CheckBoxViewModel>();
        public ObservableCollection<CheckBoxViewModel> CheckBoxItems
        {
            get { return checkBoxItems; }
            set
            {
                SetProperty(ref checkBoxItems, value);
            }
        }
        #endregion


        private List<string> m_colors = new List<string>() { "#02c0fa", "#FFB751", "#00a759", "#e95e00", "#ff00cc", "#00FFFF", "#cc00ff", "#88a700" };
        public UIChartBase()
        {

            SelectChecked = new DelegateCommand<string>(SelectCheckedMethod);
            SelectUnchecked = new DelegateCommand<string>(SelectUncheckedMethod);
        }
        public int lineIndex = 0;

        Dictionary<int, ChartValues<ObservablePoint>> Diclinevalues = new Dictionary<int, ChartValues<ObservablePoint>>();

        public bool haveUSBData = false;


        #region 画线

        /// <summary>
        /// 点曲线
        /// </summary>
        /// <param name="pointGeometry"></param>
        /// <param name="pointGeometrySize"></param>
        /// <param name="strokeThickness"></param>
        public void CreatLinePointGeometry(Geometry pointGeometry, double pointGeometrySize = 12, double strokeThickness = 2)
        {
            LineSeries line = new LineSeries();
            //line.Title = item.ParamName;
            line.PointGeometry = DefaultGeometries.Circle;
            line.PointGeometrySize = 12;
            line.LineSmoothness = 0;
            line.StrokeThickness = 2;
            line.Stroke = Brushes.Transparent;
            line.Fill = Brushes.Transparent;
            line.PointForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(m_colors[lineIndex]));

            ChartValues<ObservablePoint> linevalue;
            Diclinevalues.TryGetValue(lineIndex, out linevalue);
            line.Values = linevalue;
            SeriesDic.Add(line);

            lineIndex++;
        }
        /// <summary>
        /// 线
        /// </summary>
        /// <param name="title"></param>
        public void CreatLinePoint(string title = "")
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Diclinevalues.Add(lineIndex, new ChartValues<ObservablePoint>());
                LineSeries line = new LineSeries();
                //line.DataLabels = true;
                line.Title = title;
                line.PointGeometry = null;
                line.LineSmoothness = 0;
                line.StrokeThickness = 2;
                line.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(m_colors[lineIndex]));
                line.Fill = new SolidColorBrush(Colors.Transparent);

                ChartValues<ObservablePoint> linevalue;
                Diclinevalues.TryGetValue(lineIndex, out linevalue);
                line.Values = linevalue;
                SeriesDic.Add(line);

                lineIndex++;
            });
        }
        /// <summary>
        /// 虚线
        /// </summary>
        /// <param name="title"></param>
        public void CreatDashLinePoint(string title = "")
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Diclinevalues.Add(lineIndex, new ChartValues<ObservablePoint>());
                LineSeries line = new LineSeries();
                //line.DataLabels = true;
                line.Title = title;
                line.PointGeometry = null;
                line.LineSmoothness = 0;
                line.StrokeThickness = 2;
                line.StrokeDashArray = new DoubleCollection { 6, 4 };
                line.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(m_colors[lineIndex]));
                line.Fill = new SolidColorBrush(Colors.Transparent);

                ChartValues<ObservablePoint> linevalue;
                Diclinevalues.TryGetValue(lineIndex, out linevalue);
                line.Values = linevalue;
                SeriesDic.Add(line);

                lineIndex++;
            });
        }

        public void AddPoint(double x, double y, int lineIndex)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ObservablePoint point = new ObservablePoint();
                point.X = x;
                point.Y = y;

                ChartValues<ObservablePoint> linevalue;
                Diclinevalues.TryGetValue(lineIndex, out linevalue);
                linevalue.Add(point);

                var max = LineRange.Instance.GetLineMax(linevalue);
                if (max > Max)
                {
                    Max = max;
                }
                var min = LineRange.Instance.GetLineMin(linevalue);
                if (Min < min)
                {
                    Min = min;
                }
            });
        }

        public void ClearLine()
        {
            SeriesDic.Clear();
            lineIndex = 0;
            CheckBoxItems.Clear();

            Diclinevalues.Clear();
        }

        #endregion

        #region 曲线选取
        void SelectCheckedMethod(string index)
        {
            try
            {
                (SeriesDic[Convert.ToInt16(index)] as LineSeries).Visibility = Visibility.Visible;
            }
            catch (Exception)
            {
            }
        }
        void SelectUncheckedMethod(string index)
        {
            try
            {
                (SeriesDic[Convert.ToInt16(index)] as LineSeries).Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 添加曲线复选框
        /// </summary>
        /// <param name="text"></param>
        public void AddCheckBoxItem(string text)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => 
            {
                CheckBoxItems.Add(new CheckBoxViewModel { Text = text, IsChecked = true, Index = lineIndex.ToString() });
            }); 
        }
        #endregion

        #region 曲线测试

        public void CreatTestLine()
        {
            CreatLinePoint();
            CheckBoxItems.Add(new CheckBoxViewModel { Text = lineIndex.ToString(), IsChecked = true, Index = lineIndex.ToString() });
            Task.Run(() =>
            {
                for (int i = 0; i < 50; i++)
                {
                    AddPoint(i, i * i, lineIndex - 1);
                    Thread.Sleep(100);
                }
            });
        }

        #endregion

    }
}
