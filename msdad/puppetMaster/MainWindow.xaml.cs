using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using System.Windows.Input;
using lib;

namespace puppetMaster{

    public class MainWindow : Window{

        StackPanel pcsPanel;
        StackPanel clientPanel;
        StackPanel serverPanel;
        TextBox pcsIp;

        public MainWindow(){

            InitializeComponent();

            //load all necessary components
            pcsPanel = this.Find<StackPanel>("PcsPanel");
            Console.WriteLine(pcsPanel.Name + " Loaded.");
            clientPanel = this.Find<StackPanel>("ClientPanel");
            Console.WriteLine(clientPanel.Name + " Loaded.");
            serverPanel = this.Find<StackPanel>("ServerPanel");
            Console.WriteLine(serverPanel.Name + " Loaded.");
            pcsIp = this.Find<TextBox>("PcsIp");
            Console.WriteLine(pcsIp.Name + " Loaded.");

        }

        private void InitializeComponent(){

            AvaloniaXamlLoader.Load(this);
        }

        //Buttons here! only 499$ each

        public void pcsTest(Object sender, RoutedEventArgs e){
            string port = "8075";

            //create tcp channel
            TcpChannel channel = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(channel, false);

            //get pcs
            IPCS pcs = (IPCS) Activator.GetObject(
                    (typeof(IPCS)),
                    "tcp://localhost:8070/PCS");

            //test pcs connectivity
            try{
                System.Console.WriteLine(pcs.ping());
            } catch (Exception ex){
                System.Console.WriteLine("PCS NOT GREAT, RAPE");
                Console.WriteLine(ex.Message);
            }

            //create client trough pcs
            pcs.createClient("AndreValenteNotGayYesNoUYeah", "8080");
        }

        public void pcsTest2(Object sender, RoutedEventArgs e){
            //test
            TextBlock block = new TextBlock();
            block.Text = "general kenoby";

            Button button = new Button();
            button.Content = "test button";
            Action<object> myAction = pcsTest3;
            CommandHandler myCommand = new CommandHandler(myAction,true);
            button.Command = myCommand;
            button.CommandParameter = "hallelujah";
            
            pcsPanel.Children.Add(block);
            pcsPanel.Children.Add(button);

        }

        public void pcsTest3(object print){
            //test
            Console.WriteLine("ewan mcgregor");
            Console.WriteLine(print.ToString());
        }

        //connects to a pcs and creates the sctructure with buttons
        public void createPcs(object sender, RoutedEventArgs e){

            string ip = pcsIp.Text;
            //TODO check if valid ip address

            //TODO connect to pcs

            //TODO create pcs UI structure
            //create panel to organize everyting
            StackPanel panel = new StackPanel();
            panel.Orientation = Avalonia.Controls.Orientation.Horizontal;
            TextBlock block = new TextBlock();
            block.Text = ip;
            Button client = new Button();
            client.Content = "create client";
            //TODO program client connection
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

    }

    public class CommandHandler : ICommand{
        //credit: https://stackoverflow.com/questions/35370749/passing-parameters-to-mvvm-command/35371204
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