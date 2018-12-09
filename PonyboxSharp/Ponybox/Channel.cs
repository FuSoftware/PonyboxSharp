using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonyboxSharp.Ponybox
{
    public class Channel
    {
        PonyboxClient Parent { get; set; }
        public List<User> Users { get; set; }
        dynamic oJson;

        string name;
        string label;
        bool locked;
        string description;

        public Channel(string input, PonyboxClient parent)
        {
            this.Parent = parent;
            loadChannel(JsonConvert.DeserializeObject(input));
        }

        public Channel(dynamic oJson, PonyboxClient parent)
        {
            this.Parent = parent;
            loadChannel(oJson);
        }

        public void loadChannel(dynamic oJson)
        {
            this.oJson = oJson;

            name = oJson.name;
            label = oJson.label;
            locked = oJson.locked;
            description = oJson.description;
        }

        public string Name { get => this.name; }

        public string Label { get => this.label; }

        public IEnumerable<Message> Messages {get => this.Parent.Messages.Where(m => m.ChannelName == this.name).OrderBy(m => m.ID);}
    }
}
