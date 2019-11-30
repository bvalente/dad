using System;
using lib;
using Serilog;

namespace server{
		//interface for server to other servers
	    public class ServerToServer : MarshalByRefObject, IServerToServer {

        Server server;

        public ServerToServer(Server server){
            this.server = server;
        }

        //get meeting
        public bool sendMeeting(MeetingProposal meeting){
            //check version here?
            if (server.meetingList.ContainsKey(meeting.topic)){
                //server has meeting, check version and if meeting is blocked
                //FIXME > vs >=
                if(server.meetingList[meeting.topic].version >= meeting.version ||
                    server.blockedMeetings.ContainsKey(meeting.topic)){
                    //current meeting is newer or is blocked
                    return false;
                }
            }
            server.blockedMeetings.Add(meeting.topic, meeting);
            return true;
        }

        //write meeting
        public void writeMeeting(MeetingProposal meeting){
            server.meetingList.Remove(meeting.topic);
            server.meetingList.Add(meeting.topic, meeting);
            server.blockedMeetings.Remove(meeting.topic);
            //update location's room
            if(meeting.room != null){
                Location location = meeting.room.location;
                location.roomList.Remove(meeting.room);
                location.roomList.Add(meeting.room);
            }
        }

        public void DONTwriteMeeting(MeetingProposal meeting){
            server.blockedMeetings.Remove(meeting.topic);
        }

        public void addNewServer(ServerInfo serverInfo){
            server.addNewServer(serverInfo);
        }

        public void addMeeting(MeetingProposal meeting){
            server.addMeeting(meeting);
            Log.Debug("meeting " + meeting.topic + " added" );
        }
    }
}