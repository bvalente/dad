using System;
using System.Collections.Generic;
using lib;

namespace server{
    //interface for the Puppet Master
    public class ServerPuppeteer : MarshalByRefObject, IServerPuppeteer{

        //save instance of server interface to access data
        public Server server;

        public ServerPuppeteer(Server server){
            this.server = server;
        }

        public string ping(){
            return "Server Puppeteer is online";
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
            Console.WriteLine("Bye bye");
            Environment.Exit(0);
        }

        public void freeze(){
            server.freeze = true;
        }

        public void unfreeze(){
            server.freeze = false;
            server.executeActionList();
        }

    }
}