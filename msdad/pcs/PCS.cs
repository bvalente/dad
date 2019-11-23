using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using lib;


namespace pcs{

    class PCS : MarshalByRefObject, IPCS{

        public string ping(){
            return "PCS is online";
        }

        public ClientInfo createClient(string username, string client_url, string server_url,
                 string script_file){

            //create client process
            string cPath = AppDomain.CurrentDomain.BaseDirectory;
            string clientPath = "../../../../client/bin/Debug/net472/client.exe";
            string filePath = Path.Combine(cPath,clientPath);
            string arguments = username + ' ' + client_url + ' ' + server_url + ' ' + script_file;
            
            //if Linux
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux)){
                Process.Start("konsole", "-e " +
                    "mono " + filePath + " " + arguments);
            }else{
                Process.Start(filePath, arguments);
            }

            return new ClientInfo(username, client_url, server_url, script_file);
        }

        public ServerInfo createServer(string server_id, string url,
                 string max_faults, string min_delay, string max_delay){
 
            //create server process
            string cPath = AppDomain.CurrentDomain.BaseDirectory;
            string serverPath = "../../../../server/bin/Debug/net472/server.exe";
            string filePath = Path.Combine(cPath,serverPath);
            string arguments = server_id + ' ' + url + ' ' + max_faults + ' ' + min_delay + ' ' + max_delay;

            //if linux
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux)){
                Process.Start("konsole", "-e " +
                    "mono " + filePath + " " + arguments);
            }else{
                Process.Start(filePath, arguments);
            }

            return new ServerInfo(server_id, url, max_faults, min_delay, max_delay);
        }
    }

}