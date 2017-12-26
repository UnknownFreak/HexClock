using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DesktopApplication.Win32Addons
{
    public static class MouseHook
    {
        ///
        /// Plan was so user could toggle and move the clock located behind desktop.
        /// But never got that to work.
        ///
        
        public static event EventHandler MouseActionLUp = delegate { };
        public static event EventHandler MouseActionRUp = delegate { };
        public static event EventHandler MouseActionLDown = delegate { };
        public static event EventHandler MouseActionRDown = delegate { };
        public static void Start()
        {
            _hookID = SetHook(_proc);
        }
        public static void stop()
        {
            Win32Functions.UnhookWindowsHookEx(_hookID);
        }

        private static Win32Functions.HookProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private static IntPtr SetHook(Win32Functions.HookProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return Win32Functions.SetWindowsHookEx((IntPtr)WH_MOUSE_LL, proc,
                  Win32Functions.GetModuleHandle("user32"), 0);
            }
        }

        private static IntPtr HookCallback(
          int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && MouseMessages.WM_LBUTTONUP == (MouseMessages)wParam)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                MouseActionLUp(null, new EventArgs());
            }
            else if (nCode >= 0 && MouseMessages.WM_RBUTTONUP == (MouseMessages)wParam)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                MouseActionRUp(null, new EventArgs());
            }
            if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                MouseActionLDown(null, new EventArgs());
            }
            else if (nCode >= 0 && MouseMessages.WM_RBUTTONDOWN == (MouseMessages)wParam)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                MouseActionRDown(null, new EventArgs());
            }

            return Win32Functions.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public Win32Functions.POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
    }
}
