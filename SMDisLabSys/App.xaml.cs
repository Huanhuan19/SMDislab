using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using SMDisLabSys.ViewModels;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using SMDisLabSys.Views;
using System.Windows.Controls;
using Prism.Services.Dialogs;
using SMDisLabSys.Common;
using SMDisLabSys.Pages.WL.Views;
using SMDisLabSys.Pages.WL.ViewModels;
using SMDisLabSys.Pages;
using SMDisLabSys.Pages.WL.AG.Views;
using SMDisLabSys.Pages.WL;
using SMDisLabSys.Pages.WL.AG.ViewModels;


namespace SMDisLabSys
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            Startup += App_Startup;
            Exit += App_Exit;
        }
        private void App_Exit(object sender, ExitEventArgs e)
        {
            System.Environment.Exit(0);//线程都被强制退出
        }
        private void App_Startup(object sender, StartupEventArgs e)
        {
            //UI线程未捕获异常处理事件
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }
        /// <summary>
        /// UI线程未捕获异常处理函数
        /// </summary>
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true; //把 Handled 属性设为true，表示此异常已处理，程序可以继续运行，不会强制退出  
            LogMgr.Instance.Error("【UI线程异常】", e.Exception);
            //SysCacheHelper.cMainWindowVM.LoadingVisibility = System.Windows.Visibility.Collapsed;
        }
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<WLMain, WLMainVM>();
            containerRegistry.RegisterForNavigation<DanBai, DanBaiVM>();
            containerRegistry.RegisterForNavigation<DanBaiNew, DanBaiNewVM>();

            containerRegistry.Register<IDialogWindow, UcWinSize>(nameof(UcWinSize));
        }
        protected override Window CreateShell()
        {
            MainWindow mainWin = Container.Resolve<MainWindow>();
            (mainWin.DataContext as MainWindowVM).Init(mainWin);
            return mainWin;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var viewModelName = $"{viewName}VM, {viewAssemblyName}";
                return Type.GetType(viewModelName);
            });
        }
    }
}
