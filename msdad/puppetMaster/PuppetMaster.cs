using lib;
using System;
using System.Threading;
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
            string[] cmds = command.Split(' ');

            try{
                //switch, execute different command for each cmds[0]
                switch(cmds[0]){

                    case "Server":
                        createServer(cmds[1],cmds[2],cmds[3],cmds[4],cmds[5]);
                        break;
                    case "Client":
                        createClient(cmds[1],cmds[2],cmds[3],cmds[4]);
                        break;
                    case "AddRoom":
                        addRoom(cmds[1],cmds[2],cmds[3]);
                        break;
                    case "Status":
                        status();
                        break;
                    case "Crash":
                        crashServer(cmds[1]);
                        break;
                    case "Freeze":
                        freezeServer(cmds[1]);
                        break;
                    case "Unfreeze":
                        unfreezeServer(cmds[1]);
                        break;
                    case "Wait":
                        wait(cmds[1]);
                        break;
                    default:
                        Console.WriteLine("invalid command: unrecognized " + cmds[0]);
                        break;
                }
            }catch(IndexOutOfRangeException ex){
                Console.WriteLine("invalid command: not enough arguments");
                Console.WriteLine(ex.Message);
            }
            //Debug
            Console.WriteLine(command);

        }

        public void createServer(string server_id, string server_url, string max_faults,
                                string min_delay, string max_delay){
            
            //tcp://localhost:3000/server1 needs to be parsed
            //connect to correct pcs
            string[] urlParsed = server_url.Split(':');
            string pcsUrl = urlParsed[0] + ':' + urlParsed[1] + ':' + "10000/PCS";
            IPCS pcs = (IPCS) Activator.GetObject(typeof(IPCS), pcsUrl);
            
            //create server
            ServerInfo serverInfo;
            try{
                serverInfo = pcs.createServer(); //TODO change pcs interface
            } catch(Exception ex){
                //TODO catch diferent types of execeptions
                Console.WriteLine("pcs connection failed");
                Console.WriteLine(ex.Message);
                return;
            }

            IServerPuppeteer server = (IServerPuppeteer) Activator.GetObject(
                typeof(IServerPuppeteer),
                server_url);

            //TODO save server? 
            
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

        //wait some time to do the next command
        public void wait(string timeString){

            int time = Int32.Parse(timeString);
            Thread.Sleep(time);
        }

    }
}