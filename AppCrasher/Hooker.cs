using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using UIAutomationClient;

namespace AppCrasher
{
    internal class Hooker
    {
        private IntPtr _hook = IntPtr.Zero;
        private IUIAutomationElement _element;
        private readonly AutomationEventHandler _automationEventHandler = new AutomationEventHandler();

        private class AutomationEventHandler : IUIAutomationPropertyChangedEventHandler
        {
            public void HandlePropertyChangedEvent(IUIAutomationElement sender, int propertyId, object newValue)
            {
            }
        }

        public void Hook()
        {
            if (_hook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hook);
                _hook = IntPtr.Zero;
            }

            IntPtr wndTarget = FindTargetWindow();
            if (wndTarget == IntPtr.Zero)
            {
                MessageBox.Show("Unable to find target window!");
                return;
            }

            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string assemblyDir = Path.GetDirectoryName(assemblyPath);
            string libraryPath = Path.Combine(assemblyDir, "EmbeddedCrasher.dll");
            IntPtr module = LoadLibrary(libraryPath);
            if (module == IntPtr.Zero)
            {
                MessageBox.Show("Unable to load EmbeddedCrasher.dll!");
                return;
            }

            IntPtr ptr = GetProcPtr();
            uint threadId = GetWindowThreadProcessId(wndTarget, out _);
            _hook = SetWindowsHookEx(HookType.WH_FOREGROUNDIDLE, ptr, module, threadId);
            if (_hook == IntPtr.Zero)
            {
                MessageBox.Show("Fail to set up the hook!");
                return;
            }
            SetForegroundWindow(wndTarget);

            WindowTitle = "Embedded Crasher";
            IntPtr wndEmbedded = IntPtr.Zero;
            while (wndEmbedded == IntPtr.Zero)
            {
                Thread.Sleep(100);
                wndEmbedded = FindTargetWindow();
            }

            IUIAutomation automation = new CUIAutomation8Class();
            _element = automation.ElementFromHandle(wndEmbedded);
            int[] props = { UIA_PropertyIds.UIA_BoundingRectanglePropertyId };
            automation.AddPropertyChangedEventHandler(_element, TreeScope.TreeScope_Element, null, _automationEventHandler, props);
        }

        public void Unhook()
        {
            if (_hook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hook);
            }
        }

        private static IntPtr FindTargetWindow()
        {
            EnumWindowsCallback callback = FindWindowCallback;
            IntPtr hWnd = FindFirstWindow(callback);
            return hWnd;
        }

        private static IntPtr FindFirstWindow(EnumWindowsCallback filter)
        {
            IntPtr found = IntPtr.Zero;

            EnumWindows(delegate (IntPtr wnd, IntPtr param)
            {
                if (filter(wnd, param))
                {
                    // only add the windows that pass the filter
                    found = wnd;
                    return false;
                }

                return true;
            }, IntPtr.Zero);

            return found;
        }

        private static bool FindWindowCallback(IntPtr handle, IntPtr extraParameter)
        {
            string title = GetWindowTitle(handle);
            if (title.StartsWith(WindowTitle))
                return true;

            return false;
        }

        private static string GetWindowTitle(IntPtr handle)
        {
            string windowText = "";
            int length = GetWindowTextLength(handle) * 2;
            StringBuilder builder = new StringBuilder(length);
            if (GetWindowText(handle, builder, builder.Capacity) != 0)
                windowText = builder.ToString();

            return windowText;
        }

        public static string WindowTitle { get; set; }

        delegate bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam);
        delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool EnumWindows(EnumWindowsCallback enumProc, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(HookType hookType, IntPtr lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("kernel32", SetLastError = true)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);
        [DllImport("EmbeddedCrasher.dll")]
        static extern IntPtr GetProcPtr();
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
