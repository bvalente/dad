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
        public Dictionary<string,Location> locationList;

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
            locationList = new Dictionary<string, Location>();
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

            //see if locations are valid
            foreach(Slot slot in meeting.slotList){
                if (locationList[slot.location] == null){
                    throw new MeetingException("location " + slot.location + " does not exist");
                }
            }

            //if clientList is empty, send to all clients
            List<ClientInfo> senders = new List<ClientInfo>();
            if(meeting.invitees == null){
                senders = new List<ClientInfo>( clientList.Values);
            } else{
                foreach(String invitee in meeting.invitees){
                    senders.Add(clientList[invitee]);
                }
            }

            //send to every client except the coordinator
            foreach(ClientInfo clientInfo in senders){
                if(clientInfo.username != meeting.coordinator){ //jumps cordinator and only sends to other clients
                    IClient client = (IClient) Activator.GetObject(
                        typeof(IClient),
                        clientInfo.client_url);
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
                throw new ClientException("user " + clientInfo.username + " already exitsts");
            }

            //add client
            clientList.Add(clientInfo.username,clientInfo);

            //TODO send client info to other servers
        }

        public void addRoom(string location_name, int capacity, string room_name){
            //look for location
            Location location = locationList[location_name];
            if (location == null){
                location = new Location(location_name);
                locationList.Add(location_name, location);
            }
            
            //create room
            Room room = location.addRoom(room_name, capacity);
            //Debug
            Console.WriteLine("added " + room.room_name);
        }

        public List<MeetingProposal> getMeetings(){
            return this.meetingList;
        }

        public void status(){
            //each server prints it's own server_id
            Console.WriteLine(server_id);
            
            //print freeze status
            if(freeze == true){
                Console.WriteLine("Server is frozen"); //let it go
            } else {
                Console.WriteLine("Server is not frozen");
            }

            //print clients
            foreach(KeyValuePair<string, ClientInfo> pair in clientList){
                Console.WriteLine(pair.Key);
            }
            //print rooms
            foreach(KeyValuePair<string,Location> pair in locationList){
                Console.WriteLine(pair.Key);
                foreach(Room room in pair.Value.roomList){
                    Console.WriteLine("\t" + room.room_name);
                    foreach(string date in room.usedDates){
                        Console.WriteLine("\t\t" + date);
                    }
                }
            }
            //print meetings
            foreach(MeetingProposal meeting in meetingList){
                Console.WriteLine(meeting);
            }
        }

        public void joinClient(ClientInfo client, string meeting_topic, List<Slot> slotList){
            //procurar meeting list, ver se existe
            MeetingProposal meetingProposal = null;
            foreach(MeetingProposal meeting in meetingList){
                if(meeting.topic == meeting_topic){
                    //encontramos
                    meetingProposal = meeting;
                    break;
                }
            }
            
            //ver se esta aberta
            if(meetingProposal.open == false){
                throw new MeetingException("Meeting is closed");
            }

        }

        public void closeMeeting(string meeting_topic, ClientInfo clientInfo){

            //look for meeting, check if it is fine
            //check if client is the coordinator
            //check if there is available room at x date
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
        
        public void statusPuppeteer(){
            server.status();
        }

        public void populate(Dictionary<string, Location> locationList){
            server.locationList = locationList;
        }

        public void addRoom(string location_name, int capacity, string room_name){
            server.addRoom(location_name, capacity, room_name);
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