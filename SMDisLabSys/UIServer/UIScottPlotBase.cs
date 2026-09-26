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

        public void CreatArrow(double startX, double startY, double endX, double endY, Color color, string title = "", double titleX = 0, double titleY = 0)
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


            var txt = PlotControl.Plot.Add.Text(title, titleX, titleY);
            txt.LabelFontSize = 25;

            //PlotControl.Plot.Axes.AutoScale();
            PlotControl.Refresh(); // 新增后必须刷新
        }

        public void RemoveArrowAt(int index)
        {
            // 获取所有箭头（按添加顺序）转成List
            var allArrows = PlotControl.Plot.GetPlottables<ScottPlot.Plottables.Arrow>().ToList();

            // 判断：必须确认至少有5个箭头
            if (allArrows.Count > index)
            {
                var fifthArrow = allArrows[index]; // 索引4 = 第5个
                PlotControl.Plot.Remove(fifthArrow);
                PlotControl.Refresh();
            }
        }
        public void RemoveTextAt(int index)
        {
            // 获取所有箭头（按添加顺序）转成List
            var allTexts = PlotControl.Plot.GetPlottables<ScottPlot.Plottables.Text>().ToList();

            // 判断：必须确认至少有5个箭头
            if (allTexts.Count > index)
            {
                var fifthArrow = allTexts[index]; // 索引4 = 第5个
                PlotControl.Plot.Remove(fifthArrow);
                PlotControl.Refresh();
            }
        }
        public void RemoveArrowAndTextOver(int index)
        {
            var arrowCount = PlotControl.Plot.GetPlottables<ScottPlot.Plottables.Arrow>().Count();
            for (int i = arrowCount - 1; i >= index; i--)
            {
                RemoveArrowAt(i);
            }

            var textCount = PlotControl.Plot.GetPlottables<ScottPlot.Plottables.Text>().Count();
            for (int i = arrowCount - 1; i >= index; i--)
            {
                RemoveTextAt(i);
            }

        }

        public void CreatMarker(double startX, double startY)
        {
            PlotControl.Plot.Add.Marker(startX, startY, MarkerShape.FilledCircle, size: 12, color: Colors.Black);
        }

        public void CreatLineDashed(double startX, double startY, double endX, double endY)
        {

            var dashLine = PlotControl.Plot.Add.Line(startX, startY, endX, endY);
            dashLine.LinePattern = LinePattern.Dashed; //虚线
            dashLine.LineWidth = 2;
            dashLine.Color = Colors.DarkGray;
            PlotControl.Refresh();
        }

        public void Clear()
        {
            PlotControl.Plot.Remove<ScottPlot.Plottables.Arrow>(); //全部箭头删掉
            PlotControl.Plot.Remove<ScottPlot.Plottables.Text>();  //全部坐标文本删掉
            PlotControl.Plot.Remove<ScottPlot.Plottables.Marker>();
        }
    }
}
