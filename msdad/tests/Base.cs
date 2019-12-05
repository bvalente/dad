using System;
using lib;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace tests{

    public static class Base{

		public static void SetUpChannel(){
			var provider = new BinaryServerFormatterSinkProvider();
            provider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
            IDictionary props = new Hashtable();
            props["port"] = 9999;
            TcpChannel channel = new TcpChannel(props, null, provider);
            ChannelServices.RegisterChannel(channel, false);
			
		}

		public static T GetObject<T>(string url){
			return (T) Activator.GetObject(
				typeof(T),
				url);
		}

		public static IServer GetServer(ServerInfo serverInfo){
			return GetObject<IServer>(serverInfo.url);
		}
		public static IServerPuppeteer GetServerPuppeteer(ServerInfo serverInfo){
			return GetObject<IServerPuppeteer>(serverInfo.url_puppeteer);
		}
		public static IServerToServer GetServerToServer(ServerInfo serverInfo){
			return GetObject<IServerToServer>(serverInfo.url_to_server);
		}
		public static IClient GetClient(ClientInfo clientInfo){
			return GetObject<IClient>(clientInfo.client_url);
		}
		public static IClientPuppeteer GetClientPuppeteer(ClientInfo clientInfo){
			return GetObject<IClientPuppeteer>(clientInfo.client_url_puppeteer);
		}
		
	}
}