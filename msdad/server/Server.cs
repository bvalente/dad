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
        public string server_id;
        public string url;
        public int max_faults;
        public int min_delay;
        public int max_delay;

        //indicates if server is frozen, let it go 
        public bool freeze = false;

        //pending actions
        List<Action> actionList;

        //client database
        Dictionary<string, ClientInfo> clientList;

        //meetings database
        List<MeetingProposal> meetingList;

        //room list
        List<Room> roomList;

        //constructor
        public Server(string server_id, string url, int max_faults, int min_delay, int max_delay){

            this.server_id = server_id;
            this.url = url;
            this.max_faults = max_faults;
            this.min_delay = min_delay;
            this.max_delay = max_delay;

            clientList = new Dictionary<string, ClientInfo>();
			meetingList = new List<MeetingProposal>();
            actionList = new List<Action>();
            roomList = new List<Room>();
            //TODO load client/meetings database
        }

        public string ping(){
            return "server is online";
        }

        public void createMeeting(MeetingProposal meeting){
            //REMINDER coordinator is in the meeting class
            
            //check if server is frozen
            if(this.freeze == true){
                Action action = new Action(() => this.createMeeting(meeting));
                actionList.Add(action);
                return;
            }

            //TODO: verify if the meeting is valid!
            //if : sala livre, hora pretendida, capacidade da sala 

            //send to every client except the coordinator
            foreach(KeyValuePair<string, ClientInfo> pair in clientList){
                //send
                if(pair.Key != meeting.coordinator){ //jumps cordinator and only sends to other clients
                    IClient client = (IClient) Activator.GetObject(
                        typeof(IClient),
                        pair.Value.client_url);
                    client.sendMeeting(meeting);
                }
            }

            //add meeting to list
            meetingList.Add(meeting);

        }

        public void addClient(ClientInfo clientInfo){
            //check if server is frozen
            if(this.freeze == true){
                Action action = new Action( () => this.addClient(clientInfo));
                actionList.Add(action);
                return;
            }
            //check if there is a client with the same name
            if (clientList.ContainsKey(clientInfo.username)){
                //TODO throw exception
                return;
            }

            //add client
            clientList.Add(clientInfo.username,clientInfo);

            //send client info to other servers?
        }

        public void addRoom(Room room){
            roomList.Add(room);
            //Debug
            Console.WriteLine("added " + room.room_name);
        }

        public void executeActionList(){

            //foreach
            foreach(Action action in actionList){
                action();
            }
            actionList.Clear();
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
        
        public string status(){
            return null;
            //TODO: voltar aqui 
        }

        public void addRoom(Room room){
            server.addRoom(room);
        }

        public void kill(){
            Console.WriteLine("Bye bye");
            Environment.Exit(0);
        }

        public void freeze(){
            server.freeze = true;
        }

        public void unfreeze(){
            server.freeze = false;
            server.executeActionList();
        }

    }
}