using System;
using lib;

namespace server{
		//interface for server to other servers
	    public class ServerToServer : MarshalByRefObject, IServerToServer {

        Server server;

        public ServerToServer(Server server){
            this.server = server;
        }

        public void addNewServer(ServerInfo serverInfo){
            server.addNewServer(serverInfo);
        }

        public void addMeeting(MeetingProposal meeting){
            server.addMeeting(meeting);
            Console.WriteLine("meeting %s added", meeting.topic);
        }
    }
}