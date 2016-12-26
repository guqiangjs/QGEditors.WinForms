﻿using System;
using System.Collections.Generic;
using System.Text;

namespace QGEditors.WinForms
{
    internal static class UnsafeNativeMethods
    {
        internal const int EC_LEFTMARGIN = 0x1;
        internal const int EC_RIGHTMARGIN = 0x2;
        internal const int EM_SETMARGINS = 0xD3;

        internal const int EM_SETRECT = 0xB3;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

    }
}
