using System;
using lib;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;

namespace server{
	
    //interface for the clients
    public class Server : MarshalByRefObject, IServer{

        //tcp port
        public string port;

        //client database
        Dictionary<string, IClient> clientList;

        //meetings database
        List<MeetingProposal> meetingList;

        //constructor
        public Server(string port){

            this.port = port;
            clientList = new Dictionary<string, IClient>();
			meetingList = new List<MeetingProposal>();
            //TODO load client/meetings database
        }

        public string ping(){
            return "server is online";
        }

        public void createMeeting(MeetingProposal meeting){
            //coordinator is in the meeting class
            //TODO verify if the meeting is valid

            Dictionary<string, IClient> senders;

            if(meeting.invitees == null){
                senders = clientList;
            } else {
                senders = new Dictionary<string, IClient>();
                foreach(string s in meeting.invitees){
                    senders.Add(s, clientList[s]);
                }
            }

            foreach(KeyValuePair<string, IClient> pair in clientList){//less coordinator
                //send
                if(pair.Key != meeting.coordinator){ //jumps cordinator and only sends to other clients
                    pair.Value.sendMeeting(meeting);
                }
            }

            meetingList.Add(meeting);

        }

        public void addClient(ClientInfo clientInfo){
            //check if there is a client with the same name
            if (clientList.ContainsKey(clientInfo.Name)){
                //TODO throw exception
                return;
            }

            //add client
            IClient client = (IClient) Activator.GetObject(
                        typeof(IClient),
                        clientInfo.Url + ':' + clientInfo.Port + "/Client");
            clientList.Add(clientInfo.Name,client);

            //send client info to other servers?
        }
    }

    //interface for the Puppet Master
    public class ServerPuppeteer : MarshalByRefObject, IServerPuppeteer{

        //save instance of server interface to access data
        public Server server;

        public ServerPuppeteer(Server server){
            this.server = server;
        }

        public string ping(){
            return "Server Puppeteer is online";
        }
    }
}