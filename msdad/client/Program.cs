using System;
using lib;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;

namespace client
{
    class Program
    {
        //args - name, port?
        static void Main(string[] args)
        {
            if (args.Length != 2){
                //mandar exception
                return;
            }
            string name = args[0]; //cuidado!
            string port = args[1];
            
            
            //create tcp channel
            TcpChannel channel = new TcpChannel(Int32.Parse(port));
            ChannelServices.RegisterChannel(channel, false); 


            //create client
            Client client = new Client(name, port);
            RemotingServices.Marshal(
                client,
                "Client",
                typeof(Client));
                      
            
            
            Console.WriteLine("New client created");
            System.Console.ReadLine();
        }
    }
}

namespace lib
{
    class Client : MarshalByRefObject, IClient{
        public string name;
        public string port;
                
        //meeting database
        List<MeetingProposal> meetingList;
        
        //constructor
        public Client(string name, string port){
            this.name = name;
            this.port = port;
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