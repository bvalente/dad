using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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

        //returns all meetings
        Dictionary<string,MeetingProposal> getMeetings();

        //join client
        void joinClient(ClientInfo client, string meeting_topic, List<Slot> slotList);

        //closes meeting
        void closeMeeting(string meeting_topic, ClientInfo clientInfo);

    }

    public interface IServerToServer{

        //send this server info to other servers
        void addNewServer(ServerInfo server);

        //send meeting to other servers
        void addMeeting(MeetingProposal meeting);
    }

    public interface IServerPuppeteer{
        
        // simple ping to check server status
        string ping();

        //print server status
        void statusPuppeteer();

        //add room
        void addRoom(string location_name, int capacity, string room_name);

        //populate with locations and servers
        void populate(Dictionary<string,Location> locationList, 
            Dictionary<string, ServerInfo> serverList);

        //freeze server
        void freeze();

        //unfreeze server
        void unfreeze();

        //crash server
        void kill();
    }

    public class ServerException : Exception{
        public ServerException(){
        }

        public ServerException(string message)
            : base(message){
        }

        public ServerException(string message, Exception inner)
            : base(message, inner){
        }

        public ServerException(SerializationInfo info, StreamingContext context)
            : base(info, context){
        }
    }
}