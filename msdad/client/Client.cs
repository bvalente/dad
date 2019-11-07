using System;
using lib;
using System.Collections.Generic;
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
        List<MeetingProposal> meetingList;
        
        //constructor
        public Client(string username, string client_url, string server_url, string script_file){

            this.username = username;
            this.client_url = client_url;
			meetingList = new List<MeetingProposal>();
			//TODO import meetings data

            //TODO IMPORTANT receive server ip and connect to that
            /*
            server = (IServer) Activator.GetObject(typeof(IServer), "tcp://localhost:8090/Server"); //TODO: fazer no caso em que ha varias maquinas
            
            try{
                Console.WriteLine(server.ping());
            } catch(Exception ex){
                Console.WriteLine("Could not locate server");
				Console.WriteLine(ex.Message);
            }
            */
            
            executeScript(script_file);

        }

        //simple ping 
        public string ping(){
            return "client is online";
        }

        //receive meeting from server and save it
        public void sendMeeting(MeetingProposal meeting){
            meetingList.Add(meeting);
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
                        join(cmds[1]);
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
            //TODO a lot of exceptions

        }

        //Joins an existing meeting
        void join(string meeting_topic){

        }

        //Closes a meeting
        void close(string meeting_topic){

        }

        //Delays the execution of the next command for x milliseconds
        void wait(string time){
            int time_int = Int32.Parse(time);
            Thread.Sleep(time_int);
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

        //list all available meetings
        public string list(){
            //TODO ask server for a database refresh
            return "";
        }

        //create meeting
        public void create(string meeting_topic, int min_attendees, int number_of_slots, int number_of_invitees,
         List<Slot> list_of_slots, List<string> list_of_invitees){
             //TODO
            //criar MeetingProposal
            //enviar para o servidor
            //MeetingProposal m = new MeetingProposal(this.name, meeting_topic, min_attendees, list_of_slots, list_of_invitees);
            
            //server.createMeeting(m);
            return;
        }

        //Joins an existing meeting
        public void join(MeetingProposal meeting){
            return;
        }

        //closes a meeting
        public void close(MeetingProposal meeting){
            return;
        }

        //wait x miliseconds
        public void wait(int x){
            Thread.Sleep(x);
            return;
        }
    }
}