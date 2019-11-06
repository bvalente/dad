using lib;
using System;
using System.Threading;
using System.Collections.Generic;

namespace puppetMaster {

    class PuppetMaster{

        //singleton
        private static PuppetMaster puppetMaster;

        //window
        MainWindow window;

        //client and server dictionaries
        Dictionary<string,ClientInfo> clientList;
        Dictionary<string,ServerInfo> serverList;

        //no constructor()? 
        //TODO make singleton
        private PuppetMaster(MainWindow window){
            //save window
            this.window = window;
            //create dictionaries
            clientList = new Dictionary<string, ClientInfo>();
            serverList = new Dictionary<string, ServerInfo>();
        }

        public static PuppetMaster getPuppetMaster(MainWindow window){
            if(puppetMaster == null){
                puppetMaster = new PuppetMaster(window);
            }
            return puppetMaster;
        }

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
                        if (cmds.Length==5){
                            createClient(cmds[1],cmds[2],cmds[3],cmds[4]);
                        }else{//no script file
                            createClient(cmds[1],cmds[2],cmds[3],"");
                        }
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
                serverInfo = pcs.createServer(server_id, server_url,
                         max_faults, min_delay, max_delay);
            } catch(Exception ex){
                //TODO catch diferent types of execeptions
                Console.WriteLine("pcs connection failed");
                Console.WriteLine(ex.Message);
                return;
            }

            //save server
            serverList.Add(server_id, serverInfo);
            window.addServer(serverInfo);
            
        }

        public void createClient(string username, string client_url, string server_url,
                                string script_file ){

            //connect to correct pcs
            string[] urlParsed = server_url.Split(':');
            string pcsUrl = urlParsed[0] + ':' + urlParsed[1] + ':' + "10000/PCS";
            IPCS pcs = (IPCS) Activator.GetObject(typeof(IPCS), pcsUrl);

            //create client
            ClientInfo clientInfo;
            try{
                clientInfo = pcs.createClient(username, client_url, server_url,
                                script_file);
            } catch(Exception ex){
                //TODO catch diferent types of execeptions
                Console.WriteLine("pcs connection failed");
                Console.WriteLine(ex.Message);
                return;
            }

            //save client
            clientList.Add(username, clientInfo);
            window.addClient(clientInfo);
        }

        public void addRoom(string location, string capacity, string room_name){
            
            //create room, pass it to all servers?
            
        }

        public void status(){

            //make all servers and clients print status
            

        }

        public void crashServer(string server_id){

            //crash server
            ServerInfo serverInfo = serverList[server_id];
            IServerPuppeteer server = (IServerPuppeteer) Activator.GetObject(
                    typeof(IServerPuppeteer),
                    serverInfo.url+"Puppeteer");
            try{
                //TODO works, but needs to be assyncronous, dont wait for response
                server.kill();
            }catch(Exception ex){
                Console.WriteLine("server connection failed");
                Console.WriteLine(ex.Message);
                return;
            }

            window.removeServer(serverInfo);
            serverList.Remove(server_id);
        }

        public void freezeServer(string server_id){
            
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