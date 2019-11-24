using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.IO;
using lib;
using Serilog;

namespace client{
	
    //interface for the server
    public class Client : MarshalByRefObject, IClient{

        public string username;
        public string client_url;
        public string server_url;
        public string script_file;
                
        //meetings database
        Dictionary<string,MeetingProposal> meetingList;
        
        //constructor
        public Client(string username, string client_url, string server_url, string script_file){

            this.username = username;
            this.client_url = client_url;
            this.server_url = server_url;
            this.script_file = script_file;
			meetingList = new Dictionary<string, MeetingProposal>();
            
            IServer server = (IServer) Activator.GetObject(
                typeof(IServer), 
                server_url);
            server.addClient(this.GetInfo());
            //get meetings
            ListDelegate del = new ListDelegate(server.getMeetings);
            del.BeginInvoke(updateCallback,null);
            
            //execute script in new thread
            Thread thread = new Thread(new ThreadStart(() => this.executeScript(script_file)));
            thread.Start();

        }

        //Implementation of IClient interface methods

        //IClient.ping
        //simple ping 
        public string ping(){
            return "client is online";
        }

        //IClient.sendMeeting
        //receive meeting from server and save it
        public void sendMeeting(MeetingProposal meeting){
            
            this.addMeeting(meeting);
        }

        //End of IClient implementation

        //Script methods

        //execute script file
        void executeScript(string fileName){

            //load file and get commands
            string cPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(cPath,
                 "../../../../scripts/" + fileName);
            string[] commands = System.IO.File.ReadAllLines(filePath);

            //execute puppetMaster.executeCommand n times
            foreach (string command in commands){
                executeCommand(command);
            }
        }

        //execute one command
        public void executeCommand(string command){
            //receives a command and executes respective function
            string[] cmds = command.Split(' ');

            try{
                //switch, execute different command for each cmds[0]
                switch(cmds[0]){

                    case "list":
                        list();
                        break;
                    case "create":
                        create(cmds);
                        break;
                    case "join":
                        join(cmds);
                        break;
                    case "close":
                        close(cmds[1]);
                        break;
                    case "wait":
                        wait(cmds[1]);
                        break;
                    default:
                        Log.Error("invalid command: unrecognized " + cmds[0]);
                        break;
                }
            }catch(IndexOutOfRangeException ex){
                Log.Error(ex, "invalid command: not enough arguments");
            }
        }

        //Lists all available meetings
        void list(){
            //print meetings
            lock(meetingList){
                foreach(KeyValuePair<string, MeetingProposal> key in meetingList){
                    Log.Information(key.Value.ToString());
                }
            }
            //async ask for update
            IServer server = (IServer) Activator.GetObject(
                typeof(IServer),
                server_url);
            ListDelegate del = new ListDelegate(server.getMeetings);
            del.BeginInvoke(updateCallback,null);
        }
        
        //Creates a new meeting
        void create(string[] args){
            string meeting_topic = args[1];
            int min_attendees = Int32.Parse(args[2]);
            int number_of_slots = Int32.Parse(args[3]);
            int number_of_invitees = Int32.Parse(args[4]);
            List<Slot> slotList = new List<Slot>();
            List<string> invitees = new List<string>();
            
            //Parse to get all slots and invitees
            int n=5;
            for(int i=0; i<number_of_slots; i++){
                string[] split = args[n].Split(',');
                slotList.Add( new Slot(split[0], split[1]) );
                n++;
            }
            for (int i=0; i<number_of_invitees; i++){
                invitees.Add(args[n]);
                n++;
            }

            //create meeting
            MeetingProposal meeting = new MeetingProposal(this.username,
                meeting_topic, min_attendees, slotList, invitees);
            
            //get server
            IServer server = (IServer) Activator.GetObject(
                typeof(IServer), 
                server_url); 
            
            //try to create meeting
            try{
                server.createMeeting(meeting);
            } catch(MeetingException ex){
                Log.Error(ex, "cannot create meeting");
                return;
            }catch(Exception ex){
                Log.Error(ex, "connection with server failed");
                return;
            }

            meetingList.Add(meeting.topic, meeting);

  
        }

        //Joins an existing meeting
        void join(string[] args){
            string meeting_topic = args[1];
            int number_of_slots = Int32.Parse(args[2]);
            //parse slots
            List<Slot> slotList = new List<Slot>();
            for(int i = 3; i < number_of_slots + 3;i++){
                string[] str = args[i].Split(',');
                Slot slot = new Slot(str[0],str[1]);
                slotList.Add(slot);
            }

            //get server
            IServer server = (IServer) Activator.GetObject(
                typeof(IServer),
                server_url);

            //try to join meeting
            try{
                MeetingProposal meeting = server.joinClient(this.GetInfo(), meeting_topic, slotList);
                this.addMeeting(meeting);
            }catch(MeetingException ex){
                Log.Error(ex, "cannot join client");
            }
        }

        //Closes a meeting
        void close(string meeting_topic){
            IServer server = (IServer) Activator.GetObject(
                typeof(IServer),
                server_url);
            //try to close meeting
            try{
                MeetingProposal meeting = server.closeMeeting(meeting_topic, this.GetInfo());
                this.addMeeting(meeting);
            } catch(MeetingException ex){
                Log.Error(ex, "cannot close meeting");
            }
            
        }

        //Delays the execution of the next command for x milliseconds
        void wait(string time){
            int time_int = Int32.Parse(time);
            Thread.Sleep(time_int);
        }

        //End of script mehtods

        //Other helpful methods

        ClientInfo GetInfo(){
            return new ClientInfo(username, client_url, server_url, script_file);
        }

        //add meeting to this client
        private void addMeeting(MeetingProposal meeting){
            lock(meetingList){
                if(meetingList.ContainsKey(meeting.topic)){
                    //replace meeting
                    meetingList[meeting.topic] = meeting;
                } else {
                    //add new dictionary entry
                    meetingList.Add(meeting.topic, meeting);
                }
            }
        }
        
        //ClientPuppeteer statusPuppeteer
        public void status(){
            //print name
            Log.Information("username: {username}",username);
            //meetinglist
            foreach(KeyValuePair<string, MeetingProposal> key in meetingList){
                Log.Information(key.Value.ToString());
            }
        }

        //update meetings async
        //delegate
        delegate Dictionary<string,MeetingProposal> ListDelegate();
        //callback
        void updateCallback(IAsyncResult ar){
            //will be called when delegate is over
            ListDelegate del = (ListDelegate)((AsyncResult)ar).AsyncDelegate;
            lock(meetingList){
                //TODO update instead of replace
                meetingList.Clear();
                meetingList = del.EndInvoke(ar);
            }
        }
    }
}