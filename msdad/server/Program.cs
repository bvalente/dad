using System;
using lib;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;

namespace server{

    class Program{

        //args - port
        static void Main(string[] args){

            string port;

            //use defualt port if none is specified
            if(args.Length == 0){
                port = "8090";
            } else {
                port = args[0];
            }

            //create tcp channel
            TcpChannel channel = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(channel, false);

            //create server
            Server server = new Server(port);
            RemotingServices.Marshal(
                server,
                "Server",
                typeof(Server));

            //create server puppeteer
            ServerPuppeteer puppeteer = new ServerPuppeteer(server);
            RemotingServices.Marshal(
                puppeteer,
                "ServerPuppeteer",
                typeof(ServerPuppeteer));
            
            //DEBUG
            Console.WriteLine("New server created");

            //prevent process from closing
            System.Console.ReadLine();
        }
    }
}