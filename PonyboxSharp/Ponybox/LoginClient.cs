using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PonyboxSharp.Ponybox
{
    class LoginClient
    {
        static string cb_login = "https://www.frenchy-ponies.fr/ucp.php?mode=login";
        static string cb_pb_login = "https://www.frenchy-ponies.fr/ponybox/pb-login.php";
        static string cb_include = "https://www.frenchy-ponies.fr/ponybox/pb-include.php";

        struct LoginParams
        {
            public string username;
            public string password;
        }

        public static Tuple<string, string> Login(string username, string password)
        {
            Tuple<string, string> token = LoginCB(username, password);
            if(token == null)
            {
                token = LoginForum(username, password);
                if(token == null)
                {
                    Console.WriteLine("Login failed");
                    return null;
                }
            }
            return token;
        }

        public static Tuple<string, string> LoginForum(string username, string password)
        {
            CookieAwareWebClient c = new CookieAwareWebClient();

            try
            {
                byte[] log_res = c.UploadValues(cb_login, "POST", new NameValueCollection()
                {
                    { "username", username },
                    { "password", password },
                    { "redirect", "index.php" },
                    { "login", "Connexion" },
                    //{ "sid", "a6cce2c9a9448a93eab7002f5f459829" }
                });

                string s = Encoding.Default.GetString(log_res);

                string res = c.DownloadString(cb_include);
                Console.WriteLine(res);
                return ParseToken(res);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception for " + username + "/" + password);
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static Tuple<string, string> LoginCB(string username, string password)
        {
            CookieAwareWebClient c = new CookieAwareWebClient();
            c.Headers.Add(HttpRequestHeader.ContentType, "application/json");

            try
            {
                /*
                byte[] log_res = c.UploadValues(cb_pb_login, "POST", new NameValueCollection()
                {
                    { "username", username },
                    { "password", password },
                });
                */
                var d = new LoginParams();
                d.username = username;
                d.password = password;
                string json = JsonConvert.SerializeObject(d);
                string res = c.UploadString(cb_pb_login, json);
                dynamic oRes = JsonConvert.DeserializeObject(res);

                if ((bool)oRes.valid)
                {
                    string id = oRes.user.id;
                    string token = oRes.user.token;
                    return Tuple.Create<string, string>(id, token);
                }
                else
                {
                    Console.WriteLine("Received reply but login was invalid");
                    Console.WriteLine(res);
                    return null;
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception for " + username + "/" + password);
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static Tuple<string, string> ParseToken(string json)
        {
            dynamic oJson = JsonConvert.DeserializeObject(json);

            string token = oJson.token;
            string sid = oJson.sid;

            Match m = Regex.Match(sid, @"^user([0-9]+)\.pony");
            string id = m.Groups[1].Value;
            
            return Tuple.Create<string, string>(id, token);
        }
    }
}
