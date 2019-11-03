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
            
            string ip;
            string port = "8070";

            if(args.Length == 0){
                ip = "127.0.0.1";
            } else {
                ip = args[0];
            }

            //create tcp channel
            TcpChannel channel = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(channel, false); 

            //create PCS
            PCS pcs = new PCS(ip);
            RemotingServices.Marshal(
                pcs,
                "PCS",
                typeof(PCS));
            
            //DEBUG
            Console.WriteLine("New PCS created on "+ ip);

            //prevent process from closing
            System.Console.ReadLine();
        }
    }

    class PCS : MarshalByRefObject, IPCS{

        string ip;
        //ports are automatically atributed
        int clientPort = 8080;
        int serverPort = 8090;

        //constructor
        public PCS(string ip){
            this.ip = ip;
        }

        public string ping(){
            return "PCS "+ip+" is online";
        }

        //TODO: VOLTAR A VER ESTA MERDA 
        //NAO SABER SE FUNCIONA
        //O FELI E GAY
        public ClientInfo createClient(string name){
            //get next client port
            string port = clientPort.ToString();
            clientPort++;

            string cPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(cPath,
                 "../../../../client/bin/Debug/net472/client.exe");
            Process client = Process.Start(filePath, name + ' ' + port);

            return new ClientInfo(name, "localhost",port); //TODO populate
            System.Console.WriteLine("ClientInfo");
            return null;
        }

        public ServerInfo createServer(){
            //get next server port 
            string port = serverPort.ToString();
            serverPort++;

            string cPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(cPath,
                 "../../../../server/bin/Debug/net472/server.exe");
            Process server = Process.Start(filePath, port);

            //return new ServerInfo("localhost:///", port); //TODO populate
            return null;
        }
    }
 }