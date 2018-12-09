using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonyboxSharp.Ponybox
{
    public class User
    {
        long uid;
        string username;
        string color;
        string avatar;
        Dictionary<string, bool> rights = new Dictionary<string, bool>();

        public User(string input)
        {
            loadUser(JsonConvert.DeserializeObject(input));
        }

        public User(dynamic oJson)
        {
            loadUser(oJson);
        }

        public void loadUser(dynamic oJson)
        {
            uid = oJson.uid;
            username = oJson.username;
            color = oJson.color;
            avatar = oJson.avatar;

            //Rights
            rights.Add("admin", (bool)oJson.rights.admin);
            rights.Add("modo", (bool)oJson.rights.modo);
            rights.Add("hot", (bool)oJson.rights.hot);
            rights.Add("quizz", (bool)oJson.rights.quizz);
            rights.Add("edit", (bool)oJson.rights.edit);
            rights.Add("delete", (bool)oJson.rights.delete);
        }

        public string Name { get=>this.username;}

        public long GetUid()
        {
            return this.uid;
        }
    }
}
