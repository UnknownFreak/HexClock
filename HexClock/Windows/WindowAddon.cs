using DesktopApplication.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DesktopApplication.Windows
{
    public static class WindowAddon
    {
        public static bool isInside(this Window window, Point point)
        {
            if ((point.X >= window.Left && point.X <= window.Left +window.Width) &&
                (point.Y >= window.Top && point.Y <= window.Left + window.Height))
                return true;
            return false;
        }
    }
}
