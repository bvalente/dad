using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;
using System.Diagnostics;
using Serilog;
using lib;

namespace client{

    class Program{
        
        static void Main(string[] args){

            string username = args[0];
            string client_url = args[1];
            string server_url = args[2];
            string script_file;
            if(args.Length == 4){
                script_file = args[3];
            } else{
                script_file = "";
            }
            

            //tcp://localhost:8080/Client --> example
            string port = client_url.Split(':')[2].Split('/')[0];
            string service = client_url.Split(':')[2].Split('/')[1];

            //create tcp channel
            var provider = new BinaryServerFormatterSinkProvider();
            provider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
            IDictionary props = new Hashtable();
            props["port"] = Int32.Parse(port);
            TcpChannel channel = new TcpChannel(props, null, provider);
            //TcpChannel channel = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(channel, false); 

            //Initialize debugger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("../logs/"+username+"_.log", rollingInterval: RollingInterval.Minute, rollOnFileSizeLimit: true)
                .CreateLogger();
            // Log.Information("");
            // Log.Debug("");
            // Log.Error("");

            //create client and puppeteer
            Log.Debug("creating objects");
            Client client = new Client(username, client_url, server_url, script_file);
            ClientPuppeteer puppeteer = new ClientPuppeteer(client);
            
            Log.Debug("marshalling objects");
            //marshall objects
            RemotingServices.Marshal(
                puppeteer,
                service + ClientInfo.puppeteerExtension,
                typeof(ClientPuppeteer));
            
            RemotingServices.Marshal(
                client,
                service,
                typeof(Client));

            
            //DEBUG
            Log.Debug("Client " + username +" created");
            Log.Debug("service " + service); 
            Log.Debug(client.username + " PID: " +
                Process.GetCurrentProcess().Id.ToString());

            //read input and execute commands
            string input = System.Console.ReadLine();
            while (input != "exit"){
                client.executeCommand(input);
                input = System.Console.ReadLine();
            }
        }
    }
}