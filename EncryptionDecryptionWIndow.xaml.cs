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
    /// Interaction logic for EncryptionDecryptionWIndow.xaml
    /// </summary>
    public partial class EncryptionDecryptionWIndow : Window
    {
        public AlgorithmSelected CurrentAlgorithm = AlgorithmSelected.None;

        public EncryptionDecryptionWIndow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Minimises the current window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimiseButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// Maximises the current window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaximiseButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized) { this.WindowState = WindowState.Normal; }
            else { this.WindowState = WindowState.Maximized; }
        }
        /// <summary>
        /// Closes the parent window closing the entire application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Close();
        }
        /// <summary>
        /// Opens the settings window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.Show();
            this.Hide();
        }
        /// <summary>
        /// Closes this window and shows home window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }


        // The following code (untill comment says otherwise) fixes the border issue for the application as discovered in testing for protype 1, it is copied as referenced in the NEA documentation.
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
        //End of copied code
        /// <summary>
        /// Encrypts or Decrypts inputdata using the selected algorithm, else alerts user of incorrect algorithm configuration or input data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EncryptDecryptBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentAlgorithm != AlgorithmSelected.None) //An algorithm is selected
            {
                if(CurrentAlgorithm == AlgorithmSelected.CaesarCipher) //Caesar Cipher is selected to use
                {
                    CaesarCipher Algorithm = new CaesarCipher(); //Instance of Caesar Cipher created
                    ValidationResult InputValidity = Algorithm.SetAndValidateData(InputFieldTBox.Text, KeyFieldTBox.Text); //Attempts to set and so validate input data
                    if (InputValidity == ValidationResult.Valid) //If all input data is correct
                    {
                        Algorithm.CleanData(DataInputType.Text); //Cleans input data
                        if((string)EncryptDecryptBtn.Content == "Encrypt") //Depending on state of EncryptDecrypt Button it either encrypts or decrypts the data then composes
                        {
                            Algorithm.EncryptData();
                            Algorithm.ComposeData(DataInputType.Text);
                        }
                        else if ((string)EncryptDecryptBtn.Content == "Decrypt")
                        {
                            Algorithm.DecryptData();
                            Algorithm.ComposeData(DataInputType.Text);
                        }
                        OutputFieldTBlock.Text = Algorithm.OutputData; //Sets value of outputfield to be the human readable composed plaintext / ciphertext.
                    }
                    else if (InputValidity == ValidationResult.DataInvalid) //Alerts user input plaintext / ciphertext data is invalid for this algorithm
                    {
                        throw new NotImplementedException(); //Create pop up window alerting user of incorrect input plaintext / ciphertext data
                    }
                    else if (InputValidity == ValidationResult.KeyInvalid) //Alerts user key is invalid for this algorithm
                    {
                        throw new NotImplementedException(); //Create pop up window alerting user of incorrect key
                    }
                    else if (InputValidity == ValidationResult.KeyAndDataInvalid) //Alerts user key and input plaintext / ciphertext data is invalid for this algorithm
                    {
                        throw new NotImplementedException(); //Create pop up window alerting user of incorrect key and input plaintext / ciphertext data
                    }
                }
            }
            else // No algorithm selected
            {
                throw new NotImplementedException(); //Create pop up window alerting user of them not having selected a cipher to use
            }
        }
        /// <summary>
        /// User selected to encrypt data and so changes content and logic of EncryptDecryptBtn to match.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectEncrypt_Selected(object sender, RoutedEventArgs e)
        {
            EncryptDecryptBtn.Content = "Encrypt";
        }
        /// <summary>
        /// User selected to decrypt data and so changes content and logic of EncryptDecryptBtn to match.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectDecrypt_Selected(object sender, RoutedEventArgs e)
        {
            EncryptDecryptBtn.Content = "Decrypt";
        }
        /// <summary>
        /// Sets algorithm to be used in encryption and decryption to be Caesar Cipher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectCaesarCipher_Selected(object sender, RoutedEventArgs e)
        {
            CurrentAlgorithm = AlgorithmSelected.CaesarCipher;
        }
    }
    
}

