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

        //args - name, port
        static void Main(string[] args){

            string name = args[0];
            string port;

            //use default port if none especified
             if(args.Length < 2){
                port = "8080";
            } else {
                port = args[1];
            }
            
            //create tcp channel
            TcpChannel channel = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(channel, false); 

            //create client
            Client client = new Client(name, port);
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
            Console.WriteLine("Client " + name +" created");
            Console.WriteLine("PID: " +
                Process.GetCurrentProcess().Id.ToString());

            //prevents process from closing
            System.Console.ReadLine();
        }
    }

}