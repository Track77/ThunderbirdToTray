using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Kstudio.Forms;
using Kstudio.Forms.WinApi;
using Timer = System.Timers.Timer;

namespace ThunderbirdToTray
{
    class ThunderbirdApplication
    {
        private const string THUNDERBIRD_NAME = "thunderbird";
        public IntPtr Handle = IntPtr.Zero;
        private readonly Timer _timer;
        private string _path;
        private bool _isMaximized;


        public ThunderbirdApplication()
        {
            _timer = new Timer();
            _timer.Interval = 250;
            _timer.Elapsed += this._timer_Elapsed;
            _timer.Start();
        }


        public bool Visible
        {
            get
            {
                if (User32.IsWindow(Handle))
                {
                    return User32.IsWindowVisible(Handle);
                }

                return false;
            }
            set
            {
                if (User32.IsWindow(Handle))
                {
                    if (Visible)
                    {
                        var style = User32.GetWindowLong(Handle, (int)User32.GWL.GWL_STYLE);
                        _isMaximized = (style & (int)User32.WindowState.WS_MAXIMIZE) == (int)User32.WindowState.WS_MAXIMIZE;
                    }
                    if (value)
                    {
                        if (_isMaximized)
                            User32.ShowWindow(Handle, User32.WindowState.SW_MAXIMIZE);
                        else
                            User32.ShowWindow(Handle, User32.WindowState.SW_RESTORE);
                    }
                    else
                    {
                        User32.ShowWindow(Handle, User32.WindowState.SW_HIDE);
                    }

                }
            }
        }


        public bool IsMinimized
        {
            get
            {
                var style = User32.GetWindowLong(Handle, (int)User32.GWL.GWL_STYLE);
                return (style & (int)User32.WindowState.WS_MINIMIZE) == (int)User32.WindowState.WS_MINIMIZE;
            }
        }

        public void Hide()
        {
            Visible = false;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();

            Find();

            if (User32.IsWindow(Handle))
            {
                if (IsMinimized && Visible)
                    Hide();
            }
            _timer.Start();
        }


        void Find()
        {
            if (User32.IsWindow(Handle))
                return;

            var processes = Process.GetProcessesByName(THUNDERBIRD_NAME);
            if (processes.Length == 0)
                return;

            foreach (var process in processes)
            {
                if (string.IsNullOrWhiteSpace(_path))
                    _path = process.MainModule?.FileName;

                if (User32.IsWindow(process.MainWindowHandle))
                {
                    Handle = process.MainWindowHandle;
                    break;
                }
            }
            if (!User32.IsWindow(Handle) && !string.IsNullOrWhiteSpace(_path) && File.Exists(_path))
            {
                Process.Start(_path);
                Find();
                Hide();
            }
        }
    }
}
