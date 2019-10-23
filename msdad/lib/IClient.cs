using System.Collections.Generic;

namespace lib
{
    // class with important client info
    public class ClientInfo{
        private string name;
        private string url;
        private string port;

        ClientInfo(string name, string url, string port){
            this.name = name;
            this.url = url;
            this.port = port;
        }
        public string getName(){
            return this.name;
        }
        public string getUrl(){
            return this.url;
        }
        public string getPort(){
            return this.port;
        }
    }

    public interface IClient{
        //Simple ping to check client status
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
        //get meeting from server
        void sendMeeting(MeetingProposal meeting); //sends meeting to everyone but cordinator 
    }
}