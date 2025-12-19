using Prism.Services.Dialogs;
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
using System.Windows.Shapes;

namespace SMDisLabSys.Pages
{
    /// <summary>
    /// UcAttMonitorPlanPopoutWinSize.xaml 的交互逻辑
    /// </summary>
    public partial class UcWinSize : Window, IDialogWindow
    {
        public UcWinSize()
        {
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseEvent));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Properties, WindowsEvent));
        }
        IDialogResult result;
        public IDialogResult Result
        {
            get { return result; }
            set { result = value; }
        }

        private void CloseEvent(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
        private void WindowsEvent(object sender, ExecutedRoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this); // this指的是当前窗口
        }
    }
}
