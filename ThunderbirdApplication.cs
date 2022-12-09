using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Kstudio.Forms;
using Kstudio.Forms.WinApi;
using Timer = System.Timers.Timer;

namespace ThunderbirdToTray
{
    internal class ThunderbirdApplication
    {
        private const string THUNDERBIRD_NAME = "thunderbird";
        public IntPtr Handle = IntPtr.Zero;
        private static Timer _timer;
        private string _path;
        private bool _isMaximized;
        private bool _isOpening;
        private bool _closing;


        public ThunderbirdApplication()
        {
            if (_timer == null)
                _timer = new Timer();

            var processes = Process.GetProcessesByName(THUNDERBIRD_NAME);
            if (processes.Length == 0)
                _timer.Interval = 30 * 1000;
            else
                _timer.Interval = 250;

            _path = @"D:\ProgramData\MozillaThunderbird\thunderbird.exe";
            if (!File.Exists(_path))
                _path = null;

            _timer.Elapsed += this._timer_Elapsed;
            _timer.Start();
        }

        public void Exit()
        {
            _closing = true;
            _timer?.Stop();
            _timer?.Close();
            _timer?.Dispose();
            _timer = null;
            Visible = true;
            _closing = false;
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
                if (!User32.IsWindow(Handle))
                {
                    if (value && !_closing && !string.IsNullOrWhiteSpace(_path) && File.Exists(_path))
                    {
                        RunThunderBird();
                    }
                }
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

                        User32.SetForegroundWindow(Handle);
                        User32.SetActiveWindow(Handle);
                        User32.SetFocus(Handle);
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
            if (_timer.Interval > 250)
                _timer.Interval = 250;

            Find();

            if (User32.IsWindow(Handle))
            {
                if (IsMinimized && Visible)
                    Hide();
            }
            _timer.Start();
        }


        private void Find(int attemps = 10)
        {
            if (User32.IsWindow(Handle))
                return;

            var processes = Process.GetProcessesByName(THUNDERBIRD_NAME).ToList();
            processes.RemoveAll(proc => proc.HasExited);
            //attemps = 10;
            if (processes.Count == 0)
            {
                return;
            }

            attemps = 10;
            foreach (var process in processes)
            {
                if (process.HasExited)
                    continue;

                if (string.IsNullOrWhiteSpace(_path))
                    _path = process.MainModule?.FileName;

                while (!process.HasExited && process.MainWindowHandle == IntPtr.Zero && attemps > 0)
                {
                    attemps--;
                    Thread.Sleep(100);
                    if (!process.HasExited)
                        process.Refresh();
                }

                if (process.HasExited)
                    continue;

                if (User32.IsWindow(process.MainWindowHandle))
                {
                    Handle = process.MainWindowHandle;
                    break;
                }
            }

            if (_isOpening)
                return;

            processes = Process.GetProcessesByName(THUNDERBIRD_NAME).ToList();
            processes.RemoveAll(proc => proc.HasExited);

            if (processes.Count == 0)
                return;

            if (!User32.IsWindow(Handle) && !string.IsNullOrWhiteSpace(_path) && File.Exists(_path))
            {
                RunThunderBird();
            }

        }

        private void RunThunderBird()
        {
            if (_isOpening || _closing)
                return;

            _isOpening = true;
            var process = Process.Start(_path);
            process?.WaitForInputIdle(1000);

            var attemps = 10;
            while (attemps > 0)
            {
                if (_closing || User32.IsWindow(Handle))
                    break;

                attemps--;
                Thread.Sleep(100);
                if (!_closing && !User32.IsWindow(Handle))
                    Find(attemps);

            }

            var processes = Process.GetProcessesByName(THUNDERBIRD_NAME).ToList();
            processes.RemoveAll(proc => proc.HasExited);
            processes.RemoveAll(proc => proc.MainWindowHandle == Handle);
            foreach (var proc in processes)
            {
                if (!proc.HasExited)
                {
                    proc.CloseMainWindow();
                    if (!proc.HasExited)
                        proc.Close();
                }
            }
            //Find();
            Hide();
            _isOpening = false;
        }
    }
}
