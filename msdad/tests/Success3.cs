using NUnit.Framework;
using System;
using System.Threading;
using lib;
using puppetMaster;
using static tests.Base;

namespace tests{

    public class Tests{

        //3 servers, 3 clients, simple success
        //create meeting, 3 clients join, close meeting
        PuppetMaster puppetMaster;
        ServerInfo server1;
        ServerInfo server2;
        ServerInfo server3;
        ClientInfo client1;
        ClientInfo client2;
        ClientInfo client3;

        [SetUp]
        public void Setup(){
            TestContext.Progress.WriteLine("start setup");
            
            SetUpChannel();

            puppetMaster = PuppetMaster.getPuppetMaster();
            puppetMaster.addRoom("Lisboa", "2", "room1");
            puppetMaster.addRoom("Porto", "1", "room2");
            
            //create servers
            server1 = puppetMaster.createServer("s1", "tcp://localhost:3001/server1", "0", "0", "0");
            server2 = puppetMaster.createServer("s2", "tcp://localhost:3002/server2", "0", "0", "0");
            server3 = puppetMaster.createServer("s3", "tcp://localhost:3003/server3", "0", "0", "0");
            Thread.Sleep(2000);
            
            //create clients
            client1 = puppetMaster.createClient("c1", "tcp://localhost:4001/client1", "tcp://localhost:3001/server1", "");
            client2 = puppetMaster.createClient("c2", "tcp://localhost:4002/client2", "tcp://localhost:3002/server2", "");
            client3 = puppetMaster.createClient("c3", "tcp://localhost:4003/client3", "tcp://localhost:3003/server3", "");
            Thread.Sleep(2000);

            TestContext.Progress.WriteLine("setup done");
        }

        [Test]
        public void success(){
            TestContext.Progress.WriteLine("start test");

            //ping servers
            Assert.True(GetServerPuppeteer(server1).ping());
            Assert.True(GetServerPuppeteer(server2).ping());
            Assert.True(GetServerPuppeteer(server3).ping());
            //ping clients
            Assert.True(GetClientPuppeteer(client1).ping());
            Assert.True(GetClientPuppeteer(client2).ping());
            Assert.True(GetClientPuppeteer(client3).ping());
            
            //clients create join and close meeting
            GetClientPuppeteer(client1).createMeeting(
                "create myTopic0 1 2 2 Lisboa,2020-01-02 Porto,2020-02-03 c1 c2".Split(' '));
            Thread.Sleep(1000);
            GetClientPuppeteer(client1).join("join myTopic0 1 Lisboa,2020-01-02".Split(' '));
            GetClientPuppeteer(client2).join("join myTopic0 1 Lisboa,2020-01-02".Split(' '));
            GetClientPuppeteer(client3).join("join myTopic0 1 Lisboa,2020-01-02".Split(' '));
            Thread.Sleep(1000);
            GetClientPuppeteer(client1).close("myTopic0");
            Thread.Sleep(2000);

            //Get meeting and check parameters
            MeetingProposal m1 = GetClientPuppeteer(client1).getMeeting("myTopic0");
            MeetingProposal m2 = GetClientPuppeteer(client2).getMeeting("myTopic0");
            MeetingProposal m3 = GetClientPuppeteer(client3).getMeeting("myTopic0");

            Assert.False(m1.open);
            Assert.False(m2.open);
            Assert.False(m3.open);

            TestContext.Progress.WriteLine("end test");
            Assert.Pass();
        }

        [TearDown]
        public void TearDown(){
            TestContext.Progress.WriteLine("TearDown: press enter");
            Console.ReadLine();
            puppetMaster.reset();
        }
    }
}