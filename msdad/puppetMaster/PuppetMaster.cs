using lib;
using System.Collections.Generic;

namespace puppetMaster {

    class PuppetMaster{

        //client and server dictionaries
        Dictionary<ClientInfo,IClientPuppeteer> clientList;
        Dictionary<ServerInfo,IServerPuppeteer> serverList;

        //no constructor()? 
        //TODO make singleton

        //receives a command and executes respective function
        public void executeCommand(string command){
            string[] commands = command.Split(' ');

            //switch, execute different command for each commands[0]
        }

        public void createServer(string server_id, string server_url, string max_faults,
                                string min_delay, string max_delay){
            
            //connect to correct pcs

            //create server
        }

        public void createClient(string username, string client_url, string server_url,
                                string script_file ){

            //connect to correct pcs

            //create client

            //execute script_file if not null
        }

        public void addRoom(string loctation, string capacity, string room_name){
            
            //create room, pass it to all servers?
        }

        public void status(){

            //make all servers and clients print status
        }

        public void crashServer(string server_id){

            //crash server
        }

        public void freezeServer(string server_id){

            //freeze server
        }

        public void unfreezeServer(string server_id){

            //unfreeze server
        }
    }
}