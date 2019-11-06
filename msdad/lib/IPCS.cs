namespace lib
{
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
}