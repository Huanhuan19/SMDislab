using SMDisLabSys.Common;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMDisLabSys.Views;
using System.Threading;
using SMDisLabSys.Common.ConfigSys;
using NPOI.SS.Formula.Functions;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using SMDisLabSys.BLL;
using SMDisLabSys.Pages;
using Prism.Services.Dialogs;
using SMDisLabSys.Pages.WL.Views;


namespace SMDisLabSys.ViewModels
{
    public class MainWindowVM : BindableBase
    {
        IRegionManager _regionManage;
        private readonly IDialogService dialogService;
        MainWindow main;
        public DelegateCommand WLCommand { get; private set; }

        #region 属性
        

        #endregion

        public MainWindowVM(IRegionManager regionManager, IDialogService pdialogService)
        {
            _regionManage = regionManager;
            dialogService = pdialogService;
            WLCommand = new DelegateCommand(SendPlanCommondMethod);
        }
        void SendPlanCommondMethod()
        {
            //List<BluetoothInfo> selectList = new List<BluetoothInfo>();
            //selectList.Add(new BluetoothInfo() { MAC = "BluetoothLE#BluetoothLE8c:e9:ee:9d:4d:37-c8:47:80:37:c6:3f", Adresse = "SHM:020_C63F" });
            //SMDataSource.Instance.BluetoothConnect(selectList);

            dialogService.Show(nameof(WLMain), null, callback =>
            {
            }, nameof(UcWinSize));
        }

        void LoadCMDWin(string cmdView)
        {
            _regionManage.Regions["CMDRegion"].RequestNavigate(cmdView);
        }
        void LoadView()
        {

        }
        
        public void Init(MainWindow mainWindow)
        {
            main = mainWindow;
            SystemInit.Instance.Init();
        }
    }
}
