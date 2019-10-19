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

        public MainWindow()
        {
            InitializeComponent();
            //get UI objects here

            serverBlock = this.Find<TextBlock>("ServerBlock");
            portBox = this.Find<TextBox>("PortBox");//find in this window
            nickBox = this.Find<TextBox>("NickBox");
            chatBlock = this.Find<TextBlock>("ChatBlock");
            messageBox = this.Find<TextBox>("MessageBox");

            TcpChannel channel = new TcpChannel();
			ChannelServices.RegisterChannel(channel,false);
		
			IServerChat server = (IServerChat) Activator.GetObject(
				typeof(IServerChat),
				"tcp://localhost:8086/ServerChat");

			try{
				//Console.WriteLine(server.Ping());
                serverBlock.Text = server.Ping();
			} catch(SocketException){
				System.Console.WriteLine("socket error");
			}

        }

        public void Connect(Object sender, RoutedEventArgs e){
            messageBox.Text = "button test";
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}