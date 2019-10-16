using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace RemotingSample {

	class Server {

		static void Main(string[] args) {

			TcpChannel channel = new TcpChannel(8086);
			ChannelServices.RegisterChannel(channel,false);

			RemotingConfiguration.RegisterWellKnownServiceType(
				typeof(MyRemoteObject),
				"MyRemoteObjectName",
				WellKnownObjectMode.Singleton);
      
			CServer server = new CServer();

			RemotingServices.Marshal(
				server,
				"MyServer",
				typeof(CServer));
			
			System.Console.WriteLine("<enter> para sair...");
			System.Console.ReadLine();
		}
	}

	class CServer : MarshalByRefObject,ICServer{

		public string name(){
			return "my name";
		}

	}
}