using System;
using System.Collections.Generic;
using lib;
using Serilog;

namespace server{
		//interface for server to other servers
	    public class ServerToServer : MarshalByRefObject, IServerToServer {

        Server server;

        public ServerToServer(Server server){
            this.server = server;
        }

        public bool ping(){
            return true;
        }

        //get meeting
        public bool sendMeeting(MeetingProposal meeting){
            Log.Debug("received meeting {topic} version {v}", meeting.topic, meeting.version);
            server.randomSleep();
            //check version here?
            if (server.meetingList.ContainsKey(meeting.topic)){
                //server has meeting, check version and if meeting is blocked
                //FIXME > vs >=
                //FIXME closed vs open
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
        public void writeMeeting(MeetingProposal meeting, List<ServerInfo> servers){
            Log.Debug("write meeting {topic} version {v}", meeting.topic, meeting.version);
            server.randomSleep();
            if(server.meetingList.ContainsKey(meeting.topic) && 
                    meeting.version > server.meetingList[meeting.topic].version){
                server.meetingList.Remove(meeting.topic);
                server.meetingList.Add(meeting.topic, meeting);
                server.blockedMeetings.Remove(meeting.topic);
                //update location's room
                if(meeting.room != null){
                    Location location = meeting.room.location;
                    location.roomList.Remove(meeting.room.room_name);
                    location.roomList.Add(meeting.room.room_name, meeting.room);
                }
                //Async send meeting info to clients
                server.updateClients(meeting);
            }
            //send broadcast to servers
            server.writeBroadcast(meeting, servers);
        }

        public void DONTwriteMeeting(MeetingProposal meeting, List<ServerInfo> servers){
            server.randomSleep();
            server.blockedMeetings.Remove(meeting.topic);
            server.DONTwriteBroadcast(meeting, servers);
        }

        public void addNewServer(ServerInfo serverInfo){
            server.randomSleep();
            server.addNewServer(serverInfo);
        }

        public void addMeeting(MeetingProposal meeting){
            // server.randomSleep();
            // server.addMeeting(meeting);
            // Log.Debug("meeting " + meeting.topic + " added" );
        }
    }
}