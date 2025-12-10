using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
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
    /// Interaction logic for EncryptionDecryptionWindow.xaml
    /// </summary>
    public partial class EncryptionDecryptionWindow : Window
    {
        public AlgorithmSelected CurrentAlgorithm = AlgorithmSelected.None;

        public EncryptionDecryptionWindow()
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
                RunAlgorithm(SelectAlgorithm()!,ComposeConfig()!); //Runs the selected algorithm
            }
            else // No algorithm selected
            {
                MessageBox.Show("No algorithm selected, please select one from the drop down menu","Algorithm Selection Error"); //Creates a pop up window alerting user of them not having selected a cipher to use
            }
        }
        /// <summary>
        /// Creates an instance of the selected algorithm to use in encryption / decryption
        /// </summary>
        /// <returns></returns>
        private EncryptionAlgorithm? SelectAlgorithm()
        {
            EncryptionAlgorithm Algorithm;
            if (CurrentAlgorithm == AlgorithmSelected.CaesarCipher)
            {
                Algorithm = new CaesarCipher(); //Instance of Caesar Cipher created to use
                return Algorithm;
            }
            else if (CurrentAlgorithm == AlgorithmSelected.VigenèreCipher)
            {
                Algorithm = new VigenèreCipher(); //Instance of Vigenère Cipher created to use
                return Algorithm;
            }
            else if (CurrentAlgorithm == AlgorithmSelected.Enigma)
            {
                Algorithm = new Enigma(); //Instance of Enigma created to use
                return Algorithm;
            }
            else if (CurrentAlgorithm == AlgorithmSelected.Scytale)
            {
                Algorithm = new Scytale(); //Instance of Scytale created to use
                return Algorithm;
            }
            else if (CurrentAlgorithm == AlgorithmSelected.OneTimePad)
            {
                Algorithm = new OneTimePad(); //Instance of OneTimePad is created to use
                return Algorithm;
            }
            else
            {
                return null;
            }
            
        }
        /// <summary>
        /// Creates Config list to use in encryption / decryption if used
        /// </summary>
        /// <returns></returns>
        private List<string> ComposeConfig()
        {
            List<string> ComposedConfigSettings = new List<string> {string.Empty};
            if (CurrentAlgorithm == AlgorithmSelected.CaesarCipher)
            {
                ComposedConfigSettings.Clear();
                ComposedConfigSettings.Add("N/A"); //No config for this algorithm
                return ComposedConfigSettings;
            }
            else if (CurrentAlgorithm == AlgorithmSelected.VigenèreCipher)
            {
                ComposedConfigSettings.Clear();
                ComposedConfigSettings.Add("N/A"); //No config for this algorithm
                return ComposedConfigSettings;
            }
            else if (CurrentAlgorithm == AlgorithmSelected.Enigma)
            {
                ComposedConfigSettings.Clear();
                ComposedConfigSettings.Add(((EnigmaConfig)(AlgorithmConfigFrame.Content)).Rotor1Selection.Text); //This and two next lines add selected Rotors
                ComposedConfigSettings.Add(((EnigmaConfig)(AlgorithmConfigFrame.Content)).Rotor2Selection.Text);
                ComposedConfigSettings.Add(((EnigmaConfig)(AlgorithmConfigFrame.Content)).Rotor3Selection.Text);
                ComposedConfigSettings.Add(((EnigmaConfig)(AlgorithmConfigFrame.Content)).Rotor1Offset.UpDownCounter.Text); //This and the two next lines add the offset for the selected rotors
                ComposedConfigSettings.Add(((EnigmaConfig)(AlgorithmConfigFrame.Content)).Rotor2Offset.UpDownCounter.Text);
                ComposedConfigSettings.Add(((EnigmaConfig)(AlgorithmConfigFrame.Content)).Rotor3Offset.UpDownCounter.Text);
                ComposedConfigSettings.Add(((EnigmaConfig)(AlgorithmConfigFrame.Content)).ReflectorSelection.Text); //This adds the selected Reflector
                return ComposedConfigSettings;
            }
            else if (CurrentAlgorithm == AlgorithmSelected.Scytale)
            {
                ComposedConfigSettings.Clear();
                ComposedConfigSettings.Add("N/A"); //No config for this algorithm
                return ComposedConfigSettings;
            }
            else if (CurrentAlgorithm == AlgorithmSelected.OneTimePad)
            {
                ComposedConfigSettings.Clear();
                ComposedConfigSettings.Add(Convert.ToString(((OneTimePadConfig)(AlgorithmConfigFrame.Content)).RandomNumCheckB.IsChecked)!); //If random key is selected
                return ComposedConfigSettings;
            }
            else
            {
                return ComposedConfigSettings;
            }
            
        }
        /// <summary>
        /// Runs the selected cipher using the selected settings and the given inputs
        /// </summary>
        /// <param name="Algorithm"></param>
        private void RunAlgorithm(EncryptionAlgorithm Algorithm, List<string> ComposedConfigSettings)
        {
            
            ValidationResult InputValidity = Algorithm.SetAndValidateData(InputFieldTBox.Text, KeyFieldTBox.Text,ComposedConfigSettings); //Attempts to set and so validate input data
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
                OutpotFieldTBox.Text = Algorithm.OutputData; //Sets value of outputfield to be the human readable composed plaintext / ciphertext.
            }
            else if (InputValidity == ValidationResult.DataInvalid) //Alerts user input plaintext / ciphertext data is invalid for this algorithm
            {
                MessageBox.Show("Incorrect input data, please check raw data input requirements for this algorithm", "Incorrect Data Input");//Creates a pop up window alerting user of incorrect input plaintext / ciphertext data
            }
            else if (InputValidity == ValidationResult.KeyInvalid) //Alerts user key is invalid for this algorithm
            {
                MessageBox.Show("Incorrect key input, please check requirements for key for this algorithm", "Incorrect Key Input"); //Creates a pop up window alerting user of incorrect key
            }
            else if (InputValidity == ValidationResult.ConfigInvalid) //Alerts user config settings are invalid for this algorithm
            {
                MessageBox.Show("Incorrect config settings, please check requirements for algorithm configuration for this algorithm", "Incorrect Key Input"); //Creates a pop up window alerting user of incorrect config settings
            }
            else if (InputValidity == ValidationResult.KeyAndDataInvalid) //Alerts user key and input plaintext / ciphertext data is invalid for this algorithm
            {
                MessageBox.Show("Incorrect key and data input, please check requirements for this algorithm", "Incorrect Key and Data Input"); //Create pop up window alerting user of incorrect key and input plaintext / ciphertext data
            }
            else if (InputValidity == ValidationResult.KeyAndConfigInvalid) //Alerts user key and selected config settings are invalid for this algorithm
            {
                MessageBox.Show("Incorrect key and config settings input, please check requirements for this algorithm", "Incorrect Key and Config Input");//Creates a pop up window alerting user of incorrect key and config settings
            }
            else if (InputValidity == ValidationResult.DataAndConfigInvalid) //Alerts user input data and selected config settings are invalid for this algorithm
            {
                MessageBox.Show("Incorrect data and config settings input, please check requirements for this algorithm", "Incorrect Data and Config Input");//Creates a pop up window alerting user of incorrect key and config settings
            }
            else if (InputValidity == ValidationResult.KeyAndDataAndConfigInvalid) //Alerts user key and selected config settings are invalid for this algorithm
            {
                MessageBox.Show("Incorrect key, data, and config settings input, please check requirements for this algorithm", "Incorrect Key, Data, and Config Input");//Creates a pop up window alerting user of incorrect key and config settings
            }
            if (CurrentAlgorithm == AlgorithmSelected.OneTimePad)
            {
                KeyFieldTBox.Text = Algorithm.Key;
            }
        }
        /// <summary>
        /// Sets the config settings and the key requiremnts to be displayed
        /// </summary>
        private void CaesarCipherConfig()
        {
            InputFieldTBox.Text = "Max input character length = 536870912\nUsing extended ASCII (ISO Latin-1)";
            KeyFieldTBox.Text = "Any integer";
            AlgorithmConfigFrame.Content = null; //Clear Algorithm Config
        }
        /// <summary>
        /// Sets the config settings and the key requiremnts to be displayed
        /// </summary>
        private void VigenèreCipherConfig()
        {
            InputFieldTBox.Text = "Max input character length = 536870912\nUsing extended ASCII (ISO Latin-1)";
            KeyFieldTBox.Text = "Any English letters";
            AlgorithmConfigFrame.Content = null; //Clear Algorithm Config
        }
        /// <summary>
        /// Sets the config settings and the key requiremnts to be displayed
        /// </summary>
        private void EnigmaConfig()
        {
            InputFieldTBox.Text = "Max input character length = 536870912\nUsing regular ASCII letters (regular english letters)";
            KeyFieldTBox.Text = "Three English letters to show start positions of each rotor, left to right - first to third";
            AlgorithmConfigFrame.Content = new EnigmaConfig(); //Set Algorithm Config to EnigmaConfig settings
        }
        /// <summary>
        /// Sets the config settings and the key requiremnts to be displayed
        /// </summary>
        private void OneTimePadConfig()
        {
            InputFieldTBox.Text = "Max input character length = 536870912\nUsing only standard ASCII (english) letters";
            KeyFieldTBox.Text = "Any English letters, must be as long as or greater than data";
            AlgorithmConfigFrame.Content = new OneTimePadConfig(); //Set Algorithm to OneTimePadConfig settings
        }
        /// <summary>
        /// Sets the config settings and the key requiremnts to be displayed
        /// </summary>
        private void ScytaleConfig()
        {
            InputFieldTBox.Text = "Max input character length = 536870912, Must fit in Scytale (rectangle) of are equal to the product of the two components of the key number\nUsing extended ASCII (ISO Latin-1)";
            KeyFieldTBox.Text = "Scytale: (RowNum,ColumnNum)";
            AlgorithmConfigFrame.Content = null; //Clear Algorithm Config
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
        /// Checks if the currently selected algorithm is one which uses a numeric key
        /// </summary>
        /// <returns></returns>
        private bool IsNumerickey()
        {
            if (CurrentAlgorithm == AlgorithmSelected.CaesarCipher || CurrentAlgorithm == AlgorithmSelected.Scytale) //Contains list of algorithms which use numeric keys
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Depending on the required key, only allows the user to enter numbers or letters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyFieldTBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsNumerickey() & KeyFieldTBox.IsFocused & KeyFieldTBox.Text.Length > 0 && KeyFieldTBox.CaretIndex > 0 & CurrentAlgorithm != AlgorithmSelected.Scytale) //Key validation for numeric keys
            { 
                int ChangedCharacter = (int)Convert.ToChar(KeyFieldTBox.Text[KeyFieldTBox.CaretIndex - 1]); //Most recently added character in ASCII form
                if (ChangedCharacter < 48 | ChangedCharacter > 57) //If input at caret location is not a number
                {
                    KeyFieldTBox.Text = KeyFieldTBox.Text.ToString().Remove(KeyFieldTBox.CaretIndex-1, 1); //Clears character input
                    KeyFieldTBox.CaretIndex = KeyFieldTBox.Text.Length; //Sets caret postion to end of key
                }
            }
            else if (IsNumerickey() & KeyFieldTBox.IsFocused & KeyFieldTBox.Text.Length > 0 && KeyFieldTBox.CaretIndex > 0 & CurrentAlgorithm == AlgorithmSelected.Scytale) //Key validation for Scytale
            {
                int ChangedCharacter = (int)Convert.ToChar(KeyFieldTBox.Text[KeyFieldTBox.CaretIndex - 1]); //Most recently added character in ASCII form
                int CommaCount = KeyFieldTBox.Text.Split(",").Length - 1;
                if (ChangedCharacter == 44 & CommaCount == 1) {} //If there is only one comma input
                else if (ChangedCharacter < 48 | ChangedCharacter > 57) //If input at caret location is not a number
                {
                    KeyFieldTBox.Text = KeyFieldTBox.Text.ToString().Remove(KeyFieldTBox.CaretIndex - 1, 1); //Clears character input
                    KeyFieldTBox.CaretIndex = KeyFieldTBox.Text.Length; //Sets caret postion to end of key
                }
            }
            else if (!IsNumerickey() & KeyFieldTBox.IsFocused & KeyFieldTBox.Text.Length > 0 && KeyFieldTBox.CaretIndex > 0) //Key validation for letter keys
            {
                
                int ChangedCharacter = (int)Char.ToLower(Convert.ToChar(KeyFieldTBox.Text[KeyFieldTBox.CaretIndex - 1])); //Most recently added character in ASCII form
                if (CurrentAlgorithm == AlgorithmSelected.Enigma && KeyFieldTBox.Text.Length>3)
                {
                    KeyFieldTBox.Text = KeyFieldTBox.Text.ToString().Remove(KeyFieldTBox.CaretIndex - 1, 1); //Clears character input
                    KeyFieldTBox.CaretIndex = KeyFieldTBox.Text.Length; //Sets caret postion to end of key
                }
                else if (!(ChangedCharacter >= 97 & ChangedCharacter <= 122)) //If input at caret location is not an english letter
                {
                    KeyFieldTBox.Text = KeyFieldTBox.Text.ToString().Remove(KeyFieldTBox.CaretIndex - 1, 1); //Clears character input
                    KeyFieldTBox.CaretIndex = KeyFieldTBox.Text.Length; //Sets caret postion to end of key
                }
            }
        }
        /// <summary>
        /// Clears instruction text for user when selecting the keyfield box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void KeyFieldTBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (IsNumerickey() && KeyFieldTBox.Text == "Any integer") //If instruction text is still present for numeric keys
            {
                KeyFieldTBox.Text = "";
            }
            else if (!IsNumerickey() & (KeyFieldTBox.Text == "Any English letters" | KeyFieldTBox.Text == "Three English letters to show start positions of each rotor, left to right - first to third"))
            {
                KeyFieldTBox.Text = "";
            }
        }
        /// <summary>
        /// Swaps content of input and output fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwapFieldsBtn_Click(object sender, RoutedEventArgs e)
        {
            string TempVar = InputFieldTBox.Text;
            InputFieldTBox.Text = OutpotFieldTBox.Text;
            OutpotFieldTBox.Text = TempVar;
        }
        /// <summary>
        /// Sets algorithm to be used in encryption and shows appropiate config settings and key requirements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectVigenèreCipher_Selected(object sender, RoutedEventArgs e)
        {
            CurrentAlgorithm = AlgorithmSelected.VigenèreCipher;
            VigenèreCipherConfig(); //Labels the Keyfield to require string input
        }
        /// <summary>
        /// Sets algorithm to be used in encryption and shows appropiate config settings and key requirements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectCaesarCipher_Selected(object sender, RoutedEventArgs e)
        {
            CurrentAlgorithm = AlgorithmSelected.CaesarCipher;
            CaesarCipherConfig(); //Labels the Keyfield to require integer input
        }
        /// <summary>
        /// Sets algorithm to be used in encryption and shows appropiate config settings and key requirements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectEnigma_Selected(object sender, RoutedEventArgs e)
        {
            CurrentAlgorithm = AlgorithmSelected.Enigma;
            EnigmaConfig(); //Labels Keyfield to require specific string input and labels input field for only english letters
        }
        /// <summary>
        /// Sets algorithm to be used in encryption and shows appropiate config settings and key requirements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectScytale_Selected(object sender, RoutedEventArgs e)
        {
            CurrentAlgorithm = AlgorithmSelected.Scytale;
            ScytaleConfig(); //Labels Keyfield to require specific string input and raw data input field for extended ASCII
        }
        /// <summary>
        /// Sets algorithm to be used in encryption and shows appropiate config settings and key requirements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectOneTimePad_Selected(object sender, RoutedEventArgs e)
        {
            CurrentAlgorithm = AlgorithmSelected.OneTimePad;
            OneTimePadConfig(); // Labels Key and input field to use only regular ascii letters and tells user of min length for key, and opens config to generate random key
        }
    }
    
}

