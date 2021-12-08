using ChatClient.Services;
using ChatCore.Messages;
using System.Windows;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
        : Window
    {
        private readonly ChatService m_chatService;

        public MainWindow(ChatService chatService)
        {
            m_chatService = chatService;
            m_chatService.MessageArrived += OnMessageArrived;
            m_chatService.UserListUpdate += M_chatService_UserListUpdate;
            m_chatService.ServerDisconnected += M_chatService_ServerDisconnected;
            InitializeComponent();
        }

        private void M_chatService_ServerDisconnected(object sender, MessageServerDisconnect e)
        {
            Dispatcher.Invoke(() =>
            {
                m_chatService.Disconnect(false);
                UserListLB.Items.Clear();
                UpdateFields(true);
                _ = MessageBox.Show("Server disconnected.");
            });
        }

        private void M_chatService_UserListUpdate(object sender, MessageUserList e)
        {
            Dispatcher.Invoke(() =>
            {
                UserListLB.Items.Clear();
                e.UserList.ForEach(u => UserListLB.Items.Add(u));
            });
        }

        private void OnMessageArrived(object sender, MessageText e)
        {
            Dispatcher.Invoke(() =>
            {
                MessageListLB.Items.Add($"{e.Sender}: {e.Text}");
            });
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTB.Text))
            {
                _ = MessageBox.Show("Please Input your name in the Name Text Box");
                return;
            }
            if (string.IsNullOrWhiteSpace(MessageTB.Text))
            {
                _ = MessageBox.Show("Please type a message in the message box");
                return;
            }
            m_chatService.SendTextMessage(NameTB.Text, MessageTB.Text);
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTB.Text))
            {
                _ = MessageBox.Show("Please Input your name in the Name Text Box");
                return;
            }
            if (string.IsNullOrWhiteSpace(ServerPortTB.Text) || !int.TryParse(ServerPortTB.Text, out int port))
            {
                _ = MessageBox.Show("Please Input the port to connect and it must be numeric");
                return;
            }
            if (!m_chatService.Connect(port, NameTB.Text))
            {
                _ = MessageBox.Show("The port is not being used, please start the server on this port.");
                return;
            }
            UpdateFields(false);
        }

        private void UpdateFields(bool enable)
        {
            NameTB.IsEnabled = enable;
            ServerPortTB.IsEnabled = enable;
            ConnectB.IsEnabled = enable;
            DisconnectB.IsEnabled = !enable;
            SendMessageB.IsEnabled = !enable;
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            if (!m_chatService.Disconnect())
            {
                _ = MessageBox.Show("Error trying to disconnect.");
            }
            UpdateFields(true);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_chatService.Disconnect();
        }
    }
}
