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

        public void CreatArrow(int startX, int startY, int endX, int endY)
        {
            // 示例：新增一个蓝色箭头
            var newArrow = PlotControl.Plot.Add.Arrow(startX, startY, endX, endY);
            newArrow.ArrowLineColor = Colors.Blue;
            newArrow.ArrowFillColor = Colors.Blue;
            newArrow.ArrowheadLength = 12;

            ArrowList.Add(newArrow);
            PlotControl.Plot.Axes.AutoScale();
            PlotControl.Refresh(); // 新增后必须刷新
        }
    }
}
