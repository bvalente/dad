using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using RemotingSample;
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;

namespace ClientUI
{
    public class MainWindow : Window
    {
        public TextBlock serverBlock;
        public TextBox portBox;
        
        public TextBox nickBox;
        public Button connectButton;
        public TextBlock chatBlock;
        public TextBox messageBox;
        public Button sendButton;
        private IServerChat server;
        private ClientChat client;

        public MainWindow()
        {
            InitializeComponent();

            //get UI objects here
            serverBlock = this.Find<TextBlock>("ServerBlock");
            portBox = this.Find<TextBox>("PortBox");
            nickBox = this.Find<TextBox>("NickBox");
            connectButton = this.Find<Button>("ConnectButton");
            chatBlock = this.Find<TextBlock>("ChatBlock");
            messageBox = this.Find<TextBox>("MessageBox");
            sendButton = this.Find<Button>("SendButton");            

        }

        public void Connect(Object sender, RoutedEventArgs e){
            
            
            string port = portBox.Text;
            string nick = nickBox.Text;
            string url = "tcp://localhost:"+port+"/ClientChat";

            //create connection
            TcpChannel channel = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(channel,false);
        
            server = (IServerChat) Activator.GetObject(
                typeof(IServerChat),
                "tcp://localhost:8086/ServerChat");

            try{
                //Console.WriteLine(server.Ping());
                serverBlock.Text = server.Ping();
            } catch(SocketException){
                System.Console.WriteLine("socket error");
            }

            //marshal client chat
            
            client = new ClientChat(this);
            RemotingServices.Marshal(
                client,
                "ClientChat",
                typeof(ClientChat));

            //send info to server
            server.AddUser(nick, url); //catch exception?
        
            portBox.IsEnabled=false;
            nickBox.IsEnabled=false;
            connectButton.IsEnabled=false;
        }

        public void Send(Object sender, RoutedEventArgs e){
            string message = messageBox.Text;
            string nick = nickBox.Text;
            try{
                server.SendServer(nick, message);
            } catch(SocketException){
                System.Console.WriteLine("socket error");
            }
            
            updateChat("you: " + message);

        }

        public void updateChat(string message){
            chatBlock.Text = chatBlock.Text + '\n' + message;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}