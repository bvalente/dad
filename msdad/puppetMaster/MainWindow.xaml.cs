using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.IO;
using lib;

namespace puppetMaster{

    public class MainWindow : Window{

        //Avalonia UI
        StackPanel clientPanel;
        StackPanel serverPanel;
        TextBox puppetMasterScript;

        //CreateSeverPanel
        TextBox createServerID; 
        TextBox createServer_url;
        TextBox createServer_max_faults;
        TextBox createServer_min_delay;
        TextBox createServer_max_delay;

        //CreateClientPanel
        TextBox createClient_username;
        TextBox createClient_client_url;
        TextBox createClient_server_url;
        TextBox createClient_script_file;

        //AddRoomPanel
        TextBox room_location;
        TextBox room_capacity;
        TextBox room_name;

        //CrashPanel
        TextBox crashServerID;

        //FreezePanel
        TextBox freezeServerID;

        //UnfreezePanel
        TextBox unfreezeServerID;
        
        //WaitPanel
        TextBox waitTimeBox;

        //PuppetMaster variables
        PuppetMaster puppetMaster;

        public MainWindow(){

            InitializeComponent();
            
            //load all necessary components
            clientPanel = this.Find<StackPanel>("ClientPanelList");
            Console.WriteLine("Loading: " + clientPanel.Name);
            serverPanel = this.Find<StackPanel>("ServerPanelList");
            Console.WriteLine("Loading: " + serverPanel.Name);
            
            puppetMasterScript = this.Find<TextBox>("PuppetMasterScript");
            Console.WriteLine("Loading: " + puppetMasterScript.Name);

            createServerID = this.Find<TextBox>("createServerID");
            Console.WriteLine("Loading: " + createServerID.Name);
            createServer_url = this.Find<TextBox>("createServer_url");
            Console.WriteLine("Loading: " + createServer_url.Name);
            createServer_max_faults = this.Find<TextBox>("createServer_max_faults");
            Console.WriteLine("Loading: " + createServer_max_faults.Name);
            createServer_min_delay = this.Find<TextBox>("createServer_min_delay");
            Console.WriteLine("Loading: " + createServer_min_delay.Name);
            createServer_max_delay = this.Find<TextBox>("createServer_max_delay");
            Console.WriteLine("Loading: " + createServer_max_delay.Name);
            
               
            createClient_username = this.Find<TextBox>("createClient_username");
            Console.WriteLine("Loading: " + createClient_username.Name);
            createClient_client_url = this.Find<TextBox>("createClient_client_url");
            Console.WriteLine("Loading: " + createClient_client_url.Name);
            createClient_server_url = this.Find<TextBox>("createClient_server_url");
            Console.WriteLine("Loading: " + createClient_server_url.Name);
            createClient_script_file = this.Find<TextBox>("createClient_script_file");
            Console.WriteLine("Loading: " + createClient_script_file.Name);
            
            room_location = this.Find<TextBox>("room_location");
            Console.WriteLine("Loading: " + room_location.Name);
            room_capacity = this.Find<TextBox>("room_capacity");
            Console.WriteLine("Loading: " + room_capacity.Name);
            room_name = this.Find<TextBox>("room_name");
            Console.WriteLine("Loading: " + room_name.Name);
            
            crashServerID = this.Find<TextBox>("CrashServerID");
            Console.WriteLine("Loading: " + crashServerID.Name);

            freezeServerID = this.Find<TextBox>("FreezeServerID");
            Console.WriteLine("Loading: " + freezeServerID.Name);

            unfreezeServerID = this.Find<TextBox>("UnfreezeServerID");
            Console.WriteLine("Loading: " + unfreezeServerID.Name);
            
            waitTimeBox = this.Find<TextBox>("WaitTime");
            Console.WriteLine("Loading: " + waitTimeBox.Name);

            
            puppetMaster = PuppetMaster.getPuppetMaster(this);

            //create TCP channel on port 10001
            string port = "10001";
            TcpChannel channel = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(channel, false);

        }

        private void InitializeComponent(){

            AvaloniaXamlLoader.Load(this);
        }

        //Buttons here!

        //load script file and run it
        public void executeScript(object sender, RoutedEventArgs e){

            //load file and get commands
            string fileName = puppetMasterScript.Text;
            string cPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(cPath,
                 "../../../../scripts/" + fileName);
            try{
                string[] commands = System.IO.File.ReadAllLines(filePath);

                //execute puppetMaster.executeCommand n times
                foreach (string command in commands){
                    puppetMaster.executeCommand(command);
                }
            }catch(Exception ex){
                Console.WriteLine("cannot open file " + fileName);
                Console.WriteLine(ex.Message);
            }
        }

        //create server
        public void createServer(object sender, RoutedEventArgs e){

            string server_id = createServerID.Text; 
            string url =  createServer_url.Text;
            string max_faults = createServer_max_faults.Text;
            string min_delay = createServer_min_delay.Text;
            string max_delay =  createServer_max_delay.Text;

           puppetMaster.createServer(server_id, url, max_faults, min_delay, max_delay);

        }

        //create client
        public void createClient(object sender, RoutedEventArgs e){

            string username = createClient_username.Text;
            string url = createClient_client_url.Text;
            string server_url = createClient_server_url.Text;
            string script_file = createClient_script_file.Text;

            puppetMaster.createClient(username, url, server_url, script_file);
        }

        //add room
        public void addRoom(object sender, RoutedEventArgs e){
            
            string location = room_location.Text;
            string capacity = room_capacity.Text;
            string name = room_name.Text;
            
            puppetMaster.addRoom(location, capacity, name);
        }

        //print clients and servers status
        public void status(object sender, RoutedEventArgs e){
            
            puppetMaster.status();
        }

        //crash server
        public void crash(object sender, RoutedEventArgs e){
            string server_id = crashServerID.Text;
            puppetMaster.crashServer(server_id);

        }

        //freeze server
        public void freezeServer(object sender, RoutedEventArgs e){
            
            string server_id = freezeServerID.Text;
            puppetMaster.freezeServer(server_id);
        }

        //unfreeze server
        public void unfreezeServer(object sender, RoutedEventArgs e){
            
            string server_id = unfreezeServerID.Text;
            puppetMaster.unfreezeServer(server_id);
        }

        //freeze puppetmaster
        public void waitTime(object sender, RoutedEventArgs e){

            string time = waitTimeBox.Text;
            puppetMaster.wait(time);
        }

        //reset everything
        public void reset(object sender, RoutedEventArgs e){

            //reset puppetMaster
            puppetMaster.reset();

            //reset UI
            clientPanel.Children.Clear();
            serverPanel.Children.Clear();
        }

        //update client list
        public void addClient(ClientInfo client){
            TextBlock block = new TextBlock();
            block.Name = client.username;
            block.Text = client.username;
            clientPanel.Children.Add(block);
        }

        //update server list
        public void addServer(ServerInfo server){
            TextBlock block = new TextBlock();
            block.Text = server.server_id;
            block.Name = server.server_id;
            serverPanel.Children.Add(block);
        }

        //update server list
        public void removeServer(ServerInfo server){
            TextBlock block = this.Find<TextBlock>(server.server_id);
            serverPanel.Children.Remove(block);
        }
        
    }
}