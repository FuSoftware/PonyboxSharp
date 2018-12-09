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
    /// Logique d'interaction pour PonyboxView.xaml
    /// </summary>
    public partial class PonyboxView : Window
    {
        PonyboxClient client;
        public PonyboxView()
        {
            InitializeComponent();
            Startup();
        }

        public void Startup()
        {
            
            LoginView v = new LoginView();
            v.ShowDialog();
            Tuple<string, string> token = v.data;

            this.client = new PonyboxClient();
            this.client.OnChannelJoinedCallback = (c) => UpdateMessages(c);
            this.client.OnMessagesUpdatedCallback = (c) => UpdateMessages(c);

            this.client.Connect(token);
        }

        void UpdateMessages(Channel c)
        {
            this.Dispatcher.Invoke(() =>
            {
                IEnumerable<Message> source = c.Messages;
                this.GridMessages.ItemsSource = source;
            });
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {
            string to = To.Text;
            string message = Message.Text;

            this.client.SendMessage("general", message, to);
        }
    }
}
