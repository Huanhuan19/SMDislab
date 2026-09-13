using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;

namespace SMDisLabSys.UIServer
{
    public class UIScottPlotBase : UIBase
    {

        ObservableCollection<Arrow> ArrowList { get; set; } = new();

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

        public void CreatArrow(double startX, double startY, double endX, double endY, Color color, string title = "", double titleX = 0, double titleY = 0)
        {
            // 示例：新增一个蓝色箭头
            var newArrow = PlotControl.Plot.Add.Arrow(startX, startY, endX, endY);
            newArrow.ArrowShape = ArrowShape.Single.GetShape();
            newArrow.ArrowLineColor = color;
            newArrow.ArrowFillColor = color;

            newArrow.ArrowWidth = 5;
            newArrow.ArrowLineWidth = 0;
            newArrow.ArrowheadLength = 15;               //箭头三角长度
            newArrow.ArrowheadWidth = 20;                 //箭头三角宽度

            ArrowList.Add(newArrow);

            var txt = PlotControl.Plot.Add.Text(title, titleX, titleY);
            txt.LabelFontSize = 25;
            PlotControl.Plot.Axes.AutoScale();
            PlotControl.Refresh(); // 新增后必须刷新
        }
    }
}
