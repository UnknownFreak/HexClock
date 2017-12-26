using DesktopApplication.ColorPicker;
using DesktopApplication.Windows;
using HexClock.Properties;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media;

namespace HexClock
{
    class ClockTrayIcon : ApplicationContext
    {
        private const string SettingsFile = "ClockApp.settings";
        private NotifyIcon trayIcon;
        private ClockWindow window;
        public ClockTrayIcon(ClockWindow window)
        {
            this.window = window;
            // Initialize Tray Icon
            MenuItem exit_button = new MenuItem("Exit", Exit);

            exit_button.Click += window.Kill;
            trayIcon = new NotifyIcon()
            {
                Icon = Resources.Hex,
                ContextMenu = new ContextMenu(new MenuItem[] {
                new MenuItem("Save Settings", saveSettings),
                new MenuItem("Default Color", defaultColor),
                new MenuItem("Set Color", setColor),
                exit_button
            }),
                Visible = true
            };
        }

        void setColor(object sender, EventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            Brush b = bc.ConvertFromString("#FFFFFFFF") as Brush;
            ColorPicker cp = new ColorPicker();
            cp.DataContext = b;
            cp.ShowDialog();
            window.UpdateColor(b);
            Settings.Default.CurrentColor = b.ToString();

        }

        void defaultColor(object s, EventArgs e)
        {
            Settings.Default.Reset();
            window.UpdateColor(new BrushConverter().ConvertFromString(Settings.Default.CurrentColor) as Brush);
        }

        void saveSettings(object s, EventArgs e)
        {
            using (FileStream fs = File.Open(SettingsFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
            {
                Byte[] data = new UTF8Encoding(true).GetBytes("Color:" + Settings.Default.CurrentColor);
                fs.Write(data, 0, data.Length);
            }

        }

        public static string readColorFromSettings()
        {
            string color = "";
            try
            {

            using (StreamReader sr = File.OpenText(SettingsFile))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    string[] split = s.Split(':');
                    if (split[0] == "Color")
                    {
                        color = split[1];
                        break;
                    }
                }
            }
            }
            catch (FileNotFoundException e)
            { }
            return color;
        }

        public void Exit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
        }

    }
}
