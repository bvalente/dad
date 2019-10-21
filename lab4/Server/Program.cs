using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}

namespace CommonTypes{

    class ServerChannel : MarshalByRefObject,IServer{

        public string Ping(){
            return "I am Server";
        }

    }
}