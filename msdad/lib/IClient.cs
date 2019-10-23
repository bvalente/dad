namespace lib
{
    // class with important client info
    public class ClientInfo{
        private string name;
        private string url;
        private string port;

        ClientInfo(string name, string url, string port){
            this.name = name;
            this.url = url;
            this.port = port;
        }
        public string getName(){
            return this.name;
        }
        public string getUrl(){
            return this.url;
        }
        public string getPort(){
            return this.port;
        }
    }

    public interface IClient{
         // simple ping to check client status
        string ping();
    }
}