using ChatCore.Messages;
using NetMQ;
using NetMQ.Sockets;
using System;

namespace ChatClient.Services
{
    public class ChatService
    {
        private RequestSocket m_requestSocket;
        private SubscriberSocket m_subSocket;
        private NetMQPoller m_poller;

        public bool IsConnected { get; private set; }
        public string Sender { get; private set; }
        public int Port { get; private set; }

        public event EventHandler<MessageText> MessageArrived;
        public event EventHandler<MessageUserList> UserListUpdate;
        public event EventHandler<MessageServerDisconnect> ServerDisconnected;

        public ChatService()
        {
            Sender = null;
            IsConnected = false;
        }

        public bool Connect(int port, string sender)
        {
            try
            {
                Port = port;
                Sender = sender;
                m_requestSocket = new();
                m_subSocket = new();
                m_poller = new();
                m_requestSocket.Connect($"tcp://localhost:{Port}");
                m_subSocket.Connect($"tcp://localhost:{Port + 1}");
                m_subSocket.ReceiveReady += SubEventHandler;

                m_subSocket.SubscribeToAnyTopic();
                m_poller.Add(m_subSocket);
                m_poller.RunAsync("0MQ Thread", true);

                IsConnected = SendConnect();
                if (!IsConnected)
                {
                    StopAll();
                }

                return IsConnected;
            }
            catch
            {
                return false;
            }
        }

        private void StopAll()
        {
            try
            {
                m_poller.StopAsync();
                m_poller.Remove(m_subSocket);

                m_requestSocket.Disconnect($"tcp://localhost:{Port}");
                m_subSocket.Disconnect($"tcp://localhost:{Port + 1}");
                m_subSocket.Unsubscribe("");
            }
            finally
            {
                //m_poller?.Dispose();
                //m_subSocket?.Dispose();
                //m_requestSocket?.Dispose();
                m_poller = null;
                m_subSocket = null;
                m_requestSocket = null;
            }
        }

        public bool Disconnect(bool sendDisconnect = true)
        {
            try
            {
                if (!IsConnected)
                {
                    return false;
                }

                bool disconnected = true;
                if (sendDisconnect)
                {
                    disconnected = SendDisconnect();
                }

                StopAll();

                return disconnected;
            }
            catch
            {
                return false;
            }
        }

        private bool ReceiveSignal()
        {
            return m_requestSocket.TryReceiveSignal(TimeSpan.FromSeconds(5), out bool signal) && signal;
        }

        private bool SendConnect()
        {
            MessageConnect msg = new(Sender);
            bool resp = msg.Send(m_requestSocket);
            return resp && ReceiveSignal();
        }

        private bool SendDisconnect()
        {
            MessageDisconnect msg = new(Sender);
            bool resp = msg.Send(m_requestSocket);
            return resp;
        }

        public bool SendTextMessage(string sender, string message)
        {
            IMessage msgSend = new MessageText(sender, message);
            msgSend.Send(m_requestSocket);

            return ReceiveSignal();
        }

        private void SubEventHandler(object sender, NetMQSocketEventArgs e)
        {
            NetMQSocket socket = e.Socket;
            IMessage msg = MessageBase.Create(socket);
            switch (msg.MessageType)
            {
                case MessageType.TEXT:
                    MessageArrived?.Invoke(this, msg as MessageText);
                    break;
                case MessageType.USER_LIST:
                    UserListUpdate?.Invoke(this, msg as MessageUserList);
                    break;
                case MessageType.SERVER_DISCONNECT:
                    ServerDisconnected?.Invoke(this, msg as MessageServerDisconnect);
                    break;
                default:
                    break;
            };
        }
    }
}
