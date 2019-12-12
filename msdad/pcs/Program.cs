using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;
using Serilog;

namespace pcs{

    class Program{

        static void Main(string[] args){
            
            string port = "10000";

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
                .CreateLogger();

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