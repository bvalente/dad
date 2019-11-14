using System;
using System.Runtime.Serialization;

namespace lib{

    //interface for Puppet Master
    public interface IPCS{
        // simple ping to check PCS status
        string ping();

        // create client and return important info
        ClientInfo createClient(string username, string client_url, string server_url,
                 string script_file);

        // create server and return important info
        ServerInfo createServer(string server_id, string url,
                 string max_faults, string min_delay, string max_delay);
    }

    public class PCSException : Exception{
        public PCSException(){
        }

        public PCSException(string message)
            : base(message){
        }

        public PCSException(string message, Exception inner) 
            :base(message, inner){
        }
        
        public PCSException(SerializationInfo info, StreamingContext context)
            : base(info, context){
        }
    }
}