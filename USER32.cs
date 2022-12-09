using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Kstudio.Forms.WinApi
{
    public static class User32
    {
        public enum WindowState
        {
            SW_FORCEMINIMIZE = 11,

            //Minimizes a window, even if the thread that owns the window is not responding. This flag should only be used when minimizing windows from a different thread.
            SW_HIDE = 0,

            //Hides the window and activates another window.
            SW_MAXIMIZE = 3,

            //Maximizes the specified window.
            SW_MINIMIZE = 6,

            //Minimizes the specified window and activates the next top-level window in the Z order.
            SW_RESTORE = 9,

            /// <summary>
            ///     Activates and displays the window.
            ///     If the window is minimized or maximized, the system restores it to its original size and position.
            ///     An application should specify this flag when restoring a minimized window.
            /// </summary>
            SW_SHOW = 5,

            /// <summary>
            ///     Activates the window and displays it in its current size and position.
            /// </summary>
            SW_SHOWDEFAULT = 10,

            /// <summary>
            ///     Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess
            ///     function by the program that started the application.
            /// </summary>
            SW_SHOWMAXIMIZED = 3,

            //Activates the window and displays it as a maximized window.
            SW_SHOWMINIMIZED = 2,

            //Activates the window and displays it as a minimized window.
            SW_SHOWMINNOACTIVE = 7,

            //Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated.
            SW_SHOWNA = 8,

            //Displays the window in its current size and position. This value is similar to SW_SHOW, except that the window is not activated.
            SW_SHOWNOACTIVATE = 4,

            //Displays a window in its most recent size and position. This value is similar to SW_SHOWNORMAL, except that the window is not activated.
            SW_SHOWNORMAL = 1,

            WS_MINIMIZE = 0x20000000,
            WS_MAXIMIZE = 0x1000000
        }


        #region Window State

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, WindowState nCmdShow);

        #endregion Window State


        /// <summary>
        ///     Sets the keyboard focus to the specified window. <br />
        ///     The window must be attached to the calling thread's message queue.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr FindWindow(string className, string caption);


        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr parentWindow, IntPtr previousChildWindow, string windowClass, string windowTitle);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr window, out int process);

        public static IntPtr[] GetProcessWindows(int process, IntPtr parentWindow)
        {
            var apRet = new IntPtr[256];
            var iCount = 0;
            IntPtr pLast = IntPtr.Zero;
            do
            {
                pLast = FindWindowEx(parentWindow, pLast, null, null);
                GetWindowThreadProcessId(pLast, out int iProcess);
                if (iProcess == process)
                    apRet[iCount++] = pLast;
            } while (pLast != IntPtr.Zero);

            Array.Resize(ref apRet, iCount);
            return apRet;
        }

        /// <summary>
        ///     Determines whether the specified window handle identifies an existing window.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int LockWindowUpdate(IntPtr hwndLock);


        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        /// <summary>
        ///     The GetForegroundWindow function returns a handle to the foreground window.
        /// </summary>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        ///     Activates a window.<br />
        ///     The window must be attached to the calling thread's message queue.
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);


        [DllImport("user32.dll")]
        public static extern IntPtr SetForegroundWindow(IntPtr hWnd);


        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);


        #region Styles

        [Flags]
        public enum WindowStyleFlags
        {
            OVERLAPPED = 0x00000000,
            POPUP = unchecked((int)0x80000000),
            CHILD = 0x40000000,
            MINIMIZE = 0x20000000,
            VISIBLE = 0x10000000,
            DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            MAXIMIZE = 0x01000000,
            WS_BORDER = 0x00800000,
            DLGFRAME = 0x00400000,
            VSCROLL = 0x00200000,
            HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            THICKFRAME = 0x00040000,
            GROUP = 0x00020000,
            TABSTOP = 0x00010000,
            MINIMIZEBOX = 0x00020000,
            MAXIMIZEBOX = 0x00010000,
            CAPTION = WS_BORDER | DLGFRAME,
            TILED = OVERLAPPED,
            ICONIC = MINIMIZE,
            SIZEBOX = THICKFRAME,
            TILEDWINDOW = OVERLAPPEDWINDOW,
            OVERLAPPEDWINDOW = OVERLAPPED | CAPTION | WS_SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX,
            POPUPWINDOW = POPUP | WS_BORDER | WS_SYSMENU,
            CHILDWINDOW = CHILD
        }

        [Flags]
        public enum WindowExStyleFlags : uint
        {
            ACCEPTFILES = 0x00000010,
            APPWINDOW = 0x00040000,
            WS_EX_CLIENTEDGE = 0x00000200,

            /// <summary>
            ///     Paints all descendants of a window in bottom-to-top painting order using double-buffering.
            ///     <para> For more information, see Remarks. </para>
            ///     This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC.
            ///     <para>https://msdn.microsoft.com/ru-ru/library/windows/desktop/ff700543.aspx</para>
            /// </summary>
            WS_EX_COMPOSITED = 0x02000000,
            CONTEXTHELP = 0x00000400,
            CONTROLPARENT = 0x00010000,
            DLGMODALFRAME = 0x00000001,
            LAYERED = 0x00080000,
            LAYOUTRTL = 0x00400000,
            LEFT = 0x00000000,
            LEFTSCROLLBAR = 0x00004000,
            LTRREADING = 0x00000000,
            MDICHILD = 0x00000040,
            NOACTIVATE = 0x08000000,

            /// <summary>
            ///     The window does not pass its window layout to its child windows.
            /// </summary>
            WS_EX_NOINHERITLAYOUT = 0x00100000,
            NOPARENTNOTIFY = 0x00000004,
            OVERLAPPEDWINDOW = WINDOWEDGE | WS_EX_CLIENTEDGE,
            PALETTEWINDOW = WINDOWEDGE | TOOLWINDOW | TOPMOST,
            RIGHT = 0x00001000,
            RIGHTSCROLLBAR = 0x00000000,
            RTLREADING = 0x00002000,
            STATICEDGE = 0x00020000,
            TOOLWINDOW = 0x00000080,
            TOPMOST = 0x00000008,
            TRANSPARENT = 0x00000020,
            WINDOWEDGE = 0x00000100
        }

        /// <summary>
        ///     Window style information
        /// </summary>
        public enum GWL
        {
            GWL_WNDPROC = -4,
            GWL_HINSTANCE = -6,

            /// <summary>
            ///     -8
            /// </summary>
            GWL_HWNDPARENT = -8,

            /// <summary>
            ///     -16
            /// </summary>
            GWL_STYLE = -16,
            GWL_EXSTYLE = -20,
            GWL_USERDATA = -21,
            GWL_ID = -12
        }


        public static int GetWindowLong(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 4) return (int)GetWindowLong32(hWnd, nIndex);
            return (int)(long)GetWindowLongPtr64(hWnd, nIndex);
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        private static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        #endregion Styles
    }
}
