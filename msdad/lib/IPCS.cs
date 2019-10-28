namespace lib
{
    public interface IPCS{
        // simple ping to check PCS status
        string ping();

        // create client and return important info
        ClientInfo createClient(string name, string port);

        // create server and return important info
        ServerInfo createServer(string port);
    }
}