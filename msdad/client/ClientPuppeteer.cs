using System;
using System.Threading;
using lib;

namespace client{

	//interface for the Puppet Master
    public class ClientPuppeteer : MarshalByRefObject, IClientPuppeteer{

        //Client interface to access data
        public Client client;

        //constructor
        public ClientPuppeteer(Client client){
            this.client = client;
        }

        //simple ping
        public bool ping(){
            return true;
        }

        public MeetingProposal createMeeting(string[] args){
            return client.create(args);
        }

        public MeetingProposal join(string[] args){
            return client.join(args);
        }

        public MeetingProposal close(string meeting_topic){
            return client.close(meeting_topic);
        }

        public MeetingProposal getMeeting(string topic){
            if(client.meetingList.ContainsKey(topic)){
                return client.meetingList[topic];
            } else {
                throw new ClientException("client does not have meeting");
            }
        }

        //wait x miliseconds
        public void wait(int x){
            Thread.Sleep(x);
            return;
        }

        //print client status
        public void statusPuppeteer(){
            client.status();
        }

        
        public void kill(){
            Environment.Exit(0);
        }

    }
}