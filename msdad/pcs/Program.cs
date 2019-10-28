using System;
using lib;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;


namespace pcs
{
    class Program
    {
        static void Main(string[] args)
        {
            string port = "8070";

            //create tcp channel
            TcpChannel channel = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(channel, false); 


            //create PCS
            PCS pcs = new PCS();
            RemotingServices.Marshal(
                pcs,
                "PCS",
                typeof(PCS));
                      
            Console.WriteLine("New PCS created");
            System.Console.ReadLine();

        }
    }
}

namespace lib{
    class PCS : MarshalByRefObject, IPCS{

        public string ping(){
            return "PCS is online";
        }

        //TODO: VOLTAR A VER ESTA MERDA 
        //NAO SABER SE FUNCIONA
        //O FELI E GAY
        public ClientInfo createClient(string name, string port){
            string cPath = AppDomain.CurrentDomain.BaseDirectory;
            string client = Path.Combine(cPath, "../../../../client/bin/Debug/net472/client.exe");
            Process proc = Process.Start(client, name + ' ' + port);

            //return new ClientInfo(name, "localhost",port); //TODO populate
            System.Console.WriteLine("ClientInfo");
            return null;
        }
        

        public ServerInfo createServer(string port){
            string cPath = AppDomain.CurrentDomain.BaseDirectory;
            string server = Path.Combine(cPath, "../../../../server/bin/Debug/net472/server.exe");
            Process proc = Process.Start(server, port);

            //return new ServerInfo("localhost:///", port); //TODO populate
            return null;
            
        }
    }
 }