using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using NPOI.SS.Formula.Functions;

//using NPOI.SS.Formula.Functions;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;

namespace SMDisLabSys.UIServer
{
    public class UIScottPlotBase : UIBase
    {
        #region Props
        WpfPlot _plotControl;
        public WpfPlot PlotControl
        {
            get { return _plotControl; }
            set { SetProperty(ref _plotControl, value); }
        }
        #endregion

        public UIScottPlotBase()
        {
            PlotControl = new WpfPlot();
        }

        public void SetAxis(double minX, double maxX, double minY, double maxY)
        {
            PlotControl.Plot.Axes.SetLimits(minX, maxX, minY, maxY);
            PlotControl.Refresh(); // 新增后必须刷新
        }
        #region 创建

        #region 创建箭头
        public void CreatArrow(double startX, double startY, double endX, double endY, Color color, string title = "", double titleX = 0, double titleY = 0)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                // 示例：新增一个蓝色箭头
                var newArrow = PlotControl.Plot.Add.Arrow(startX, startY, endX, endY);
                newArrow.ArrowShape = ArrowShape.Single.GetShape();
                newArrow.ArrowLineColor = color;
                newArrow.ArrowFillColor = color;

                newArrow.ArrowWidth = 3;
                newArrow.ArrowLineWidth = 1;
                newArrow.ArrowheadLength = 15;               //箭头三角长度
                newArrow.ArrowheadWidth = 10;                 //箭头三角宽度

                PlotControl.Refresh(); // 新增后必须刷新

            });

            CreatPlotText(title, titleX, titleY, color);
        }
        public void CreatArrowNoText(double startX, double startY, double endX, double endY, Color color)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                // 示例：新增一个蓝色箭头
                var newArrow = PlotControl.Plot.Add.Arrow(startX, startY, endX, endY);
                newArrow.ArrowShape = ArrowShape.Single.GetShape();
                newArrow.ArrowLineColor = color;
                newArrow.ArrowFillColor = color;

                newArrow.ArrowWidth = 3;
                newArrow.ArrowLineWidth = 1;
                newArrow.ArrowheadLength = 15;               //箭头三角长度
                newArrow.ArrowheadWidth = 10;                 //箭头三角宽度
                PlotControl.Refresh(); // 新增后必须刷新

            });
        }
        public void CreatDashArrow(double startX, double startY, double endX, double endY, Color color, string title = "", double titleX = 0, double titleY = 0)
        {
            CreatLineDashed(startX, startY, endX, endY, color);
            CreatArrow(0.95 * endX, 0.95 * endY, endX, endY, color, title, titleX, titleY);
        }
        #endregion

        public void CreatPlotText(string title, double titleX, double titleY, Color color, float fontSize = 25)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                string fontName = Fonts.Detect(title);
                var txt = PlotControl.Plot.Add.Text(title, titleX, titleY);
                txt.LabelFontSize = fontSize;
                txt.LabelFontName = fontName;
                txt.LabelFontColor = color;

                //PlotControl.Plot.Axes.AutoScale();
                PlotControl.Refresh(); // 新增后必须刷新
            });
        }

        public void CreatMarker(double startX, double startY)
        {
            PlotControl.Plot.Add.Marker(startX, startY, MarkerShape.FilledCircle, size: 12, color: Colors.Black);
        }

        public void CreatLineDashed(double startX, double startY, double endX, double endY, Color color)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var dashLine = PlotControl.Plot.Add.Line(startX, startY, endX, endY);
                dashLine.LinePattern = LinePattern.Dashed; //虚线
                dashLine.LineWidth = 2;
                dashLine.Color = color;
                PlotControl.Refresh();
            });
        }

        #endregion


        #region 移除


        public void RemoveLineAt(int index)
        {
            // 获取所有文本（按添加顺序）转成List
            var allLines = PlotControl.Plot.GetPlottables<ScottPlot.Plottables.LinePlot>().ToList();

            // 判断：
            if (allLines.Count > index)
            {
                var fifthArrow = allLines[index]; // 
                PlotControl.Plot.Remove(fifthArrow);
                PlotControl.Refresh();
            }
        }
        public void RemoveLineOver(int index)
        {
            var lineCount = PlotControl.Plot.GetPlottables<ScottPlot.Plottables.LinePlot>().Count();
            for (int i = lineCount - 1; i >= index; i--)
            {
                RemoveLineAt(i);
            }

        }
        public void RemoveArrowAt(int index)
        {
            // 获取所有箭头（按添加顺序）转成List
            var allArrows = PlotControl.Plot.GetPlottables<ScottPlot.Plottables.Arrow>().ToList();

            // 判断：
            if (allArrows.Count > index)
            {
                var fifthArrow = allArrows[index]; // 
                PlotControl.Plot.Remove(fifthArrow);
                PlotControl.Refresh();
            }
        }
        public void RemoveArrowOver(int index)
        {
            var arrowCount = PlotControl.Plot.GetPlottables<ScottPlot.Plottables.Arrow>().Count();
            for (int i = arrowCount - 1; i >= index; i--)
            {
                RemoveArrowAt(i);
            }


        }
        public void RemoveTextAt(int index)
        {
            // 获取所有文本（按添加顺序）转成List
            var allTexts = PlotControl.Plot.GetPlottables<ScottPlot.Plottables.Text>().ToList();

            // 判断：
            if (allTexts.Count > index)
            {
                var fifthArrow = allTexts[index]; // 
                PlotControl.Plot.Remove(fifthArrow);
                PlotControl.Refresh();
            }
        }
        public void RemoveTextOver(int index)
        {
            var textCount = PlotControl.Plot.GetPlottables<ScottPlot.Plottables.Text>().Count();
            for (int i = textCount - 1; i >= index; i--)
            {
                RemoveTextAt(i);
            }
        }

        #endregion




        public void Clear()
        {
            PlotControl.Plot.Remove<ScottPlot.Plottables.Arrow>(); //全部箭头删掉
            PlotControl.Plot.Remove<ScottPlot.Plottables.Text>();  //全部坐标文本删掉
            PlotControl.Plot.Remove<ScottPlot.Plottables.Marker>();
            PlotControl.Plot.Remove<ScottPlot.Plottables.LinePlot>();

            PlotControl.Refresh();
        }
    }
}
