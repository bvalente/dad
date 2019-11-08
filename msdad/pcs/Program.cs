using System;
using lib;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;


namespace pcs{

    class Program{

        static void Main(string[] args){
            
            string port = "10000";

            //create tcp channel
            TcpChannel channel = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(channel, false); 

            //create PCS
            PCS pcs = new PCS();
            RemotingServices.Marshal(
                pcs,
                "PCS",
                typeof(PCS));
            
            //DEBUG
            Console.WriteLine("New PCS created");

            //prevent process from closing
            System.Console.ReadLine();
        }
    }

    class PCS : MarshalByRefObject, IPCS{

        public string ping(){
            return "PCS is online";
        }

        public ClientInfo createClient(string username, string client_url, string server_url,
                 string script_file){

            //create client process
            string cPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(cPath,
                 "../../../../client/bin/Debug/net472/client.exe");
            Process client = Process.Start(filePath, 
                username + ' ' + client_url + ' ' + server_url + ' ' + script_file);

            return new ClientInfo(username, client_url, server_url, script_file);
        }

        public ServerInfo createServer(string server_id, string url,
                 string max_faults, string min_delay, string max_delay){
 
            //create server process
            string cPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(cPath,
                 "../../../../server/bin/Debug/net472/server.exe");
            Process server = Process.Start(filePath, 
                server_id + ' ' + url + ' ' + max_faults + ' ' + min_delay + ' ' + max_delay);

            return new ServerInfo(server_id, url, max_faults, min_delay, max_delay);
        }
    }
 }