using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace RemotingSample {

	class Server {

		static void Main(string[] args) {

			TcpChannel channel = new TcpChannel(8086);
			ChannelServices.RegisterChannel(channel,false);
      
			ServerChat server = new ServerChat();

			RemotingServices.Marshal(
				server,
				"ServerChat",
				typeof(ServerChat));
			
			System.Console.WriteLine("<enter> para sair...");
			System.Console.ReadLine();
		}
	}

	class ServerChat : MarshalByRefObject,IServerChat{

		public string Ping(){
			return "server is online";
		}

		public void AddUser(string nick, string url){
			//TODO connect to client and save it
		}

		public void SendServer(string nick, string message){
			//TODO send message to every user except the original sender
		}

	}
}