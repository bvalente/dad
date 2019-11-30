using System;
using lib;
using System.Collections.Generic;
using System.Threading;
using Serilog;

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

        //save other servers
        Dictionary<string, ServerInfo> serverList;

        //meetings database
        public Dictionary<string,MeetingProposal> meetingList;
        public Dictionary<string,MeetingProposal> blockedMeetings;
        AutoResetEvent meetingLock;

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
            serverList = new Dictionary<string, ServerInfo>();
			meetingList = new Dictionary<string,MeetingProposal>();
            blockedMeetings = new Dictionary<string, MeetingProposal>();
            actionList = new List<Action>();
            locationList = new Dictionary<string, Location>();
            meetingLock = new AutoResetEvent(false);
            //TODO load client/meetings database ?

        }

        //Implementation of IServer interface methods        

        //IServer.ping
        public string ping(){
            return "server is online";
        }

        //IServer.createMeeting
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
            //remove coordinator
            //TODO lock?
            Dictionary<string, ClientInfo> newdic = new Dictionary<string, ClientInfo>(clientList);
            newdic.Remove(meeting.coordinator);
            if(meeting.invitees.Count == 0){
                senders = new List<ClientInfo>( newdic.Values);
            } else{
                foreach(String invitee in meeting.invitees){
                    //only send meeting if the server knows the invitee
                    if(newdic.ContainsKey(invitee)){
                        senders.Add(newdic[invitee]);
                    }
                }
            }

            //Async send meeting info to clients
            UpdateClientsDelegate clientDel = new UpdateClientsDelegate(updateClients);
            clientDel.BeginInvoke(meeting, senders, null, null);

            //add meeting to list
            lock(meetingList){
                meetingList.Add(meeting.topic,meeting);
            }

            //Async send meeting to other servers
            UpdateServersDelegate serverDel = new UpdateServersDelegate(this.updateServers);
            serverDel.BeginInvoke(meeting,null,null);
            //TODO callback function?

        }

        //IServer.addClient
        public Dictionary<string, ServerInfo> addClient(ClientInfo clientInfo){
            //check if server is frozen
            if(this.freeze == true){
                Action action = new Action( () => this.addClient(clientInfo));
                actionList.Add(action);
                return serverList;
            }

            //check if there is a client with the same name
            if (clientList.ContainsKey(clientInfo.username)){
                throw new ClientException("user " + clientInfo.username + " already exitsts");
            }

            //add client
            clientList.Add(clientInfo.username,clientInfo);

            //client is responsible for fetching meetings

            return serverList;
        }

        //IServer.getMeetings
        public Dictionary<string,MeetingProposal> getMeetings(){
            return this.meetingList;
        }

        //IServer.joinClient
        public MeetingProposal joinClient(ClientInfo client, string meeting_topic, List<Slot> slotList){
            //slotList e a lista de disponibilidade do cliente
            
            //procurar meeting list, ver se existe
            MeetingProposal meeting;
            lock(meetingList){
                if( ! meetingList.ContainsKey(meeting_topic)){
                    throw new MeetingException("meeting does not exist");
                }
                meeting = meetingList[meeting_topic];
            }

            
            //ver se esta aberta
            if(meeting.open == false){
                throw new MeetingException("Meeting is closed");
            }
            
            //see if the client is invited
            if(meeting.invitees.Count == 0 ||
                    meeting.invitees.Contains(client.username)){
                //client can join
                meeting.join(new Participant(client, slotList));
            } else {
                
                throw new MeetingException("client " + client.username + 
                        " can not participate in " + meeting_topic);
            }

            //Async send meeting to other servers
            UpdateServersDelegate serverDel = new UpdateServersDelegate(this.updateServers);
            serverDel.BeginInvoke(meeting,null,null);
            //TODO callback function?

            //Async send to other clients
            Dictionary<string, ClientInfo> newdic = new Dictionary<string, ClientInfo>(clientList);
            newdic.Remove(client.username);
            UpdateClientsDelegate clientDel = new UpdateClientsDelegate(this.updateClients);
            clientDel.BeginInvoke(meeting,new List<ClientInfo>(newdic.Values), null, null);

            return meeting;
        }

        //IServer.closeMeeting
        public MeetingProposal closeMeeting(string meeting_topic, ClientInfo clientInfo){

            //procurar meeting list, ver se existe
            MeetingProposal meeting;
            lock(meetingList){
                if( ! meetingList.ContainsKey(meeting_topic)){
                    throw new MeetingException("meeting does not exist");
                }
                meeting = meetingList[meeting_topic];
            }

            //check if meeting is already closed
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
                //TODO cancel meeting
            }

            //check if there is available room at x date
            //procurar uma room na location certa, ver se tem espaco
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

            //depois de encontrar os slots em comum temos de encontrar uma sala com espaco
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

            //Async send meeting to other servers
            UpdateServersDelegate serverDel = new UpdateServersDelegate(this.updateServers);
            serverDel.BeginInvoke(meeting,null,null);
            //TODO callback function?

            //TODO update locations to servers beacause of used Dates in rooms

            //TODO send only to invited clients??
            //Async send to other clients
            Dictionary<string, ClientInfo> newdic = new Dictionary<string, ClientInfo>(clientList);
            newdic.Remove(clientInfo.username);
            UpdateClientsDelegate clientDel = new UpdateClientsDelegate(this.updateClients);
            clientDel.BeginInvoke(meeting,new List<ClientInfo>(newdic.Values), null, null);

            return meeting;

        }

        //End of IServer implementation

        //Methods for other interfaces

        //ServerPuppeteer statusPuppeteer
        public void status(){
            //each server prints it's own server_id
            Log.Information("Server id: " + server_id);
            
            //print freeze status
            if(freeze == true){
                Log.Information("Server is frozen"); //let it go
            } else {
                Log.Information("Server is not frozen");
            }

            //print clients
            foreach(KeyValuePair<string, ClientInfo> pair in clientList){
                Log.Information(pair.Key);
            }
            //print rooms
            foreach(KeyValuePair<string,Location> pair in locationList){
                Log.Information(pair.Key);
                foreach(Room room in pair.Value.roomList){
                    Log.Information("\t" + room.room_name);
                    foreach(string date in room.usedDates){
                        Log.Information("\t\t" + date);
                    }
                }
            }
            lock(meetingList){
                foreach(KeyValuePair<string, MeetingProposal> key in meetingList){
                    Log.Information(key.Value.ToString());
                }
            }
        }

        //ServerPuppeteer populate
        public void addOldServers( Dictionary<string, ServerInfo> serverList){

            ServerInfo me = this.GetInfo();
            foreach(KeyValuePair<string, ServerInfo> pair in serverList){
                //save server
                this.serverList.Add(pair.Key, pair.Value);
                //connect to server and send my info
                IServerToServer server = (ServerToServer) Activator.GetObject(
                    typeof(IServerToServer),
                    pair.Value.url_to_server);
                server.addNewServer(me);
            }
        }
        
        //ServerPuppeteer addRoom
        public void addRoom(string location_name, int capacity, string room_name){
            //look for location
            Location location;
            if(locationList.ContainsKey(location_name)){
                location = locationList[location_name];
            }    
            else {
                location = new Location(location_name);
                locationList.Add(location_name, location);
                Log.Debug("Location added " + location_name);
            }
            
            //create room
            Room room = location.addRoom(room_name, capacity);
            Log.Debug("Room added " + room.room_name);
            
        }

        //ServerPuppeteer unfreeze
        public void executeActionList(){
            //Async execute actions after unfreeze
            foreach(Action action in actionList){
                action();
            }
            actionList.Clear();
        }

        //ServerToServer addNewServer
        public void addNewServer(ServerInfo serverInfo){
            serverList.Add(serverInfo.server_id,serverInfo);
        }

        //ServerToServer addMeeting
        public void addMeeting(MeetingProposal meeting){
            //if already exists, replace
            lock(meetingList){
                if(meetingList.ContainsKey(meeting.topic)){
                    meetingList.Remove(meeting.topic);
                }
                meetingList.Add(meeting.topic,meeting);
            }

            //Async send meeting info to clients
            UpdateClientsDelegate del = new UpdateClientsDelegate(updateClients);
            del.BeginInvoke(meeting, new List<ClientInfo>(clientList.Values), null, null);
        }

        //End of other interfaces methods

        //Other helpful methods

        //recursive function to search fot available slots
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

        //generate ServerInfo
        public ServerInfo GetInfo(){
            return new ServerInfo(server_id,url,max_faults.ToString()
                ,min_delay.ToString(),max_delay.ToString());
        }

        //Async send meeting to server
        delegate void SendMeetingDelegate(ServerInfo serverInfo, MeetingProposal meeting, ref int consensus);

        void sendMeeting(ServerInfo serverInfo, MeetingProposal meeting, ref int consensus){

            IServerToServer server = (ServerToServer) Activator.GetObject(
                typeof(ServerInfo),
                serverInfo.url_to_server);
            bool join = server.sendMeeting(meeting);
            if (join){
                if( Interlocked.Decrement(ref consensus) == 0){
                    meetingLock.Set();
                }
            }
        }

        //Async write the meeting
        delegate void WriteMeetingDelegate(ServerInfo serverInfo, MeetingProposal meeting);

        void writeMeeting(ServerInfo serverInfo, MeetingProposal meeting){

            IServerToServer server = (ServerToServer) Activator.GetObject(
                typeof(ServerInfo),
                serverInfo.url_to_server);
            server.writeMeeting(meeting);

        }

        delegate void DONTwriteMeetingDelegate(ServerInfo serverInfo, MeetingProposal meeting);

        void DONTwriteMeeting(ServerInfo serverInfo, MeetingProposal meeting){
            IServerToServer server = (ServerToServer) Activator.GetObject(
                typeof(ServerInfo),
                serverInfo.url_to_server);
            server.DONTwriteMeeting(meeting);
            
        }


        //Async update meeting to other servers
        delegate void UpdateServersDelegate(MeetingProposal meeting);

        void updateServers(MeetingProposal meeting){
            //FIXME sleep, wait for services
            Thread.Sleep(200);
            //update other servers
            foreach(KeyValuePair<string, ServerInfo> pair in serverList){
                IServerToServer server = (IServerToServer) Activator.GetObject(
                    typeof(IServerToServer),
                    pair.Value.url_to_server);
                server.addMeeting(meeting);
            }
        }

        //Async send meeting to client
        delegate void UpdateClientsDelegate(MeetingProposal meeting, List<ClientInfo> clients);

        void updateClients(MeetingProposal meeting, List<ClientInfo> clients){
            //TODO send only to invited clients??
            foreach(ClientInfo clientInfo in clients){
                IClient client = (IClient) Activator.GetObject(
                    typeof(IClient),
                    clientInfo.client_url);
                client.sendMeeting(meeting);
            }
        }
        

    }
}