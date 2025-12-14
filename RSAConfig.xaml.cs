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
    /// Interaction logic for RSAConfig.xaml
    /// </summary>
    public partial class RSAConfig : Page
    {
        public RSAConfig()
        {
            InitializeComponent();
        }

        private void CopyPubliKeyBtn_Click(object sender, RoutedEventArgs e)
        {
            EncryptionDecryptionWindow OwnerWindow = (EncryptionDecryptionWindow)Window.GetWindow(this);
            if (!(OwnerWindow.RSAKeyList.Count < 3))
            {
                Clipboard.SetText(OwnerWindow.RSAKeyList[0] + "," + OwnerWindow.RSAKeyList[1]);
            }
        }

        private void CopyPrivateKeyBtn_Click(object sender, RoutedEventArgs e)
        {
            EncryptionDecryptionWindow OwnerWindow = (EncryptionDecryptionWindow)Window.GetWindow(this);
            if (!(OwnerWindow.RSAKeyList.Count < 3))
            {
                Clipboard.SetText(OwnerWindow.RSAKeyList[0] + "," + OwnerWindow.RSAKeyList[2]);
            }
        }
    }
}
