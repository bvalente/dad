using System;
using lib;
using System.Collections.Generic;

namespace server
{
    class Program
    {
        //args - port
        static void Main(string[] args)
        {
            string port; //cuidado!
            if(args.Length == 0){
                port = "8090";
            } else {
                port = args[0];
            }

            //create tcp channel
            TcpChannel channel = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(channel, false); 


            //create server
            Server server = new Server(port);
            RemotingServices.Marshal(
                server,
                "Server",
                typeof(Server)); 
            
            Console.WriteLine("New server created");
            System.Console.ReadLine();
        }
    }
}

namespace lib{

    class Server : MarshalByRefObject, IServer{

        public string port;

        public Server(string port){
            this.port = port;
        }

        //client database
        Dictionary<string, IClient> clientList;

        //meetings database
        List<MeetingProposal> meetingList;



        Server(){
            clientList = new Dictionary<string, IClient>();
            //TODO load client/meetings database

            //create TCP channel
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

            meetingList.add(meeting);

        }

        public void addClient(ClientInfo clientInfo){
            //check if there is a client with the same name
            if (clientList.ContainsKey(client.getName())){
                //TODO throw exception
                return;
            }

            //add client
            IClient client = (IClient) Activator.GetObject(
                        typeof(IClient),
                        clientInfo.getUrl() + ':' + clientInfo.getPort() + "/Client");
            clientList.add(clientInfo.getName(),client);

            //send client info to other servers?
        }
    }
}