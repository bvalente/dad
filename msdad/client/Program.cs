using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Diagnostics;

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
            TcpChannel channel = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(channel, false); 

            //create client and puppeteer
            Client client = new Client(username, client_url, server_url, script_file);
            ClientPuppeteer puppeteer = new ClientPuppeteer(client);
            
            //marshall objects
            RemotingServices.Marshal(
                puppeteer,
                service + "Puppeteer",
                typeof(ClientPuppeteer));
            
            RemotingServices.Marshal(
                client,
                service,
                typeof(Client));

            
            //DEBUG
            Console.WriteLine("Client " + username +" created");
            Console.WriteLine("service " + service); 
            Console.WriteLine(client.username + " PID: " +
                Process.GetCurrentProcess().Id.ToString());

            //read input and execute commands
            string input = System.Console.ReadLine();
            while (input != "exit"){
                client.executeCommand(input);
                System.Console.ReadLine();
            }
        }
    }
}