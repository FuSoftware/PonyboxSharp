using PonyboxSharp.Ponybox;
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

namespace PonyboxSharp
{
    /// <summary>
    /// Logique d'interaction pour LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public Tuple<string, string> data;

        public LoginView()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            PBLogin();
        }

        private void PBLogin()
        {
            data = Tuple.Create<string, string>(Username.Text, Password.Text);
            //data = LoginClient.Login(Username.Text, Password.Text);

            if(data != null)
            {
                this.Hide();
            }
        }
    }
}
