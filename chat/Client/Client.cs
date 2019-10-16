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
		
			IChat server = (IChat) Activator.GetObject(
				typeof(IChat),
				"tcp://localhost:8086/Chat");

			try{
				Console.WriteLine(server.name());
			} catch(SocketException){
				System.Console.WriteLine("socket error");
			}

			//end of file
			Console.ReadLine();
		}
	}
}