using System;
using System.Runtime.Serialization;

namespace lib{

    // class with important client info
    [Serializable]
    public class ClientInfo {

        public static string puppeteerExtension = "Puppeteer";

        public string username;
        public string client_url;
        public string client_url_puppeteer;
        public string server_url;
        public string script_file;

        public ClientInfo(string username, string client_url, string server_url, string script_file){
            this.username = username;
            this.client_url = client_url;
            this.client_url_puppeteer = client_url + puppeteerExtension;
            this.server_url = server_url;
            this.script_file = script_file;
        }
    }

    //interface for the servers
    public interface IClient{

        //Simple ping to check client status
        bool ping();

        //get meeting from server
        void sendMeeting(MeetingProposal meeting);
    }

    //interface for the Puppet Master
    public interface IClientPuppeteer{

        //simple ping to check client status
        bool ping();

        //create meeting
        MeetingProposal createMeeting(string[] args);

        //join meeting
        MeetingProposal join(string[] args);

        //close meeting
        MeetingProposal close(string topic);

        //get meeting
        MeetingProposal getMeeting(string topic);

        //get client's server url
        string getServer();

        //Delays the execution of the next command for x milliseconds
        void wait(int x);

        //print status
        void statusPuppeteer();

        //reset program
        void kill();

        void undo();
    }

    [Serializable]
    public class ClientException : Exception{
        public ClientException(){
        }

        public ClientException(string message)
            : base(message){
        }

        public ClientException(string message, Exception inner)
            : base(message, inner){
        }

        public ClientException(SerializationInfo info, StreamingContext context)
            : base(info, context){
        }
    }
}