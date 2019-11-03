using System;

namespace lib{

    // class with important server info
    // TODO discuss serverInfo important values
    [Serializable]
    public class ServerInfo{

        public string Url;
        public string Port;

        public ServerInfo(string url, string port){
            this.Url = url;
            this.Port = port;
        }
    }

    public interface IServer{

        // simple ping to check server status
        string ping();

        // create meeting 
        void createMeeting(MeetingProposal meeting);

        //add client
        void addClient(ClientInfo client);
    }

    public interface IServerPuppeteer{
        
        // simple ping to check server status
        string ping();
    }
}