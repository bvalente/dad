using System;
using lib;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

namespace client{

    class Program{
        
        static void Main(string[] args){

            string username = args[0];
            string client_url = args[1];
            string server_url = args[2];
            string script_file = args[3];

            //tcp://localhost:8080/Client --> example
            string port = client_url.Split(':')[2].Split('/')[0];

            //create tcp channel
            TcpChannel channel = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(channel, false); 

            //create client
            Client client = new Client(username, client_url, server_url, script_file);
            RemotingServices.Marshal(
                client,
                "Client",
                typeof(Client));
            
            //create client puppeteer
            ClientPuppeteer puppeteer = new ClientPuppeteer(client);
            RemotingServices.Marshal(
                puppeteer,
                "ClientPuppeteer",
                typeof(ClientPuppeteer));
            
            //DEBUG
            Console.WriteLine("Client " + username +" created");
            Console.WriteLine(client.username + " PID: " +
                Process.GetCurrentProcess().Id.ToString());

            //prevents process from closing
            System.Console.ReadLine();
        }
    }

}