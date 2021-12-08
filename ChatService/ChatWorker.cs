using ChatCore.Messages;
using ChatService.Commands;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatService
{
    public class ChatWorker
        : BackgroundService
    {
        private readonly ILogger<ChatWorker> m_logger;
        private readonly int m_mainPort;
        private ResponseSocket m_responseSocket;
        private PublisherSocket m_pubSocket;
        private NetMQPoller m_poller;
        private readonly IMediator m_mediator;
        private readonly HashSet<string> m_connected = new();

        public ChatWorker(ILogger<ChatWorker> logger,
                          IOptions<AppSettings> appSettings,
                          IConfiguration configuration,
                          IMediator mediator)
        {
            m_logger = logger;
            m_mediator = mediator;
            m_mainPort = appSettings.Value.ZeroMq.Port;
            if (!string.IsNullOrEmpty(configuration.GetValue<string>("port")))
            {
                m_mainPort = configuration.GetValue<int>("port");
            }
        }

        private void RespEventHandler(object sender, NetMQSocketEventArgs e)
        {
            NetMQSocket socket = e.Socket;
            IMessage msg = MessageBase.Create(socket);
            m_logger.LogInformation(msg.ToString());
            socket.SignalOK();
            switch (msg.MessageType)
            {
                case MessageType.TEXT:
                    _ = m_mediator.Send(new MessageTextCommand(msg as MessageText, m_pubSocket));
                    break;
                case MessageType.CONNECT:
                    {
                        MessageConnect msgConnect = msg as MessageConnect;
                        _ = m_connected.Add(msgConnect.Sender);
                        MessageUserList msgUserList = new(m_connected.ToList());
                        _ = m_mediator.Send(new MessageUserListCommand(msgUserList, m_pubSocket));
                    }
                    break;
                case MessageType.DISCONNECT:
                    {
                        MessageDisconnect msgDisconnect = msg as MessageDisconnect;
                        _ = m_connected.Remove(msgDisconnect.Sender);
                        MessageUserList msgUserList = new(m_connected.ToList());
                        _ = m_mediator.Send(new MessageUserListCommand(msgUserList, m_pubSocket));
                    }
                    break;
                default:
                    break;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            m_logger.LogInformation($"Chat Server started running in port {m_mainPort} at: {DateTimeOffset.Now}");
            m_logger.LogInformation($"Type \"Q\" and \"ENTER\" to quit server");

            try
            {
                m_responseSocket = new();
                m_pubSocket = new();

                m_responseSocket.Bind($"tcp://*:{m_mainPort}");
                m_pubSocket.Bind($"tcp://*:{m_mainPort + 1}");
                m_responseSocket.ReceiveReady += RespEventHandler;

                m_poller = new();
                m_poller.Add(m_responseSocket);
                m_poller.RunAsync("0MQ Thread", true);

                while (!stoppingToken.IsCancellationRequested)
                {
                    string text = Console.In.ReadLine();
                    if (text.Equals("Q", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                    await Task.Delay(200, stoppingToken);
                }
                _ = m_mediator.Send(new MessageServerDisconnectCommand(new MessageServerDisconnect(), m_pubSocket));

                m_poller.Dispose();
                m_responseSocket.Dispose();
                m_pubSocket.Dispose();
            }
            catch (AddressAlreadyInUseException e)
            {
                m_logger.LogError("Port alredy in use: {message}", e.Message);
            }
            catch (Exception e)
            {
                m_logger.LogError("Exception: {message}", e.Message);
            }

            m_logger.LogInformation("Chat Server ending.");
        }
    }
}
