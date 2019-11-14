using System;
using System.IO;
using System.Diagnostics;
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
            string filePath = Path.Combine(cPath,
                 "../../../../client/bin/Debug/net472/client.exe");
            Process client = Process.Start(filePath, 
                username + ' ' + client_url + ' ' + server_url + ' ' + script_file);

            return new ClientInfo(username, client_url, server_url, script_file);
        }

        public ServerInfo createServer(string server_id, string url,
                 string max_faults, string min_delay, string max_delay){
 
            //create server process
            string cPath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(cPath,
                 "../../../../server/bin/Debug/net472/server.exe");
            Process server = Process.Start(filePath, 
                server_id + ' ' + url + ' ' + max_faults + ' ' + min_delay + ' ' + max_delay);

            return new ServerInfo(server_id, url, max_faults, min_delay, max_delay);
        }
    }

}