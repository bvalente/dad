using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using System;
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net;
using System.Net.Sockets;
using System.Windows.Input;
using System.Collections.Generic;
using lib;

namespace puppetMaster{

    public class MainWindow : Window{

        //Avalonia UI
        StackPanel pcsPanel;
        StackPanel clientPanel;
        StackPanel serverPanel;
        TextBox pcsIp;

        //PuppetMaster variables
        //client and server dictionaries
        Dictionary<ClientInfo,IClientPuppeteer> clientList;
        Dictionary<ServerInfo,IServerPuppeteer> serverList;
        int clientCounter = 0;


        public MainWindow(){

            InitializeComponent();

            //load all necessary components
            pcsPanel = this.Find<StackPanel>("PcsPanel");
            Console.WriteLine("Loading: " + pcsPanel.Name);
            clientPanel = this.Find<StackPanel>("ClientPanel");
            Console.WriteLine("Loading: " + clientPanel.Name);
            serverPanel = this.Find<StackPanel>("ServerPanel");
            Console.WriteLine("Loading: " + serverPanel.Name);
            pcsIp = this.Find<TextBox>("PcsIp");
            Console.WriteLine("Loading: " + pcsIp.Name);

            //create TCP channel on port 8075
            string port = "8075";
            TcpChannel channel = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(channel, false);

            //initialize dictionaries
            clientList = new Dictionary<ClientInfo, IClientPuppeteer>();
            serverList = new Dictionary<ServerInfo, IServerPuppeteer>();

        }

        private void InitializeComponent(){

            AvaloniaXamlLoader.Load(this);
        }

        //Buttons here! only 499$ each

        //connects to a pcs and creates the sctructure with buttons
        public void createPcs(object sender, RoutedEventArgs e){

            string ip = pcsIp.Text;
            
            //check if ip is valid
            IPAddress address = null;
            if( ! IPAddress.TryParse(ip, out address)){
                //localhost doenst work, use 127.0.0.1
                Console.WriteLine(ip + " is not a valid ip address.");
                return;
            }

            //connect to pcs
            IPCS pcs = (IPCS) Activator.GetObject(
                    typeof(IPCS),
                    "tcp://"+ip+":8070/PCS");

            //test pcs connectivity
            try{
                System.Console.WriteLine(pcs.ping());
            } catch (Exception ex){
                System.Console.WriteLine("pcs connectivity failed.");
                Console.WriteLine(ex.Message);
            }

            //create panel to organize everyting
            StackPanel panel = new StackPanel();
            panel.Orientation = Avalonia.Controls.Orientation.Horizontal;
            TextBlock block = new TextBlock();
            block.Text = ip;

            //create client button
            Button client = new Button();
            client.Content = "create client";
            Action<object> clientAction = createClient;
            client.Command = new CommandHandler(clientAction,true);
            client.CommandParameter = pcs;

            Button server = new Button();
            server.Content = "create server";
            //TODO program server connection
            /*
            Button disconnect = new Button();
            disconnect.Content = "disconnect";
            */
            //TODO program disconect
            //DISCUSS is this necessary???
            //disconect every client and server with same ip


            //add text and button to panel
            panel.Children.Add(block);
            panel.Children.Add(client);
            panel.Children.Add(server);
            //panel.Children.Add(disconnect);
            
            //add panel to window
            pcsPanel.Children.Add(panel);

        }

        //create client button action
        public void createClient(object pcs){
            //cast to IPCS object type
            IPCS PCS = (IPCS) pcs;
            ClientInfo clientInfo = PCS.createClient("client" + clientCounter.ToString());
            clientCounter++;

            //connect to client puppeteer
            IClientPuppeteer client = (IClientPuppeteer) Activator.GetObject(
                    typeof(IClientPuppeteer),
                    "tcp://"+clientInfo.Url+":"+clientInfo.Port+"/ClientPuppeteer");


            //test client connectivity
            Thread.Sleep(1000); //wait for client bootup
            try{
                System.Console.WriteLine(client.ping());
            } catch (Exception ex){
                System.Console.WriteLine("client connectivity failed.");
                Console.WriteLine(ex.Message);
            }

            clientList.Add(clientInfo,client);

            //TODO add client to the UI
        }

        public void createServer(object pcs){
            //TODO
            return;
        }

    }

    //credit: https://stackoverflow.com/questions/35370749/passing-parameters-to-mvvm-command/35371204
    public class CommandHandler : ICommand{
        private Action<object> _action;
        private bool _canExecute;
        
        public CommandHandler(Action<object> action, bool canExecute){
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter){

            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter){

            _action(parameter);
        }
    }
}