using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NEA
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }
        


        /// <summary>
        /// Sets the user theme depending on the currently selected radiobutton.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (LightModeRButton.IsChecked != null && (bool)LightModeRButton.IsChecked)
            {
                this.Owner.Resources["Theme"] = "Light";//This changes the value of theme to light
                Application.Current.Resources["Button"] = Application.Current.Resources["BaseButton"];
                Application.Current.Resources["Menu"] = Application.Current.Resources["BaseMenu"];
                Application.Current.Resources["MenuItem"] = Application.Current.Resources["BaseMenuItem"];
                Application.Current.Resources["Border"] = Application.Current.Resources["BaseBorder"];
                Application.Current.Resources["Window"] = Application.Current.Resources["BaseWindow"];
                Application.Current.Resources["TextBlock"] = Application.Current.Resources["BaseTextBlock"];
                Application.Current.Resources["Label"] = Application.Current.Resources["BaseLabel"];
                Application.Current.Resources["TextBox"] = Application.Current.Resources["BaseTextBox"];
                Application.Current.Resources["RadioButton"] = Application.Current.Resources["BaseRadioButton"];
            }
            else if (DarkModeRButton.IsChecked != null && (bool)DarkModeRButton.IsChecked)
            {
                this.Owner.Resources["Theme"] = "Dark";//This changes the value of theme to dark
                Application.Current.Resources["Button"] = Application.Current.Resources["DarkButton"];
                Application.Current.Resources["Menu"] = Application.Current.Resources["DarkMenu"];
                Application.Current.Resources["MenuItem"] = Application.Current.Resources["DarkMenuItem"];
                Application.Current.Resources["Border"] = Application.Current.Resources["DarkBorder"];
                Application.Current.Resources["Window"] = Application.Current.Resources["DarkWindow"];
                Application.Current.Resources["TextBlock"] = Application.Current.Resources["DarkTextBlock"];
                Application.Current.Resources["Label"] = Application.Current.Resources["DarkLabel"];
                Application.Current.Resources["TextBox"] = Application.Current.Resources["DarkTextBox"];
                Application.Current.Resources["RadioButton"] = Application.Current.Resources["DarkRadioButton"];
            }
            this.Owner.Show();
            this.Close();
        }
        /// <summary>
        /// When the apply button is loaded, this sets the currently enabled themes and settings to be checked within the settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Owner is MainWindow) //If settings was opened from Home window.
            {
                if ((string)this.Owner.FindResource("Theme") == "Light")
                {
                    LightModeRButton.IsChecked = true;
                }
                else if ((string)this.Owner.FindResource("Theme") == "Dark")
                {
                    DarkModeRButton.IsChecked = true;
                }
                else //Base case for first loading
                {
                    LightModeRButton.IsChecked = true;
                }
            }
            else //If settings was opened from any other window (the solution will only have 2 layers of owned windows with settings).
            {
                if ((string)this.Owner.Owner.FindResource("Theme") == "Light")
                {
                    LightModeRButton.IsChecked = true;
                }
                else if ((string)this.Owner.Owner.FindResource("Theme") == "Dark")
                {
                    DarkModeRButton.IsChecked = true;
                }
                else //Base case for first loading
                {
                    LightModeRButton.IsChecked = true;
                }
            }
        }
        /// <summary>
        /// Minimises the current window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimiseButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// Maximises the current window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaximiseButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized) { this.WindowState = WindowState.Normal; }
            else { this.WindowState = WindowState.Maximized; }
        }
        /// <summary>
        /// Closes the main window closing the entire application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Owner is MainWindow) //If settings was opened from Home window.
            {
                this.Owner.Close();
            }
            else //If settings was opened from any other window (the solution will only have 2 layers of owned windows with settings).
            {
                this.Owner.Owner.Close();
            }
        }
        /// <summary>
        /// Does nothing as user already in settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// Closes this window and shows home window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }

        // The following code fixes the border issue for the application as discovered in testing for protype 1, it is copied as referenced in the NEA documentation.
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            ((HwndSource)PresentationSource.FromVisual(this)).AddHook(HookProc);
        }

        public static IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_GETMINMAXINFO)
            {
                // We need to tell the system what our size should be when maximized. Otherwise it will cover the whole screen,
                // including the task bar.
#pragma warning disable CS8605 // Unboxing a possibly null value.
                MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
#pragma warning restore CS8605 // Unboxing a possibly null value.

                // Adjust the maximized size and position to fit the work area of the correct monitor
                IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

                if (monitor != IntPtr.Zero)
                {
                    MONITORINFO monitorInfo = new MONITORINFO();
                    monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                    GetMonitorInfo(monitor, ref monitorInfo);
                    RECT rcWorkArea = monitorInfo.rcWork;
                    RECT rcMonitorArea = monitorInfo.rcMonitor;
                    mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                    mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                    mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                    mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
                }

                Marshal.StructureToPtr(mmi, lParam, true);
            }

            return IntPtr.Zero;
        }

        private const int WM_GETMINMAXINFO = 0x0024;

        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr handle, uint flags);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }
    }
}
