using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using static DesktopApplication.Win32Addons.Win32Functions;

namespace DesktopApplication.Windows
{
    /// <summary>
    /// Interaction logic for ClockWindow.xaml
    /// </summary>
    public partial class ClockWindow : Window
    {
        HexClock.ClockTrayIcon tray;
        DispatcherTimer timeUpdater;
        public ClockWindow()
        {

            InitializeComponent();
            Left = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - 250;
            Top = 0;
            UpdateTime(this, null);


            timeUpdater = new DispatcherTimer();
            timeUpdater.Tick += new EventHandler(UpdateTime);
            timeUpdater.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timeUpdater.Start();

            tray = new HexClock.ClockTrayIcon(this);

            string colorSetting = HexClock.ClockTrayIcon.ReadColorFromSettings();
            if (colorSetting != "")
                HexClock.Properties.Settings.Default.CurrentColor = colorSetting;
            UpdateColor(new BrushConverter().ConvertFromString(HexClock.Properties.Settings.Default.CurrentColor) as Brush);
        }

        private void UpdateTime(object sender, EventArgs e)
        {
            TimeModel.Time = GetTime;
            TimeModel.Date = GetDate;
            TimeModel.Date_y = GetDateYear;
        }

        private string GetSystemTime()
        {
            string m = DateTime.Now.Minute.ToString("x2");
            string h = DateTime.Now.Hour.ToString("x2");
            //string longtime = DateTime.Now.ToLongTimeString();
            //string shortime = DateTime.Now.ToShortTimeString();

            return $"{h} {m}";
        }
        private String GetTime { get { return GetSystemTime(); } }
        private string GetSystemDate()
        {

            string d = DateTime.Now.Day.ToString("x2");
            string m = DateTime.Now.Month.ToString("x2");
            //string y = DateTime.Now.Year.ToString("x4");

            //string longDate = DateTime.Now.ToLongDateString();
            //string shortDate = DateTime.Now.ToShortDateString();

            return $"{m} {d}";
        }
        private String GetDate { get { return GetSystemDate(); } }

        private string GetSystemDateYear()
        {

            string y = DateTime.Now.Year.ToString("x4");
            //string y = DateTime.Now.Year.ToString("x4");

            //string longDate = DateTime.Now.ToLongDateString();
            //string shortDate = DateTime.Now.ToShortDateString();

            return $"{y}";
        }

        public void UpdateColor(Brush b)
        {
            Foreground = b;
        }

        private String GetDateYear { get { return GetSystemDateYear(); } }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //HexClock.Properties.Settings.Default.Save();
            timeUpdater.Stop();
            tray.Exit(null, null);
        }

        private void HexShape_Click(object sender, RoutedEventArgs e)
        {
            new Settings().Show();
            e.Handled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            setParent(new WindowInteropHelper(this).Handle);
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);

            int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void HexShape_MouseUp(object sender, MouseButtonEventArgs e)
        {
            HexShape_Click(sender,e);
        }
        private void HexShape_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            HexShape_Click(sender, e);
        }

        private void SetColorClick(object sender, RoutedEventArgs e)
        {

        }

        void NotifyIcon_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                var menu = this.FindResource("NotifierContextMenu") as ContextMenu;
                menu.IsOpen = true;
            }
        }

        public void Kill(object sender, EventArgs e)
        {
            Close();
        }
    }

    public class TimeModel : INotifyPropertyChanged
    {
        public TimeModel()
        {
            //if(Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Time = "FF FF";
                Date = "FF FF";
                Date_y = "FFFF";
            }
        }
   
        string m_Time = default(string);
        public string Time { get { return m_Time; } set { SetProperty(ref m_Time, value); } }
        string m_Date = default(string);
        public string Date { get { return m_Date; } set { SetProperty(ref m_Date, value); } }
        string m_Date_y = default(string);
        public string Date_y { get { return m_Date_y; } set { SetProperty(ref m_Date_y, value); } }
   
        public event PropertyChangedEventHandler PropertyChanged;
   
        protected void SetProperty<T> (ref T storage, T value, [System.Runtime.CompilerServices.CallerMemberName] String propertyName = null)
        {
            if(!object.Equals(storage,value))
            {
                storage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
