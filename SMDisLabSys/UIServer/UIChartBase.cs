using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Prism.Commands;
using Prism.Mvvm;

namespace SMDisLabSys.UIServer
{
    public class UIChartBase : BindableBase
    {
        public DelegateCommand ExplainCommand { get; private set; }

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
        #endregion

        private List<string> m_colors = new List<string>() { "#02c0fa", "#FFB751", "#00a759", "#88a700", "#e95e00", "#ff00cc", "#cc00ff", "#00FFFF" };

        private int lineIndex = 0;

        Dictionary<int, ChartValues<ObservablePoint>> Diclinevalues = new Dictionary<int, ChartValues<ObservablePoint>>();

        public bool haveUSBData = false;

        public UIChartBase()
        {
            ExplainCommand = new DelegateCommand(ExplainCommandMethod);
        }
        void ExplainCommandMethod()
        {

        }

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
        public void CreatLinePoint(string title = "")
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Diclinevalues.Add(0, new ChartValues<ObservablePoint>());
                LineSeries line = new LineSeries();
                //line.DataLabels = true;
                line.Title = title;
                line.PointGeometry = null;
                line.LineSmoothness = 0;
                line.StrokeThickness = 2;
                line.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(m_colors[lineIndex]));

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
            });
        }
    }
}
