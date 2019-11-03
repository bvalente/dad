using System.Collections.Generic;
using System;

namespace lib{

    // class with important client info
    [Serializable]
    public class ClientInfo {

        public string Name;
        public string Url;
        public string Port;

        public ClientInfo(string name, string url, string port){
            this.Name = name;
            this.Url = url;
            this.Port = port;
        }
    }

    //interface for the servers
    public interface IClient{

        //Simple ping to check client status
        string ping();

        //get meeting from server
        void sendMeeting(MeetingProposal meeting);
    }

    //interface for the Puppet Master
    public interface IClientPuppeteer{

        //simple ping to check client status
        string ping();

        //Lists all available meetings
        string list();

        //Creates a new meeting
        void create(string meeting_topic, int min_attendees, int number_of_slots, int number_of_invitees,
            List<Slot> list_of_slots, List<string> list_of_invitees);

        //Joins an existing meeting
        void join(MeetingProposal meeting);

        //Closes a meeting
        void close(MeetingProposal meeting);

        //Delays the execution of the next command for x milliseconds
        void wait(int x);
    }
}