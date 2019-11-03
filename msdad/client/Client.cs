using System;
using lib;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

namespace client{
	
    //interface for the server
    public class Client : MarshalByRefObject, IClient{

        public string Name;
        public string Port;
        private IServer server;
                
        //meetings database
        List<MeetingProposal> meetingList;
        
        //constructor
        public Client(string name, string port){

            this.Name = name;
            this.Port = port;
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

        }

        //simple ping 
        public string ping(){
            return "client is online";
        }

        //receive meeting from server and save it
        public void sendMeeting(MeetingProposal meeting){
            meetingList.Add(meeting);
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
            return "Client "+client.Name+" is online";
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