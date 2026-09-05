using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using SMDisLabSys.BLL;

namespace SMDisLabSys.UIServer
{
    public class UIBase : BindableBase
    {
        public DelegateCommand ExplainCommand { get; private set; }

        public string ExpDes = "";

        string connectItem = "未连接";
        public string ConnectItem
        {
            get { return connectItem; }
            set { SetProperty(ref connectItem, value); }
        }

        public UIBase()
        {
            ExplainCommand = new DelegateCommand(ExplainCommandMethod);

        }

        #region 蓝牙连接
        public void ConnectDevice(string bleName)
        {
            Thread.Sleep(500);//等有无USB数据
            if (SMDataSource.Instance.HidConnected())
            {
                return;
            }
            Task.Run(() =>
            {
                int loop = 100;
                while (loop > 0)//
                {
                    if (SMDataSource.Instance.BluetoothList.Count > 0)
                    {
                        var bluetooth = SMDataSource.Instance.BluetoothList.Where(o => o.Adresse.Contains(bleName)).FirstOrDefault();
                        if (bluetooth != null)
                        {
                            List<BluetoothInfo> selectList = new List<BluetoothInfo>();
                            selectList.Add(bluetooth);
                            SMDataSource.Instance.BluetoothConnect(selectList);
                            break;
                        }

                    }
                    Thread.Sleep(500);//50s

                    loop--;
                }
            });
        }
        #endregion

        #region 命令发送
        public void SendMethod(byte[] buffer)
        {
            if (SMDataSource.Instance.HidConnected())
            {
                SMDataSource.Instance.hid1.SendBuffer(buffer);
            }
            else
            {
                if (SMDataSource.Instance.BluetoothList.Count > 0)
                {
                    SMDataSource.Instance.SendCommandBle(buffer);
                }
            }
        }
        #endregion

        void ExplainCommandMethod()
        {
            if (!File.Exists(ExpDes))
            {
                MessageBox.Show("文件不存在");
                return;
            }
            try
            {
                Process.Start(new ProcessStartInfo(ExpDes) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开失败：{ex.Message}\n请确认电脑已安装Microsoft Word");
            }
        }
    }
}
