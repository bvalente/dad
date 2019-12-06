using NUnit.Framework;
using System;
using System.Threading;
using lib;
using puppetMaster;
using static tests.Base;

namespace tests{

    public class CrashServer{

        //2 servers, 4 clients, simple success
        //create meeting, 4 clients join, close meeting
        PuppetMaster puppetMaster;
        ServerInfo server1;
        ServerInfo server2;
        ServerInfo server3;
        ClientInfo client1;
        ClientInfo client2;
        ClientInfo client3;

        [OneTimeSetUp]
        public void initialSetup(){
            SetUpChannel();

            puppetMaster = PuppetMaster.getPuppetMaster();
            puppetMaster.addRoom("Lisboa", "4", "room1");
            puppetMaster.addRoom("Porto", "3", "room2");
            
            //create servers
            server1 = puppetMaster.createServer("s1", "tcp://localhost:3001/server1", "1", "0", "0");
            server2 = puppetMaster.createServer("s2", "tcp://localhost:3002/server2", "1", "0", "0");
            server3 = puppetMaster.createServer("s3", "tcp://localhost:3003/server3", "1", "0", "0");
            Thread.Sleep(2000);
            
            //create clients
            client1 = puppetMaster.createClient("c1", "tcp://localhost:4001/client1", "tcp://localhost:3001/server1", "");
            client2 = puppetMaster.createClient("c2", "tcp://localhost:4002/client2", "tcp://localhost:3001/server1", "");
            client3 = puppetMaster.createClient("c3", "tcp://localhost:4003/client3", "tcp://localhost:3002/server2", "");
        }

        [SetUp]
        public void Setup(){
            TestContext.Progress.WriteLine("start setup");
            
            puppetMaster = PuppetMaster.getPuppetMaster();
            puppetMaster.addRoom("Lisboa", "4", "room1");
            puppetMaster.addRoom("Porto", "3", "room2");
            Thread.Sleep(3000);

            TestContext.Progress.WriteLine("setup done");
        }

        //changes server if server is dead
        [Test]
        public void changeServer(){
            TestContext.Progress.WriteLine("start test: change server after crash");

            GetClientPuppeteer(client1).createMeeting(
                "create myTopic0 1 2 0 Lisboa,2020-01-02 Porto,2020-02-03".Split(' '));
            Thread.Sleep(2000);

            string server1URL = "tcp://localhost:3001/server1";

            GetServerPuppeteer(server1).kill();
            Thread.Sleep(1000);

            GetClientPuppeteer(client1).join("join myTopic0 1 Lisboa,2020-01-02".Split(' '));
            GetClientPuppeteer(client2).join("join myTopic0 1 Lisboa,2020-01-02".Split(' '));
            GetClientPuppeteer(client3).join("join myTopic0 1 Lisboa,2020-01-02".Split(' '));

            GetClientPuppeteer(client1).close("myTopic0");

            Assert.AreNotEqual(GetClientPuppeteer(client1).getServer(), server1URL);
            Assert.AreNotEqual(GetClientPuppeteer(client2).getServer(), server1URL);
            
            TestContext.Progress.WriteLine("end test");
            Assert.Pass();
        }

        [OneTimeTearDown]
        public void finalTearDown(){
            TestContext.Progress.WriteLine("TearDown: press enter");
            Console.ReadLine();
            puppetMaster.reset();
            CloseChannel();
        }
    }
}