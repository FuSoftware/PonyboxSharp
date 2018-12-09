using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace PonyboxSharp.Ponybox
{
    public class Message
    {
        long id;
        string format;
        int type;
        bool isPrivate;
        User from;
        User to;
        string channel;
        long sendDate;
        Dictionary<string, bool> rights = new Dictionary<string, bool>();
        public Channel Channel { get; set; }

        dynamic oJson;

        public Message(string input, PonyboxClient parent)
        {
            loadMessage(JsonConvert.DeserializeObject(input));
            this.loadChannel(parent);
        }

        public Message(dynamic oJson, PonyboxClient parent)
        {
            loadMessage(oJson);
            this.loadChannel(parent);
        }

        private void loadChannel(PonyboxClient parent)
        {
            this.Channel = parent.Channels.Single(c => c.Name == this.channel);
        }

        public void loadMessage(dynamic oJson)
        {
            this.oJson = oJson;
            id = oJson.id;
            format = oJson.format;
            type = oJson.type;
            sendDate = oJson.sendDate;

            //from
            from = new User(oJson.from);

            //to
            if (oJson.to != null)
            {
                to = new User(oJson.to);
            }

            channel = oJson.channel;
            isPrivate = oJson["private"];
        }

        public string ChannelName { get => this.channel; }
        public long Timestamp { get => this.sendDate; }
        public string Data { get => this.format; }
        public User Sender { get => this.from; }
        public User Recipient { get => this.to; }
        public long ID { get => this.id; }
        public bool IsPrivate { get => this.isPrivate; }


        public object GetJson()
        {
            return this.oJson;
        }
    }
}
