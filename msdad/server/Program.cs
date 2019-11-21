﻿using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Serilog;
using lib;

namespace server{

    class Program{

        //args - server_id, url, max_faults, min_delay, max_delay
        static void Main(string[] args){

            string server_id = args[0];
            string url = args[1];
            string max_faults = args[2];
            string min_delay = args[3];
            string max_delay = args[4];
            
            //tcp://localhost:8080/Client --> example
            string port = url.Split(':')[2].Split('/')[0]; //8080
            string service = url.Split(':')[2].Split('/')[1]; //Client

            //create tcp channel
            TcpChannel channel = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(channel, false);

            int max_faults_int = Int32.Parse(max_faults);
            int min_delay_int = Int32.Parse(min_delay);
            int max_delay_int = Int32.Parse(max_delay);

            //Initialize debugger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
            // Log.Information("");
            // Log.Debug("");
            // Log.Error("");
            
            //create server, puppeteer and serverToServer
            Server server = new Server(server_id, url, max_faults_int, min_delay_int, max_delay_int);
            ServerPuppeteer puppeteer = new ServerPuppeteer(server);
            ServerToServer serverToServer = new ServerToServer(server);

            //marshall objects
            RemotingServices.Marshal(
                puppeteer,
                service + ServerInfo.puppeteerExtension,
                typeof(ServerPuppeteer));
            
            RemotingServices.Marshal(
                serverToServer,
                service + ServerInfo.toServerExtension,
                typeof(ServerToServer));

            RemotingServices.Marshal(
                server,
                service,
                typeof(Server));

            
            //DEBUG
            //Console.WriteLine("New server created");
            Log.Debug("New server created");

            //prevent process from closing
            System.Console.ReadLine();
        }
    }
}