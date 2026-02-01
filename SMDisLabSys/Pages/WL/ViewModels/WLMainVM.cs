using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SMDisLabSys.Pages.WL.AG.Views;
using SMDisLabSys.Pages.WL.Views;

namespace SMDisLabSys.Pages.WL.ViewModels
{
    public class WLMainVM : BindableBase, IDialogAware
    {
        private readonly IDialogService dialogService;
        public DelegateCommand APLCommand { get; private set; }//安培力
        public DelegateCommand DanBaiCommand { get; private set; }//单摆

        public WLMainVM(IDialogService pdialogService)
        {
            dialogService = pdialogService;
            HMACMD();
        }
        void HMACMD()
        {
            APLCommand = new DelegateCommand(APLCommandMethod);
            DanBaiCommand = new DelegateCommand(DanBaiCommandMethod);
        }

        #region A-H
        void APLCommandMethod()
        {
            
        }
        void DanBaiCommandMethod()
        {
            dialogService.Show(nameof(DanBaiNew), null, callback =>
            {
            }, nameof(UcWinSize));
        }
        #endregion

        #region IDialogAware接口实现
        string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
        int height;
        public int Height
        {
            get { return height; }
            set { SetProperty(ref height, value); }
        }
        int width;
        public int Width
        {
            get { return width; }
            set { SetProperty(ref width, value); }
        }
        public event Action<IDialogResult> RequestClose;
        public bool CanCloseDialog()
        {
            return true;
        }
        public void OnDialogClosed()
        {

        }
        public void OnDialogOpened(IDialogParameters parameters)
        {
            //if (parameters.ContainsKey("Title"))
            //{
            //    Title = (parameters.GetValue<string>("Title"));
            //}
            Title = "物理实验";
            Width = 1350;
            Height = 820;
        }
        #endregion
    }
}
