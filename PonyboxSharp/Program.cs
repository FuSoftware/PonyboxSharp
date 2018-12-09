using PonyboxSharp.Ponybox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonyboxSharp
{
    class Program
    {
        public static void Main()
        {
            Tuple<string,string> token = LoginClient.Login("Nacle", "10judge02");
            PonyboxClient client = new PonyboxClient();
            client.Connect(token);
            Console.ReadLine();
        }
    }
}
