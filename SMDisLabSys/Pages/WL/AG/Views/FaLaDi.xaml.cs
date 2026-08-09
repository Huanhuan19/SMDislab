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

namespace SMDisLabSys.Pages.WL.AG.Views
{
    /// <summary>
    /// FaLaDi.xaml 的交互逻辑
    /// </summary>
    public partial class FaLaDi : UserControl
    {
        public FaLaDi()
        {
            InitializeComponent();

            // ---------- 在 Y=0 线上画 X 轴刻度 ----------
            double xMin = 0, xMax = 36;     // X 数据范围
            double tickStep = 5;           // X 轴刻度步长
            double yLine = -0;             // 中间X轴所在 Y 值

            for (double x = xMin; x <= xMax + 0.001; x += tickStep)
            {
                // 刻度标签（数字）
                Chart.VisualElements.Add(new VisualElement
                {
                    X = x,
                    Y = yLine,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    UIElement = new TextBlock
                    {
                        Text = x.ToString("0") + "s",
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008aff")),
                        FontSize = 26,
                        Margin = new Thickness(0, 2, 0, 0)
                    }
                });

                // 可选：短刻度竖线（可用 tiny LineSeries 或在 Canvas 上画，
                // VisualElement 本身不支持 Shape，若需要短线推荐用额外 LineSeries
                // 或把下面注释取消用 Rectangle 近似）
            }
        }
    }
}
