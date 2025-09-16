using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FR_Operator
{
    internal class FormEnabled
    {
        private const int SC_CLOSE = 0xF060;
        private const int MF_GRAYED = 0x1;
        internal const int MF_ENABLED = 0x0;
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        private static extern int EnableMenuItem(IntPtr hMenu, int wIDEnableItem, int wEnable);
        [DllImport("user32.dll")]
        public static extern int RemoveMenu(int systemMenu, int itemPosition, int flag);
        [DllImport("user32.dll")]
        public static extern int GetMenuItemCount(int systemMenu);
        [DllImport("user32.dll")]
        public static extern int DrawMenuBar(IntPtr currentWindow);
        public void FormCloseEnabled(Form form, bool enabled)
        {
            MethodInvoker method = delegate
            {
                int disable = 2;
                int enable = 1;
                IntPtr menu;
                int itemCount;
                if (enabled)
                {
                    EnableMenuItem(GetSystemMenu(form.Handle, false), SC_CLOSE, MF_ENABLED);
                    //get the system menu of the application
                    menu = GetSystemMenu(form.Handle, false);
                    //get the count of menu items in the system menu
                    itemCount = GetMenuItemCount(menu.ToInt32());
                    //disable the "Close" command in the menu
                    RemoveMenu(menu.ToInt32(), itemCount - 1, enable);
                    //now draw the menu bar on the application
                    DrawMenuBar(form.Handle);
                }
                else
                {
                    EnableMenuItem(GetSystemMenu(form.Handle, false), SC_CLOSE, MF_GRAYED);
                    //get the system menu of the application
                    menu = GetSystemMenu(form.Handle, false);
                    //get the count of menu items in the system menu
                    itemCount = GetMenuItemCount(menu.ToInt32());
                    //disable the "Close" command in the menu
                    RemoveMenu(menu.ToInt32(), itemCount - 1, disable);
                    //now draw the menu bar on the application
                    DrawMenuBar(form.Handle);
                }
            };

            if (form.InvokeRequired)
                form.BeginInvoke(method);
            else
                method.Invoke();
        }
    }
}
