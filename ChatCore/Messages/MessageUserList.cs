using NetMQ;
using System.Collections.Generic;
using System.Text;

namespace ChatCore.Messages
{
    public class MessageUserList
        : MessageBase
    {
        public override MessageType MessageType => MessageType.USER_LIST;

        public List<string> UserList { get; } = new();

        public MessageUserList(List<string> userList)
        {
            UserList = userList;
        }

        public MessageUserList()
        {
        }

        public override bool Receive(NetMQSocket socket)
        {
            (bool result, NetMQMessage msg) = TryReceiveMultipartMessage(socket, 1);
            if (result)
            {
                //int.Parse(msg[0].ConvertToString()));
                UserList.Clear();
                foreach (NetMQFrame frame in msg)
                {
                    UserList.Add(frame.ConvertToString());
                }
            }
            return result;
        }

        public override bool Send(NetMQSocket socket)
        {
            //List<string> toSend = new() { UserList.Count.ToString() };
            //toSend.AddRange(UserList);
            return TrySendMultipartMessage(socket, UserList);
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            UserList.ForEach(u => sb.Append((sb.Length > 0 ? ", " : "") + u));
            return $"{MessageType}: [{sb}]";
        }
    }
}
