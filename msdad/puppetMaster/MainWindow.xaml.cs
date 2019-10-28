using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using lib;

namespace puppetMaster
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void pcsTest(Object sender, RoutedEventArgs e){
            string port = "8075";
            TcpChannel channel = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(channel, false);  

            IPCS pcs = (IPCS) Activator.GetObject(
                    (typeof(IPCS)),
                    "tcp://localhost:8070/PCS");

            try{
                System.Console.WriteLine(pcs.ping());
            } catch (SocketException ex){
                System.Console.WriteLine("PCS NOT GREAT, RAPE");    
            }

            pcs.createClient("AndreValenteNotGayYesNoUYeah", "8080");
        }
    }
}