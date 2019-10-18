using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
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
        public TextBox box;
        public TextBlock block;
        public Button button;

        public MainWindow()
        {
            InitializeComponent();
            //get UI objects here

            box = this.Find<TextBox>("box");//find in this window
            block = this.Find<TextBlock>("block");

            box.Text = "banana2";
            block.Text = "banana3";
            System.Console.WriteLine(box.Text);
            System.Console.WriteLine(block.Text);

            TcpChannel channel = new TcpChannel();
			ChannelServices.RegisterChannel(channel,false);
		
			IServerChat server = (IServerChat) Activator.GetObject(
				typeof(IServerChat),
				"tcp://localhost:8086/ServerChat");

			try{
				//Console.WriteLine(server.Ping());
                block.Text = server.Ping();
			} catch(SocketException){
				System.Console.WriteLine("socket error");
			}

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}