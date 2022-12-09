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


        static void Main(string[] args)
        {
            application = new ThunderbirdApplication();
            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            var contextMenu = new ContextMenuStrip();
            notifyIcon.Click += (s, e) =>
            {
                if (contextMenu.Visible)
                    return;

                var visible = application.Visible;
                application.Visible = !visible;
            };

            notifyIcon.Visible = true;
            notifyIcon.Text = Application.ProductName;

            contextMenu.Items.Add("Exit", null, (s, e) =>
            {
                //application.Visible = true;
                application.Exit();
                Application.Exit();
            });

            notifyIcon.ContextMenuStrip = contextMenu;

            //Console.WriteLine("Running!");

            Application.Run();

            notifyIcon.Visible = false;
        }

    }
}
