using System;
using lib;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using System.Diagnostics;

namespace client
{
    class Program
    {
        //args - name, port?
        static void Main(string[] args)
        {

            string name = args[0];
            string port;

             if(args.Length < 2){
                port = "8080";
            } else {
                port = args[1];
            }
          
            
            //create tcp channel
            TcpChannel channel = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(channel, false); 


            //create client
            Client client = new Client(name, port);
            RemotingServices.Marshal(
                client,
                "Client",
                typeof(Client));
                      
            
            
            Console.WriteLine("Client " + name +" created");
            Console.WriteLine("PID: " +
                Process.GetCurrentProcess().Id.ToString());
            System.Console.ReadLine();
        }
    }
}

namespace lib
{
    class Client : MarshalByRefObject, IClient{
        public string name;
        public string port;

        private IServer server;
                
        //meeting database
        List<MeetingProposal> meetingList;
        
        //constructor
        public Client(string name, string port){
            this.name = name;
            this.port = port;

            /*TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel,false);
            */
            server = (IServer) Activator.GetObject(typeof(IServer), "tcp://localhost:8090/Server"); //TODO: fazer no caso em que ha varias maquinas
            
            if (server == null){
            System.Console.WriteLine("Could not locate server");
            }

            Console.WriteLine("hi OwO");
        }

        public string ping(){
            return "client is online";
        }

        public string list(){
            return "";
        }

        public void create(string meeting_topic, int min_attendees, int number_of_slots, int number_of_invitees,
         List<Slot> list_of_slots, List<string> list_of_invitees){
             //TODO
            //criar MeetingProposal
            //enviar para o servidor
            //MeetingProposal m = new MeetingProposal(this.name, meeting_topic, min_attendees, list_of_slots, list_of_invitees);
            
            //server.createMeeting(m);
        }

        //Joins an existing meeting
        public void join(MeetingProposal meeting){}

        public void close(MeetingProposal meeting){}

        public void wait(int x){}
        public void sendMeeting(MeetingProposal meeting){
            //reveive meeting from server and save it
            meetingList.Add(meeting);
        }

    }
}