using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

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
 }