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
    /// Interaction logic for NetworkingWindow.xaml
    /// </summary>
    public partial class NetworkingWindow : Window
    {
        /// <summary>
        ///Currently selected algorithm
        /// </summary>
        public AlgorithmSelected CurrentAlgorithm = AlgorithmSelected.None;
        /// <summary>
        ///List of keys for RSA, in the order Common,Public,Private
        /// </summary>
        public List<string> RSAKeyList = new List<string>(3);

        public NetworkingWindow()
        {
            InitializeComponent();
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
                    mmi.ptMaxPosition.X = System.Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                    mmi.ptMaxPosition.Y = System.Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                    mmi.ptMaxSize.X = System.Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                    mmi.ptMaxSize.Y = System.Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
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
        //End of copied code.

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
        /// <summary>
        /// Encrypts or Decrypts inputdata using the selected algorithm, else alerts user of incorrect algorithm configuration or input data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EncryptDecryptBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentAlgorithm != AlgorithmSelected.None) //An algorithm is selected
            {
                RunAlgorithm(SelectAlgorithm()!, ComposeConfig()!); //Runs the selected algorithm
            }
            else // No algorithm selected
            {
                MessageBox.Show("No algorithm selected, please select one from the drop down menu", "Algorithm Selection Error"); //Creates a pop up window alerting user of them not having selected a cipher to use
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
            else if (CurrentAlgorithm == AlgorithmSelected.RSA)
            {
                Algorithm = new RSA(); //Instance of RSA is created to use
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
            RSAKeyList.Clear();
            List<string> ComposedConfigSettings = new List<string> { string.Empty };
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
                if (ComposedConfigSettings.Count != 7 || ComposedConfigSettings[0].Length > 3 || ComposedConfigSettings[1].Length > 3 || ComposedConfigSettings[2].Length > 3 || ComposedConfigSettings[6].Length > 1)
                {
                    ComposedConfigSettings.Clear();
                }
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
            else if (CurrentAlgorithm == AlgorithmSelected.RSA)
            {
                ComposedConfigSettings.Clear();
                ComposedConfigSettings.Add(Convert.ToString(((RSAConfig)AlgorithmConfigFrame.Content).HexNumberCheckB.IsChecked)!); //If Hex input is selected
                ComposedConfigSettings.Add(Convert.ToString(((RSAConfig)AlgorithmConfigFrame.Content).GenerateKeyCheckB.IsChecked)!); //If random key is selected
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
            if ((CurrentAlgorithm == AlgorithmSelected.RSA | KeyFieldTBox.Text.Length < 10) | !IsNumerickey())
            {
                ValidationResult InputValidity = Algorithm.SetAndValidateData(InputFieldTBox.Text, KeyFieldTBox.Text, ComposedConfigSettings); //Attempts to set and so validate input data
                if (InputValidity == ValidationResult.Valid) //If all input data is correct
                {
                    Algorithm.CleanData(DataInputType.String); //Cleans input data
                    if ((string)EncryptDecryptBtn.Content == "Encrypt") //Depending on state of EncryptDecrypt Button it either encrypts or decrypts the data then composes
                    {
                        Algorithm.EncryptData();
                        Algorithm.ComposeData(DataInputType.String);
                    }
                    else if ((string)EncryptDecryptBtn.Content == "Decrypt")
                    {
                        Algorithm.DecryptData();
                        Algorithm.ComposeData(DataInputType.String);
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
                    MessageBox.Show("Incorrect config settings, please check requirements for algorithm configuration for this algorithm", "Incorrect Config Input"); //Creates a pop up window alerting user of incorrect config settings
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
                if ((CurrentAlgorithm == AlgorithmSelected.OneTimePad & Algorithm.AlgorithmConfig.Count > 0) && (InputValidity == ValidationResult.Valid & Algorithm.AlgorithmConfig[0] == "True")) //Sets Key after using random key
                {
                    KeyFieldTBox.Text = Algorithm.Key;
                }
                else if ((CurrentAlgorithm == AlgorithmSelected.RSA & Algorithm.AlgorithmConfig.Count > 0) && (InputValidity == ValidationResult.Valid & Algorithm.AlgorithmConfig[1] == "True")) //Sets key after using random key generation, and makes each component accessible for user
                {
                    KeyFieldTBox.Text = Algorithm.Key;
                    RSAKeyList.Add(Algorithm.AlgorithmConfig[2]);
                    RSAKeyList.Add(Algorithm.AlgorithmConfig[3]);
                    RSAKeyList.Add(Algorithm.AlgorithmConfig[4]);
                }
            }
            else //Key length is too long to hold in an itneger
            {
                MessageBox.Show("Incorrect key input, please check requirements for key for this algorithm", "Incorrect Key Input"); //Creates a pop up window alerting user of incorrect key
            }


        }
        /// <summary>
        /// Sets the config settings and the key requiremnts to be displayed
        /// </summary>
        private void CaesarCipherConfig()
        {
            OutputFieldLabel.Content = "Output Field";
            InputFieldTBox.Text = "Max input character length = 536870912\nUsing extended ASCII (ISO Latin-1)";
            KeyFieldTBox.Text = "Any integer";
            AlgorithmConfigFrame.Content = null; //Clear Algorithm Config
        }
        /// <summary>
        /// Sets the config settings and the key requiremnts to be displayed
        /// </summary>
        private void VigenèreCipherConfig()
        {
            OutputFieldLabel.Content = "Output Field";
            InputFieldTBox.Text = "Max input character length = 536870912\nUsing extended ASCII (ISO Latin-1)";
            KeyFieldTBox.Text = "Any English letters";
            AlgorithmConfigFrame.Content = null; //Clear Algorithm Config
        }
        /// <summary>
        /// Sets the config settings and the key requiremnts to be displayed
        /// </summary>
        private void EnigmaConfig()
        {
            OutputFieldLabel.Content = "Output Field";
            InputFieldTBox.Text = "Max input character length = 536870912\nUsing regular ASCII letters (regular english letters)";
            KeyFieldTBox.Text = "Three English letters to show start positions of each rotor, left to right - first to third";
            AlgorithmConfigFrame.Content = new EnigmaConfig(); //Set Algorithm Config to EnigmaConfig settings
        }
        /// <summary>
        /// Sets the config settings and the key requiremnts to be displayed
        /// </summary>
        private void OneTimePadConfig()
        {
            OutputFieldLabel.Content = "Output Field";
            InputFieldTBox.Text = "Max input character length = 536870912\nUsing only standard ASCII (english) letters";
            KeyFieldTBox.Text = "Any English letters, must be as long as or greater than data";
            AlgorithmConfigFrame.Content = new OneTimePadConfig(); //Set Algorithm Config to OneTimePadConfig settings
        }
        /// <summary>
        /// Sets the config settings and the key requiremnts to be displayed
        /// </summary>
        private void ScytaleConfig()
        {
            InputFieldTBox.Text = "Max input character length = 536870912, Must fit in Scytale (rectangle) of are equal to the product of the two components of the key number\nUsing extended ASCII (ISO Latin-1)";
            OutputFieldLabel.Content = "Output Field";
            KeyFieldTBox.Text = "Scytale: (RowNum,ColumnNum)";
            AlgorithmConfigFrame.Content = null; //Clear Algorithm Config
        }
        /// <summary>
        /// Sets the config settings and the key requiremnts to be displayed
        /// </summary>
        private void RSAConfig()
        {
            InputFieldTBox.Text = "Reccomended maximum character input length is 3 characters, any more can substantially impact performance\nUsing extended ASCII (ISO Latin-1)";
            OutputFieldLabel.Content = "Output Field (Hexadecimal)";
            if ((string)EncryptDecryptBtn.Content == "Encrypt")
            {
                KeyFieldTBox.Text = "(Common Public Prime, PublicKey coprime)";
            }
            else
            {
                KeyFieldTBox.Text = "(Common Public Prime, Private key)";
            }
            AlgorithmConfigFrame.Content = new RSAConfig(); //Set Algorithm Config to RSA settings
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
            if (CurrentAlgorithm == AlgorithmSelected.CaesarCipher || CurrentAlgorithm == AlgorithmSelected.Scytale || CurrentAlgorithm == AlgorithmSelected.RSA) //Contains list of algorithms which use numeric keys
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
            if (IsNumerickey() & KeyFieldTBox.IsFocused & KeyFieldTBox.Text.Length > 0 && KeyFieldTBox.CaretIndex > 0 & CurrentAlgorithm != AlgorithmSelected.Scytale & CurrentAlgorithm != AlgorithmSelected.RSA) //Key validation for numeric keys
            {
                int ChangedCharacter = (int)Convert.ToChar(KeyFieldTBox.Text[KeyFieldTBox.CaretIndex - 1]); //Most recently added character in ASCII form
                if (ChangedCharacter < 48 | ChangedCharacter > 57) //If input at caret location is not a number
                {
                    KeyFieldTBox.Text = KeyFieldTBox.Text.ToString().Remove(KeyFieldTBox.CaretIndex - 1, 1); //Clears character input
                    KeyFieldTBox.CaretIndex = KeyFieldTBox.Text.Length; //Sets caret postion to end of key
                }
            }
            else if (IsNumerickey() & KeyFieldTBox.IsFocused & KeyFieldTBox.Text.Length > 0 && KeyFieldTBox.CaretIndex > 0 & (CurrentAlgorithm == AlgorithmSelected.Scytale | CurrentAlgorithm == AlgorithmSelected.RSA)) //Key validation for Scytale and RSA
            {
                int ChangedCharacter = (int)Convert.ToChar(KeyFieldTBox.Text[KeyFieldTBox.CaretIndex - 1]); //Most recently added character in ASCII form
                int CommaCount = KeyFieldTBox.Text.Split(",").Length - 1;
                if (ChangedCharacter == 44 & CommaCount == 1) { } //If there is only one comma input
                else if (ChangedCharacter < 48 | ChangedCharacter > 57) //If input at caret location is not a number
                {
                    KeyFieldTBox.Text = KeyFieldTBox.Text.ToString().Remove(KeyFieldTBox.CaretIndex - 1, 1); //Clears character input
                    KeyFieldTBox.CaretIndex = KeyFieldTBox.Text.Length; //Sets caret postion to end of key
                }
            }
            else if (!IsNumerickey() & KeyFieldTBox.IsFocused & KeyFieldTBox.Text.Length > 0 && KeyFieldTBox.CaretIndex > 0) //Key validation for letter keys
            {

                int ChangedCharacter = (int)Char.ToLower(Convert.ToChar(KeyFieldTBox.Text[KeyFieldTBox.CaretIndex - 1])); //Most recently added character in ASCII form
                if (CurrentAlgorithm == AlgorithmSelected.Enigma && KeyFieldTBox.Text.Length > 3)
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
            if (IsNumerickey() && (KeyFieldTBox.Text == "Any integer" | KeyFieldTBox.Text == "Scytale: (RowNum,ColumnNum)" | KeyFieldTBox.Text == "(Common Public Prime, PublicKey coprime)" | KeyFieldTBox.Text == "(Common Public Prime, Private key)")) //If instruction text is still present for numeric keys
            {
                KeyFieldTBox.Text = "";
            }
            else if (!IsNumerickey() && (KeyFieldTBox.Text == "Any English letters" | KeyFieldTBox.Text == "Three English letters to show start positions of each rotor, left to right - first to third"))
            {
                KeyFieldTBox.Text = "";
            }
        }
        /// <summary>
        /// If Encrypt/Decrypt choice changed do things
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EncryptDecryptCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentAlgorithm == AlgorithmSelected.RSA)
            {
                if ((string)EncryptDecryptBtn.Content == "Encrypt" & KeyFieldTBox.Text == "(Common Public Prime, Private key)")
                {
                    KeyFieldTBox.Text = "(Common Public Prime, PublicKey coprime)";
                }
                else if ((string)EncryptDecryptBtn.Content == "Decrypt" & KeyFieldTBox.Text == "(Common Public Prime, PublicKey coprime)")
                {
                    KeyFieldTBox.Text = "(Common Public Prime, Private key)";
                }
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
        /// <summary>
        /// Sets algorithm to be used in encryption and shows appropiate config settings and key requirements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectRSA_Selected(object sender, RoutedEventArgs e)
        {
            CurrentAlgorithm = AlgorithmSelected.RSA;
            RSAConfig(); // Labels input field to allow extended ascii and key input depending on whether encrypting or decrypting
        }
        /// <summary>
        /// Gives a description and basic history of Caesar Cipher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CaesarCipherBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Description and History\nThis cipher is one of the most famous encryption algorithms, due to its nature of being very simple, which was historically used by Julius Caesar to encrypt messages whilst on campaign.\n" + "Function\n" +
                " An integer key is used as the offset to apply for each character input from its current position in the alphabet, and it loops back to beginning if offset is longer than than 26, the length of the english alphabet."
                , "Caesar Cipher"); //Creates a pop up window giving a description and basic history of this algorithm
        }
        /// <summary>
        /// Gives a description and basic history of Vigenère Cipher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VigenèreCipherBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Description and History\nThis algorithm is a major advancement in cryptogrpahy and was used from its conception in the late 16th century to the 19th century during which it garned the name \"The indecipherable cipher\", it is an evolution of the Caesar Cipher.\n" + "Function\n" +
                " A key is used as the offset to apply for each character input from its current position in the alphabet, the key is composed of a string of letters, where the offset per letter to encrypt is the offset each letter in the key is from A," +
                "if the there are not enough letters in the key to match the number of input characters, then loop back around to the start of the key."
                , "Vigenère Cipher"); //Creates a pop up window giving a description and basic history of this algorithm
        }
        /// <summary>
        /// Gives a description and basic history of Enigma
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnigmaBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Description and History\nThis is perhaps the most famous encryption algorithm, especially in the UK, due to the notoriety it gained from its use in WW2 by the Nazis, and its importance has since been elevated by many pieces of media." +
                " It fundamentally works by linking encryption of one letter with the encryption of the next letter, and this was done in thw actual enigma machine by a series or rotors linked by wires which would rotate when a certain letter was encrypted.\n" + "Function\n" +
                "The key represents the starting position of each rotor, left to right, and the notch offsets change at what character the rotors cause rotation. Each input letter goes through the machine by following the connections of the rotors, then is reflected back by the reflector and then leaves through the rotors"
                , "Enigma"); //Creates a pop up window giving a description and basic history of this algorithm
        }
        /// <summary>
        /// Gives a description and basic history of Scytale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScytaleBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Description and History\nThis is one of the oldest theorised encryption methods, however its historical legitmacy is questioned by many scholars, it was purportedly used by Ancient greek soliders to encrypt messages however its very weak encryption meant it was more like a method of message authentication than encryption\n" + "Function\n" +
                "This algorithm works by winding a strip around a cylinder of a specific width, then writing the message lengthwise along the cylinder, on each of the \"sides\" then unwinding the strip leading to a seamingly scrambeled message, the key represents the number of columns (letters written along the cylinder) and the number of rows (\"faces\" used to write on)"
                , "Enigma"); //Creates a pop up window giving a description and basic history of this algorithm
        }
        /// <summary>
        /// Gives a description and basic history of One Time Pad Cipher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OneTimePadBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Description and History\nThis is unique algorithm which is quite special, fundamentally it is a special case of Vigenère Cipher, however under specific conditions it is mathemetically unbreakble. It is the only historical cipher in this solution which is still used for encryption as it can be used to send message sperfectly securely," +
                " however its use peaked during the cold war where spies had hundreds of these one time pads to securely transfer secret information\n" + "Function\n" +
                "This applies the Vigenère Cipher on the input data as normal, however the key must be as long or longer than the input data, so there is no two letters that could have used the same key and so caused a pattern. If this is met and the keys are truly random and the pad is only used once, it is a cryptographically secure method of encryption"
                , "One Time Pad"); //Creates a pop up window giving a description and basic history of this algorithm
        }
        /// <summary>
        /// Gives a description and basic history of the RSA algorithm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RSABtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Description and History\nThis is one of the cornerstones of modern cryptography and so cybersecurity, and it is still used widely to this day, it is a form of assymetric encryption which allows secure data transfer (at current technology) without transfering the secret key\n" + "Function\n" +
                " This algorithm uses the idea of large prime numbers being difficult to factorise to encrypt data securely, and it works by converting input data into a number which is then multiplied and exponentiated using the public key, and then that number can be decrypted using the private key. "
                , "RSA algorithm"); //Creates a pop up window giving a description and basic history of this algorithm
        }
    }

}
