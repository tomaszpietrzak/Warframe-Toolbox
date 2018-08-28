using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Warframe_Toolbox
{
    class NotificationWindowViewModel
    {
        public NotificationWindowViewModel()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += CloseWindow;
            timer.Start();
        }

        public Action CloseAction { get; set; }

        private void CloseWindow(object sender, EventArgs e)
        {
            CloseAction();
        }

        string _notification;
        public string Notification
        {
            get { return _notification; }
            set { _notification = value; }
        }
    }
    
}
