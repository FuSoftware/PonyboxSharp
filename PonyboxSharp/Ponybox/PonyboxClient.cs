using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quobject.EngineIoClientDotNet.ComponentEmitter;
using Quobject.SocketIoClientDotNet.Client;


namespace PonyboxSharp.Ponybox
{

    public class PonyboxClient
    {
        public struct LoginData
        {
            public string user { get; set; }
            public string token { get; set; }
            public LoginData(string user, string token)
            {
                this.user = user;
                this.token = token;
            }
        }

        public struct SendMessageData
        {
            public string to { get; set; }
            public string channel { get; set; }
            public string message { get; set; }
            public SendMessageData(string channel, string message, string to)
            {
                this.to = to == "" ? null : to;
                this.channel = channel;
                this.message = message;
            }
        }

        static string cb_address = "https://www.frenchy-ponies.fr:2096";
        Socket socket;

        public List<Channel> Channels { get; set; }
        public List<Message> Messages { get; set; }
        public Action<Channel> OnChannelJoinedCallback;
        public Action<Channel> OnMessagesUpdatedCallback;

        public PonyboxClient()
        {
            Channels = new List<Channel>();
            Messages = new List<Message>();
            OnChannelJoinedCallback = null;
        }

        public bool Connect(Tuple<string, string> token)
        {
            return Connect(token.Item1, token.Item2);
        }

        public bool Connect(string uid, string token)
        {
            socket = IO.Socket(cb_address);

            socket.On(Socket.EVENT_CONNECT_ERROR, (oError) =>
            {
                Log("EVENT_CONNECT_ERROR", oError.ToString());
            });

            socket.On(Socket.EVENT_CONNECT_TIMEOUT, (oError) =>
            {
                Log("EVENT_CONNECT_TIMEOUT", oError.ToString());
            });

            socket.On(Socket.EVENT_ERROR, (oError) =>
            {
                Log("EVENT_ERROR", oError.ToString());
            });

            socket.On(Socket.EVENT_CONNECT, () =>
            {
                Console.WriteLine("On : {0}", "CONNECT", "Connected");

                JToken data = JToken.FromObject(new LoginData(uid, token));
                Console.WriteLine("Emit : {0} {1} {2}", "create", uid, token);
                
                socket.Emit("create", new AckImpl((oReturn) =>
                {
                    Console.WriteLine("Recieved ack from creation");
                    Log("create", oReturn);
                    socket.Emit("login");
                }), data);
            });

            socket.On("join-channel", (dynamic oChannel) =>
            {
                Log("join-channel", oChannel);
                this.OnJoinChannel(new Channel(oChannel, this));
            });

            socket.On("edit-message", (dynamic oMessage) =>
            {
                Log("edit-message", oMessage);
                this.OnEditMessage(new Message(oMessage, this));
            });

            socket.On("get-older-message", (dynamic oParams) =>
            {
                Log("get-older-message", oParams);
                List<Message> messages = new List<Message>();

                for (int i = 0; i < oParams["messages"].Count; i++)
                {
                    messages.Add(new Message(oParams["messages"][i], this));
                }

                string channel = oParams["channel"];
                this.OnGetOlderMessages(channel, messages);
            });

            socket.On("refresh-channel-users", (dynamic oParams) =>
            {
                Log("refresh-channel-users", oParams);
            });

            socket.On("refresh-new-channels", (dynamic aChannels) =>
            {
                Log("refresh-new-channels", aChannels);
                Console.WriteLine(aChannels);
            });

            socket.On("delete-message", (dynamic oParams) =>
            {
                Log("delete-message", oParams);
                string id = oParams["id"];
                this.OnDeleteMessage(id);
            });

            socket.On("edit-message", (dynamic oMessage) =>
            {
                Log("edit-message", oMessage);
                this.OnEditMessage(new Message(oMessage, this));
            });

            socket.On("new-message", (dynamic oMessage) =>
            {
                Log("new-message", oMessage);
                this.OnNewMessage(new Message(oMessage, this));
            });

            socket.On("login-success", () =>
            {
                Log("login-success", "");
            });

            return false;
        }

        private void OnEditMessage(Message message)
        {
            this.Messages.RemoveAll(m => m.ID == message.ID);
            this.Messages.Add(message);
            OnMessagesUpdatedCallback?.Invoke(message.Channel);
        }

        private void OnNewMessage(Message message)
        {
            this.Messages.Add(message);
            OnMessagesUpdatedCallback?.Invoke(message.Channel);
        }

        private void OnDeleteMessage(string id)
        {
            Message m = this.Messages.Single(n => n.ID == long.Parse(id));
            this.Messages.Remove(m);
            OnMessagesUpdatedCallback?.Invoke(m.Channel);
        }

        private void OnJoinChannel(Channel channel)
        {
            this.Channels.Add(channel);
            OnChannelJoinedCallback?.Invoke(channel);
        }

        private void OnRefreshChannels(List<Channel> channels)
        {
            this.Channels = channels;
        }

        private void OnGetOlderMessages(string channel, List<Message> messages)
        {
            this.Messages.AddRange(messages);
            OnMessagesUpdatedCallback?.Invoke(this.Channels.Single(c=>c.Name == channel));
        }

        private void OnRefreshUsers(string channel, List<User> users)
        {
            this.Channels.Single(c => c.Name == channel).Users = users;
        }

        private void Log(string label, dynamic data, bool show = false)
        {
            Log(label, show?data.ToString():"");
        }

        private void Log(string label, string data)
        {
            Console.WriteLine("On : {0} {1}", label, data);
        }

        public void SendMessage(string channel, string message, string to)
        {
            Log("send-message","");
            dynamic oJson = JToken.FromObject(new SendMessageData(channel, message, to));
            this.socket.Emit("send-message", new AckImpl((oReturn) =>
            {
                Log("send-message-ack", oReturn, true);
            }), oJson);
        }

        public IEnumerable<Message> ChannelMessages(string channel)
        {
            if(this.Channels.Any(c=>c.Name == channel))
            {
                return this.Channels.Single(c => c.Name == channel).Messages;
            }
            else
            {
                return null;
            }
        }
    }
}
