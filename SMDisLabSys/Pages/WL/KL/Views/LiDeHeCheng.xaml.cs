using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts.Wpf;
using LiveCharts;
using ScottPlot;
using Colors = ScottPlot.Colors;
using ScottPlot.Plottables;

namespace SMDisLabSys.Pages.WL.KL.Views
{
    /// <summary>
    /// LiDeHeCheng.xaml 的交互逻辑
    /// </summary>
    public partial class LiDeHeCheng : UserControl
    {
        public LiDeHeCheng()
        {
            InitializeComponent();

            //ArrowShape[] shapes = Enum.GetValues<ArrowShape>();
            //for (int i = 0; i < shapes.Length; i++)
            //{
            //    double y = 7 - i;
            //    var arrow = WpfPlot1.Plot.Add.Arrow(xBase: 1, yBase: y, xTip: 4, yTip: y);
            //    arrow.ArrowShape = shapes[i].GetShape();
            //    //arrow.ArrowLineWidth = 1;
            //    //arrow.ArrowFillColor = Colors.Blue;
            //    //arrow.ArrowLineColor = Colors.Black; h
            //    //arrow.ArrowheadLength = 20;
            //    //arrow.ArrowheadWidth = 15;
            //    //arrow.ArrowOffset = 8;

            //    arrow.ArrowShape = ArrowShape.Single.GetShape();
            //    arrow.ArrowFillColor = Colors.Red; //填充颜色
            //    arrow.ArrowLineColor = Colors.Red;        //线条颜色
            //    arrow.ArrowLineWidth = 1;                //箭杆+描边粗细（替代过时LineWidth）
            //    arrow.ArrowheadLength = 20;               //箭头三角长度
            //    arrow.ArrowheadWidth = 15;                 //箭头三角宽度
            //    arrow.ArrowOffset = 0;                    //箭头尖和目标点距离
            //    arrow.ArrowAnchor = ArrowAnchor.Tip;

            //    // 添加文字标签
            //    WpfPlot1.Plot.Add.Text(shapes[i].ToString(), 6, y);
            //}

            //WpfPlot1.Plot.Axes.SetLimits(0, 8, 0, 8);
            //WpfPlot1.Refresh();
        }
    }
}
