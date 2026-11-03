using SMDisLabSys.Common;
using SMDisLabSys.UIServer;
using System.Windows;
using System.Windows.Input;

namespace SMDisLabSys.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            KillSameProcess.KillRunningInstance();
            InitializeComponent();
            Title = "实验系统";
            WindowTaskbarHelper.Attach(this, "实验系统");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (ResizeMode != ResizeMode.CanResize && ResizeMode != ResizeMode.CanResizeWithGrip) return;
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
            else
            {
                DragMove();
            }
        }
    }
}
