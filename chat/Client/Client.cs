using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;

namespace RemotingSample {

	class Client {

		static void Main() {
			TcpChannel channel = new TcpChannel();
			ChannelServices.RegisterChannel(channel,false);
		
			IServerChat server = (IServerChat) Activator.GetObject(
				typeof(IServerChat),
				"tcp://localhost:8086/ServerChat");

			try{
				Console.WriteLine(server.Ping());
			} catch(SocketException){
				System.Console.WriteLine("socket error");
			}

			//end of file
			Console.ReadLine();
		}
	}
}