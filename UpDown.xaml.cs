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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NEA
{
    /// <summary>
    /// Interaction logic for UpDown.xaml
    /// </summary>
    public partial class UpDown : UserControl
    {  
        /// <summary>
        /// UpdownControl
        /// </summary>
        public UpDown()
        {
            InitializeComponent();
        }
        

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            if(Int32.Parse(UpDownCounter.Text) ==25)
            {
                UpDownCounter.Text = "0";
            }
            else
            {
                UpDownCounter.Text = Convert.ToString((Int32.Parse(UpDownCounter.Text) + 1));
            }
        }
        private void Down_Click(object sender, RoutedEventArgs e)
        {
            if (Int32.Parse(UpDownCounter.Text) == 0)
            {
                UpDownCounter.Text = "25";
            }
            else
            {
                UpDownCounter.Text = Convert.ToString((Int32.Parse(UpDownCounter.Text) - 1));
            }
        }
        public string CounterVal => (UpDownCounter.Text);
    }
}
