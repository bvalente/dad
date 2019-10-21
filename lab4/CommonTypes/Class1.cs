using System;

namespace CommonTypes
{
    public interface IServer{
    string Ping();

    }

    public class Person{ //serializable?
        string Name;
        int Age;
    }
}
