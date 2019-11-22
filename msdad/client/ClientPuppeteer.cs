using System;
using System.Threading;
using lib;

namespace client{

	//interface for the Puppet Master
    public class ClientPuppeteer : MarshalByRefObject, IClientPuppeteer{

        //Client interface to access data
        public Client client;

        //constructor
        public ClientPuppeteer(Client client){
            this.client = client;
        }

        //simple ping
        public string ping(){
            return "Client "+client.username+" is online";
        }

        //wait x miliseconds
        public void wait(int x){
            Thread.Sleep(x);
            return;
        }

        //print client status
        public void statusPuppeteer(){
            client.status();
        }

        
        public void kill(){
            Environment.Exit(0);
        }

    }
}