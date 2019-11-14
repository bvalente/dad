namespace lib{
    
    // TODO discuss, does puppet master need an interface?
    // maybe it is useful to send information to display in the UI
    public interface IPupperMaster{
         // simple ping to check puppetMaster status
        string ping();
    }
}