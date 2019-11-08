using System;
using lib;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;
using System.IO;

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
			//TODO import meetings data?
            
            IServer server = (IServer) Activator.GetObject(
                typeof(IServer), 
                server_url);
            server.addClient(this.GetInfo());
            executeScript(script_file);

        }

        //simple ping 
        public string ping(){
            return "client is online";
        }

        //receive meeting from server and save it
        public void sendMeeting(MeetingProposal meeting){
            meetingList.Add(meeting.topic,meeting);
        }

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
                        Console.WriteLine("invalid command: unrecognized " + cmds[0]);
                        break;
                }
            }catch(IndexOutOfRangeException ex){
                Console.WriteLine("invalid command: not enough arguments");
                Console.WriteLine(ex.Message);
            }
        }

        
        //Lists all available meetings
        void list(){
            //print meetings
            foreach(KeyValuePair<string, MeetingProposal> key in meetingList){
                Console.WriteLine(key.Value);
            }
            //async ask for update
            IServer server = (IServer) Activator.GetObject(
                typeof(IServer),
                server_url);
            ListDelegate del = new ListDelegate(server.getMeetings); //TODO replace ping
            del.BeginInvoke(listCallback,null);
        }
        //delegate
        delegate Dictionary<string,MeetingProposal> ListDelegate();
        //callback
        void listCallback(IAsyncResult ar){
            //will be called when delegate is over
            ListDelegate del = (ListDelegate)((AsyncResult)ar).AsyncDelegate;
            meetingList.Clear();
            meetingList = del.EndInvoke(ar);
            //TODO
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
            
            //what to do with the meeting?

            //guardar meeting


            //get server
            IServer server = (IServer) Activator.GetObject(
                typeof(IServer), 
                server_url); 
            
            //try to create meeting
            try{
                server.createMeeting(meeting);
            } catch(Exception ex){

                //TODO nossas excepcoes
                Console.WriteLine("connection with server failed");
				Console.WriteLine(ex.Message);
                return;
            }

            //pedir mais meetings?
            meetingList.Add(meeting.topic, meeting);
            

        }

        //Joins an existing meeting
        void join(string[] args){
            string meeting_topic = args[1];
            int number_of_slots = Int32.Parse(args[2]);
            List<Slot> slotList = new List<Slot>();
            for(int i = 3; i < number_of_slots + 3;i++){
                string[] str = args[i].Split(',');
                Slot slot = new Slot(str[0],str[1]);
                slotList.Add(slot);
            }

            IServer server = (IServer) Activator.GetObject(
                typeof(IServer),
                server_url);
            //TODO try catch
            try{
                server.joinClient(this.GetInfo(), meeting_topic, slotList);
            }catch(MeetingException ex){
                Console.WriteLine(ex.Message);
            }            

        }

        //Closes a meeting
        void close(string meeting_topic){
            IServer server = (IServer) Activator.GetObject(
                typeof(IServer),
                server_url);
            try{
                server.closeMeeting(meeting_topic, this.GetInfo());
            } catch(MeetingException ex){
                Console.WriteLine(ex.Message);
            }
            
        }

        //Delays the execution of the next command for x milliseconds
        void wait(string time){
            int time_int = Int32.Parse(time);
            Thread.Sleep(time_int);
        }

        ClientInfo GetInfo(){
            return new ClientInfo(username, client_url, server_url, script_file);
        }
        
        public void status(){
            //print name
            Console.WriteLine(username);
            //meetinglist
            foreach(KeyValuePair<string, MeetingProposal> key in meetingList){
                Console.WriteLine(key.Value);
            }
        }
    }

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

        public void statusPuppeteer(){
            client.status();
        }

    }
}