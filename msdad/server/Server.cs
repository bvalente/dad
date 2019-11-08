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
        Dictionary<string,MeetingProposal> meetingList;

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
			meetingList = new Dictionary<string,MeetingProposal>();
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
                if (! locationList.ContainsKey(slot.location)){
                    throw new MeetingException("location " + slot.location + " does not exist");
                }
            }

            //if clientList is empty, send to all clients
            List<ClientInfo> senders = new List<ClientInfo>();
            if(meeting.invitees.Count == 0){
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
            meetingList.Add(meeting.topic,meeting);

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
            Location location;
            if(locationList.ContainsKey(location_name)){
                location = locationList[location_name];
            }    
            else {
                location = new Location(location_name);
                locationList.Add(location_name, location);
                //Debug
                Console.WriteLine("Location added " + location_name);
            }
            
            //create room
            Room room = location.addRoom(room_name, capacity);
            //Debug
            Console.WriteLine("Room added " + room.room_name);
            
        }

        public Dictionary<string,MeetingProposal> getMeetings(){
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
            foreach(KeyValuePair<string, MeetingProposal> key in meetingList){
                Console.WriteLine(key.Value);
            }
        }

        public void joinClient(ClientInfo client, string meeting_topic, List<Slot> slotList){
            //slotList e a lista de disponibilidade do cliente
            
            //procurar meeting list, ver se existe
            if( ! meetingList.ContainsKey(meeting_topic)){
                throw new MeetingException("meeting does not exist");
            }
            MeetingProposal meeting = meetingList[meeting_topic];

            
            //ver se esta aberta
            if(meeting.open == false){
                throw new MeetingException("Meeting is closed");
            }
            
            //see if the client is invited
            if(meeting.invitees.Count == 0 ||
                meeting.invitees.Contains(client.username)){
                //client can join
                meeting.participants.Add(new Participant(client, slotList));
            } else {
                
                throw new MeetingException("client " + client.username + 
                        " can not participate in " + meeting_topic);
                
            }
        }

        //book a meeting
        public void closeMeeting(string meeting_topic, ClientInfo clientInfo){
            //look for meeting, check if it is fine
            MeetingProposal meeting = meetingList[meeting_topic];
            if(meeting.open == false){
               throw new MeetingException("Meeting is already closed");
            }
            //check if client is the coordinator
            if(clientInfo.username != meetingList[meeting_topic].coordinator){
                throw new MeetingException("This client can't close the meeting");
            }
            //ver se tem numero de participantes necessario
            if(meeting.participants.Count < meeting.minParticipants){
                throw new MeetingException("Not enough participants");
            }

            //check if there is available room at x date
            //ainda nao tem room
            //procurar uma room na location certa, ver se tem espaco
            //for participant, 
            List<Participant> participants = meeting.participants;
            List<Participant> participant_recursive = new List<Participant>(participants);
            participant_recursive.RemoveAt(0); //remove first element
            List<Slot> possibleSlots = new List<Slot>();
            foreach(Slot slot in participants[0].slotList){
                Slot slot2 = findSlot(participant_recursive, slot);
                if (slot2!= null){
                    possibleSlots.Add(slot2);
                }
            }
            //possible slot tem todas as slots possiveis
            if(possibleSlots.Count == 0){
                throw new MeetingException("Nao ha slot possivel");
            }
            //depois de encontrar os slots em comum

            //temos de encontrar uma sala com espaco
            //TODO:
            Room room = null;
            string date = null;
            foreach(Slot slot in possibleSlots){
                if( ! locationList.ContainsKey(slot.location)){
                    throw new MeetingException("location " + slot.location + " does not exist");
                }

                Location location = locationList[slot.location];
                string date2 = slot.date;
                //procurar room na location com date disponivel
                foreach(Room room2 in location.roomList){
                    if ( ! room2.usedDates.Contains(date2) &&
                            room2.capacity >= meeting.participants.Count){
                        room = room2;
                        date = date2;
                        break;
                    }
                }

                //se ja encontramos room, sai do loop
                if(room != null) break;
            }

            //ver se existe sala com espaco e data disponivel
            if(room == null){
                throw new MeetingException("No available rooms");
            }

            //if everythinh ok, books the meeting
            meeting.close(room, date);
        }

        private Slot findSlot(List<Participant> participants, Slot slot){
            
            if (participants.Count == 0){
                return slot;
            } else if(participants[0].slotList.Contains(slot)){
                List<Participant> participant_recursive = new List<Participant>(participants);
                participant_recursive.RemoveAt(0); //remove first element
                return findSlot(participant_recursive,slot);
            } else {
                return null;
            }
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