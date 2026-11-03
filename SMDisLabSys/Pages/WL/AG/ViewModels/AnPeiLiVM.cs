using Prism.Commands;
using Prism.Services.Dialogs;
using SMDisLabSys.BLL.RealData;
using SMDisLabSys.Model;
using SMDisLabSys.UIServer;
using System;
using System.Threading;
using System.Threading.Tasks;
using static SMDisLabSys.BLL.RealData.RealDataBLE;

namespace SMDisLabSys.Pages.WL.AG.ViewModels
{
    class AnPeiLiVM : UIChartBase, IDialogAware
    {
        const byte ProductLow = 0x03;
        const byte ProductHigh = 0x00;
        const byte CmdNear = 0x11;
        const byte CmdFar = 0x22;
        const byte CmdRotate = 0x33;
        const byte CmdCycle = 0x99;
        const byte CmdCurrentUp = 0x66;
        const byte CmdCurrentDown = 0x55;
        const int SendIntervalMs = 100;
        const byte CurrentMax = 100;

        double xAxis = 0;
        byte currentXx = 0;
        CancellationTokenSource pressCts;
        readonly object sendLock = new object();
        volatile bool isPressing;

        public DelegateCommand<string> PressCommand { get; private set; }
        public DelegateCommand<string> ReleaseCommand { get; private set; }

        #region 属性
        double f;
        public double F
        {
            get { return f; }
            set { SetProperty(ref f, value); }
        }
        double b;
        public double B
        {
            get { return b; }
            set { SetProperty(ref b, value); }
        }
        double i;
        public double I
        {
            get { return i; }
            set { SetProperty(ref i, value); }
        }
        double l;
        public double L
        {
            get { return l; }
            set { SetProperty(ref l, value); }
        }
        double theta;
        public double Theta
        {
            get { return theta; }
            set { SetProperty(ref theta, value); }
        }
        #endregion

        public AnPeiLiVM()
        {
            XFormatter = XFormatters;
            YFormatter = YFormatters;
            ChartTitle = "安培力";
            YLineTitle = "F";
            InitAxis();
            CreatLinePoint("F");
            PressCommand = new DelegateCommand<string>(PressCommandMethod);
            ReleaseCommand = new DelegateCommand<string>(ReleaseCommandMethod);
            ConnectDevice("SHM");
            RealDataBLE.Instance.BLEDataUpdated += Instance_BLEDataUpdated;
        }

        void InitAxis()
        {
            Min = 0;
            Max = 10;
            MaxY = 1;
            MinY = -1;
        }

        private void Instance_BLEDataUpdated(object? sender, EventArgs e)
        {
            DataParseEventArgs args = (DataParseEventArgs)e;
            int index = 0;
            foreach (var item in args.ParamListDic)
            {
                if (item.Value == null || item.Value.Count == 0)
                {
                    index++;
                    continue;
                }
                if (index == 0)
                {
                    F = item.Value[0];
                }
                else if (index == 1)
                {
                    B = item.Value[0];
                }
                else if (index == 2)
                {
                    I = item.Value[0];
                }
                else if (index == 3)
                {
                    L = item.Value[0];
                }
                else if (index == 4)
                {
                    Theta = item.Value[0];
                }
                index++;
            }

            if (args.ConnectType == ConnectTypeEnum.USB)
            {
                haveUSBData = true;
            }

            AddPointSameMaxYMinY(xAxis, F, 0);
            xAxis += 1;
            if (xAxis > Max)
            {
                Max = xAxis;
            }
        }

        void PressCommandMethod(string action)
        {
            if (!TryGetCmd(action, out byte cmd, out bool currentUp, out bool currentDown))
            {
                return;
            }
            StartRepeat(cmd, currentUp, currentDown);
        }

        void ReleaseCommandMethod(string action)
        {
            if (!TryGetCmd(action, out byte cmd, out _, out _))
            {
                return;
            }
            StopRepeat(cmd);
        }

        static bool TryGetCmd(string action, out byte cmd, out bool currentUp, out bool currentDown)
        {
            cmd = 0;
            currentUp = false;
            currentDown = false;
            switch (action)
            {
                case "Near":
                    cmd = CmdNear;
                    return true;
                case "Far":
                    cmd = CmdFar;
                    return true;
                case "Rotate":
                    cmd = CmdRotate;
                    return true;
                case "Cycle":
                    cmd = CmdCycle;
                    return true;
                case "CurrentUp":
                    cmd = CmdCurrentUp;
                    currentUp = true;
                    return true;
                case "CurrentDown":
                    cmd = CmdCurrentDown;
                    currentDown = true;
                    return true;
                default:
                    return false;
            }
        }

        void StartRepeat(byte cmd, bool currentUp, bool currentDown)
        {
            lock (sendLock)
            {
                CancelRepeatLocked();
                isPressing = true;
                pressCts = new CancellationTokenSource();
                var token = pressCts.Token;
                Task.Run(async () =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        byte dataLow = 0x01;
                        if (currentUp)
                        {
                            if (currentXx < CurrentMax)
                            {
                                currentXx++;
                            }
                            dataLow = currentXx;
                        }
                        else if (currentDown)
                        {
                            if (currentXx > 0)
                            {
                                currentXx--;
                            }
                            dataLow = currentXx;
                        }
                        SendFrame(cmd, 0x00, dataLow);
                        try
                        {
                            await Task.Delay(SendIntervalMs, token);
                        }
                        catch (TaskCanceledException)
                        {
                            break;
                        }
                    }
                }, token);
            }
        }

        void StopRepeat(byte cmd)
        {
            lock (sendLock)
            {
                if (!isPressing)
                {
                    return;
                }
                CancelRepeatLocked();
                isPressing = false;
            }
            SendFrame(cmd, 0x00, 0x00);
        }

        void CancelRepeatLocked()
        {
            if (pressCts != null)
            {
                try
                {
                    pressCts.Cancel();
                    pressCts.Dispose();
                }
                catch
                {
                }
                pressCts = null;
            }
        }

        void SendFrame(byte cmd, byte dataHigh, byte dataLow)
        {
            // CE CB + 产品编码 03 00 + 指令 + 数据2字节
            SendMethod(new byte[] {0x07, 0xCE, 0xCB, ProductLow, ProductHigh, cmd, dataHigh, dataLow });
        }

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
            RealDataBLE.Instance.BLEDataUpdated -= Instance_BLEDataUpdated;
            lock (sendLock)
            {
                CancelRepeatLocked();
                isPressing = false;
            }
        }
        public void OnDialogOpened(IDialogParameters parameters)
        {
            Title = "安培力研究";
            Width = 1350;
            Height = 820;
        }
        #endregion
    }
}
