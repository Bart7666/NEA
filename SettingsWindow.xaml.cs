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

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)LightModeRButton.IsChecked)
            {
                Application.Current.Resources["Button"] = Application.Current.Resources["BaseButton"];
                Application.Current.Resources["Menu"] = Application.Current.Resources["BaseMenu"];
                Application.Current.Resources["HMenu"] = Application.Current.Resources["BaseHMenu"];
                Application.Current.Resources["MenuItem"] = Application.Current.Resources["BaseMenuItem"];
                Application.Current.Resources["Border"] = Application.Current.Resources["BaseBorder"];
                Application.Current.Resources["GridBorders"] = Application.Current.Resources["BaseGridBorders"];
                Application.Current.Resources["Window"] = Application.Current.Resources["BaseWindow"];
                Application.Current.Resources["TextBlock"] = Application.Current.Resources["BaseTextBlock"];
                Application.Current.Resources["Label"] = Application.Current.Resources["BaseLabel"];
                Application.Current.Resources["TextBox"] = Application.Current.Resources["BaseTextBox"];
                Application.Current.Resources["RadioButton"] = Application.Current.Resources["BaseRadioButton"];
            }
            else if ((bool)DarkModeRButton.IsChecked)
            {
                Application.Current.Resources["Button"] = Application.Current.Resources["DarkButton"];
                Application.Current.Resources["Menu"] = Application.Current.Resources["DarkMenu"];
                Application.Current.Resources["HMenu"] = Application.Current.Resources["DarkHMenu"];
                Application.Current.Resources["MenuItem"] = Application.Current.Resources["DarkMenuItem"];
                Application.Current.Resources["Border"] = Application.Current.Resources["DarkBorder"];
                Application.Current.Resources["GridBorders"] = Application.Current.Resources["DarkGridBorders"];
                Application.Current.Resources["Window"] = Application.Current.Resources["DarkWindow"];
                Application.Current.Resources["TextBlock"] = Application.Current.Resources["DarkTextBlock"];
                Application.Current.Resources["Label"] = Application.Current.Resources["DarkLabel"];
                Application.Current.Resources["TextBox"] = Application.Current.Resources["DarkTextBox"];
                Application.Current.Resources["RadioButton"] = Application.Current.Resources["DarkRadioButton"];
            }
                this.Owner.Show();
            this.Close();
        }
    }
}
