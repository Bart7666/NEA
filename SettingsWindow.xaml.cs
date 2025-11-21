using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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
            this.WindowState = WindowState.Maximized; //Window will be fullscreen upon opening
        }
        


        /// <summary>
        /// Sets the user theme depending on the currently selected radiobutton
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
    }
}
