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
            DateTime date = DateTime.Now;

            TimeModel.DateDay = date.Day.ToString("x2");
            TimeModel.DateMonth = date.Month.ToString("x2");
            TimeModel.DateYear = date.Year.ToString("x4");

            TimeModel.TimeHours = date.Hour.ToString("x2");
            TimeModel.TimeMinutes = date.Minute.ToString("x2");

        }

        public void UpdateColor(Brush b)
        {
            Foreground = b;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
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
            m_DateYear = "FFFF";
            m_TimeHours = m_TimeMinutes = "FF";
            m_DateMonth = m_DateDay = "FF";

        }


        string m_TimeMinutes = String.Empty;
        string m_TimeHours = String.Empty;

        string m_DateYear = String.Empty;
        string m_DateMonth = String.Empty;
        string m_DateDay = String.Empty;


        public string TimeMinutes { get { return m_TimeMinutes; } set { SetProperty(ref m_TimeMinutes, value); } }
        public string TimeHours { get { return m_TimeHours; } set { SetProperty(ref m_TimeHours, value); } }
        public string DateYear { get { return m_DateYear; } set { SetProperty(ref m_DateYear, value); } }
        public string DateMonth { get { return m_DateMonth; } set { SetProperty(ref m_DateMonth, value); } }
        public string DateDay { get { return m_DateDay; } set { SetProperty(ref m_DateDay, value); } }

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
