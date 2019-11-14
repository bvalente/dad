using System;
using System.Runtime.Serialization;

namespace lib{

    // class with important client info
    [Serializable]
    public class ClientInfo {

        public string username;
        public string client_url;
        public string server_url;
        public string script_file;

        public ClientInfo(string username, string client_url, string server_url, string script_file){
            this.username = username;
            this.client_url = client_url;
            this.server_url = server_url;
            this.script_file = script_file;
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

        //Delays the execution of the next command for x milliseconds
        void wait(int x);

        //print status
        void statusPuppeteer();
    }

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