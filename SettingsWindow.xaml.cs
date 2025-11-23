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
        /// Does nothing as user already in settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {

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

    }
}
