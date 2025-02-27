using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections.Generic;
using System.Threading;

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

		private Dictionary<string,IClientChat> clientList = new Dictionary<string, IClientChat>();

		public string Ping(){
			return "server is online";
		}

		public void AddUser(string nick, string url){
			
			IClientChat client = (IClientChat) Activator.GetObject(
				typeof(IClientChat),
				url	);
			clientList.Add(nick, client);
			System.Console.WriteLine("Client " + nick + " added.");
		}

		public void SendServer(string nick, string message){
			string msg = nick + ": " + message;
			Console.WriteLine(msg);

			foreach (KeyValuePair<string, IClientChat> pair in clientList){
				if(!pair.Key.Equals(nick)){
					Console.WriteLine("sending message to " + pair.Key);
					pair.Value.SendClient(msg);
				}
			}
		}
		
	}
}