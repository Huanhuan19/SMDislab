using Prism.Services.Dialogs;
using SMDisLabSys.UIServer;
using System.Windows;
using System.Windows.Input;

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
            WindowTaskbarHelper.Attach(this);
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
