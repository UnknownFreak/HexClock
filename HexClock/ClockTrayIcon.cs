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
                Text = "Hex Clock",
                Icon = Resources.Hex,
                ContextMenu = new ContextMenu(new MenuItem[] {
                new MenuItem("Save Settings", SaveSettings),
                new MenuItem("Default Color", DefaultColor),
                new MenuItem("Set Color", SetColor),
                exit_button
            }),
                Visible = true
            };
        }

        void SetColor(object sender, EventArgs e)
        {
            Brush brush = new BrushConverter().ConvertFromString("#FFFFFFFF") as Brush;
            ColorPicker cp = new ColorPicker
            {
                DataContext = brush
            };
            cp.ShowDialog();
            window.UpdateColor(brush);
            Settings.Default.CurrentColor = brush.ToString();
        }

        void DefaultColor(object s, EventArgs e)
        {
            Settings.Default.Reset();
            window.UpdateColor(new BrushConverter().ConvertFromString(Settings.Default.CurrentColor) as Brush);
        }

        void SaveSettings(object s, EventArgs e)
        {
            using (FileStream fs = File.Open(SettingsFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
            {
                Byte[] data = new UTF8Encoding(true).GetBytes("Color:" + Settings.Default.CurrentColor);
                fs.Write(data, 0, data.Length);
            }

        }

        public static string ReadColorFromSettings()
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
            catch (FileNotFoundException)
            { }
            return color;
        }

        public void Exit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
        }

    }
}
