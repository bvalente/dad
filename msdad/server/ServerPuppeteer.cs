using System;
using System.Threading;
using System.Collections.Generic;
using lib;
using Serilog;

namespace server{
    //interface for the Puppet Master
    public class ServerPuppeteer : MarshalByRefObject, IServerPuppeteer{

        //save instance of server interface to access data
        public Server server;

        public ServerPuppeteer(Server server){
            this.server = server;
        }

        public bool ping(){
            return true;
        }
        
        public void statusPuppeteer(){
            server.status();
        }

        public void populate(Dictionary<string, Location> locationList,
                Dictionary<string, ServerInfo> serverList){
            server.locationList = locationList;
            server.addOldServers(serverList);
        }

        public void addRoom(string location_name, int capacity, string room_name){
            server.addRoom(location_name, capacity, room_name);
        }

        public void kill(){
            Log.Fatal("Bye bye");
            Thread t = new Thread( () => killAsync());
            t.Start();
        }

        public void killAsync(){
            Thread.Sleep(500);
            Environment.Exit(0);
        }

        public void freeze(){
            server.freeze = true;
        }

        public void unfreeze(){
            server.freeze = false;
            server.executeActionList();
        }

        public MeetingProposal getMeeting(string topic){
            if(server.meetingList.ContainsKey(topic)){
                return server.meetingList[topic];
            }
            return null;
        }

        public void undo(){
            Log.Fatal("undo all");
            server.meetingList.Clear();
            server.locationList.Clear();
            //TODO
            
        }
    }

}