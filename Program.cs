using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Kstudio.Forms.WinApi;

namespace ThunderbirdToTray
{
    static class Program
    {
        static readonly NotifyIcon notifyIcon = new NotifyIcon();
        private static ThunderbirdApplication application;

        [DllImport("Kernel32")]
        private static extern IntPtr GetConsoleWindow();

        static void Main(string[] args)
        {
            var hwnd = GetConsoleWindow();
            User32.ShowWindow(hwnd, User32.WindowState.SW_HIDE);


            application = new ThunderbirdApplication();
            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            notifyIcon.Click += (s, e) =>
            {
                application.Visible = !application.Visible;
            };

            notifyIcon.Visible = true;
            notifyIcon.Text = Application.ProductName;

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Exit", null, (s, e) =>
            {
                application.Visible = true;
                Application.Exit();
            });

            notifyIcon.ContextMenuStrip = contextMenu;

            //Console.WriteLine("Running!");

            Application.Run();

            notifyIcon.Visible = false;
        }

    }
}
