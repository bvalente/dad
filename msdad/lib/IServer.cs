namespace lib
{
    // class with important server info
    // TODO discuss serverInfo important values
    public class ServerInfo{

        private string url;
        private string port;

        ServerInfo(string url, string port){
            this.url = url;
            this.port = port;
        }
        public string getUrl(){
            return this.url;
        }
        public string getPort(){
            return this.port;
        }
    }

    public interface IServer{

        // simple ping to check server status
        string ping();

        // create meeting 
        void createMeeting(MeetingProposal meeting);

        void addClient(ClientInfo client);
    }
}