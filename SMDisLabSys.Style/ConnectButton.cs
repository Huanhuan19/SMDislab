using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SMDisLabSys.Styles
{
    public class ConnectButton : Button
    {
        public bool ConnectState
        {
            get { return (bool)GetValue(ConnectStateProperty); }
            set { SetValue(ConnectStateProperty, value); }
        }
        public static readonly DependencyProperty ConnectStateProperty = DependencyProperty.Register
            ("ConnectState", typeof(bool), typeof(ConnectButton), new PropertyMetadata((s, e) =>
            {
                var cb = s as ConnectButton;
                if (cb.ConnectState)//已连接
                {
                    cb.Background = new SolidColorBrush(Colors.DarkGreen);
                }
                else
                {
                    cb.Background = new SolidColorBrush(Colors.DarkRed);
                }
            }));
    }
    
}
