using System;

namespace lib{

    // class with important server info
    // TODO discuss serverInfo important values
    [Serializable]
    public class ServerInfo{

        public string server_id;
        public string url;
        public string max_faults;
        public string min_delay;
        public string max_delay;

        public ServerInfo(string server_id, string url, string max_faults, string min_delay, string max_delay){
            this.server_id = server_id;
            this.url = url;
            this.max_faults = max_faults;
            this.min_delay = min_delay;
            this.max_delay = max_delay;
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

        //return server status
        string stauts();

        //crash server
        void kill();
    }
}