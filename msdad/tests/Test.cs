using NUnit.Framework;
using System;
using System.Threading;
using lib;
using puppetMaster;
using static tests.Base;

namespace tests{

    public class Tests{

        //2 servers, 4 clients, simple success
        //create meeting, 4 clients join, close meeting
        PuppetMaster puppetMaster;
        ServerInfo server1;
        ServerInfo server2;
        ServerInfo server3;
        ClientInfo client1;
        ClientInfo client2;
        ClientInfo client3;
        ClientInfo client4;

        [OneTimeSetUp]
        public void initialSetup(){
            SetUpChannel();

            puppetMaster = PuppetMaster.getPuppetMaster();
            puppetMaster.addRoom("Lisboa", "4", "room1");
            puppetMaster.addRoom("Porto", "3", "room2");
            
            //create servers
            server1 = puppetMaster.createServer("s1", "tcp://localhost:3001/server1", "0", "0", "0");
            server2 = puppetMaster.createServer("s2", "tcp://localhost:3002/server2", "0", "0", "0");
            server3 = puppetMaster.createServer("s3", "tcp://localhost:3003/server3", "0", "0", "0");
            Thread.Sleep(2000);
            
            //create clients
            client1 = puppetMaster.createClient("c1", "tcp://localhost:4001/client1", "tcp://localhost:3001/server1", "");
            client2 = puppetMaster.createClient("c2", "tcp://localhost:4002/client2", "tcp://localhost:3001/server1", "");
            client3 = puppetMaster.createClient("c3", "tcp://localhost:4003/client3", "tcp://localhost:3002/server2", "");
            client4 = puppetMaster.createClient("c4", "tcp://localhost:4004/client4", "tcp://localhost:3002/server2", "");
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
        
        //simple ping success
        //[Test]
        public void success(){
            TestContext.Progress.WriteLine("start test: success");

            //ping servers
            Assert.True(GetServerPuppeteer(server1).ping());
            Assert.True(GetServerPuppeteer(server2).ping());
            //ping clients
            Assert.True(GetClientPuppeteer(client1).ping());
            Assert.True(GetClientPuppeteer(client2).ping());
            
            //clients create join and close meeting
            GetClientPuppeteer(client1).createMeeting(
                "create myTopic0 1 2 2 Lisboa,2020-01-02 Porto,2020-02-03 c1 c2".Split(' '));
            Thread.Sleep(2000);
            GetClientPuppeteer(client1).join("join myTopic0 1 Lisboa,2020-01-02".Split(' '));
            GetClientPuppeteer(client2).join("join myTopic0 1 Lisboa,2020-01-02".Split(' '));
            Thread.Sleep(2000);
            GetClientPuppeteer(client1).close("myTopic0");
            Thread.Sleep(3000);

            //Get meeting and check parameters
            MeetingProposal m1 = GetClientPuppeteer(client1).getMeeting("myTopic0");
            MeetingProposal m2 = GetClientPuppeteer(client2).getMeeting("myTopic0");

            Assert.False(m1.open);
            Assert.False(m2.open);

            TestContext.Progress.WriteLine("end test");
            Assert.Pass();
        }

        //meeting com minimo capacidade 5
        //[Test]
        public void NotEnoughParticipants(){
            TestContext.Progress.WriteLine("start test: not enough participants");
            
            //clients create join and close meeting
            GetClientPuppeteer(client1).createMeeting(
                "create myTopic0 5 2 0 Lisboa,2020-01-02 Porto,2020-02-03".Split(' '));
            Thread.Sleep(1000);
            GetClientPuppeteer(client1).join("join myTopic0 1 Lisboa,2020-01-02".Split(' '));
            GetClientPuppeteer(client2).join("join myTopic0 1 Lisboa,2020-01-02".Split(' '));
            GetClientPuppeteer(client3).join("join myTopic0 1 Lisboa,2020-01-02".Split(' '));
            GetClientPuppeteer(client4).join("join myTopic0 1 Lisboa,2020-01-02".Split(' '));
            Thread.Sleep(2000);

            Assert.Throws( Is.TypeOf<ClientException>()
                .And.Message.EqualTo( "cannot close meeting" )
                .And.InnerException.InstanceOf(typeof(MeetingException))
                .And.InnerException.Message.EqualTo("Not enough participants"),
                delegate { GetClientPuppeteer(client1).close("myTopic0");} );

            TestContext.Progress.WriteLine("end test");
            Assert.Pass();
        }

        //close by someone not coordinator
        //[Test]
        public void NotCoordinator(){
            TestContext.Progress.WriteLine("start test: not coordinator");
            
            //clients create join and close meeting
            GetClientPuppeteer(client1).createMeeting(
                "create myTopic0 2 2 0 Lisboa,2020-01-02 Porto,2020-02-03".Split(' '));
            Thread.Sleep(1000);
            GetClientPuppeteer(client1).join("join myTopic0 1 Lisboa,2020-01-02".Split(' '));
            GetClientPuppeteer(client2).join("join myTopic0 1 Lisboa,2020-01-02".Split(' '));
 
            Thread.Sleep(1000);
            Assert.Throws( Is.TypeOf<ClientException>()
                .And.Message.EqualTo( "cannot close meeting" )
                .And.InnerException.InstanceOf(typeof(MeetingException))
                .And.InnerException.Message.EqualTo("This client can't close the meeting"),
                delegate { GetClientPuppeteer(client2).close("myTopic0");} );

            TestContext.Progress.WriteLine("end test");
            Assert.Pass();
        }
        
        //try close meeting two times in row
        //[Test]
        public void AlreadyClose(){
            TestContext.Progress.WriteLine("start test: already closed");
            
            //clients create join and close meeting
            GetClientPuppeteer(client1).createMeeting(
                "create myTopic0 2 2 0 Lisboa,2020-01-02 Porto,2020-02-03".Split(' '));
            Thread.Sleep(1000);
            GetClientPuppeteer(client1).join("join myTopic0 1 Lisboa,2020-01-02".Split(' '));
            GetClientPuppeteer(client2).join("join myTopic0 1 Lisboa,2020-01-02".Split(' '));
 
            Thread.Sleep(1000);
            //primeiro close
            TestContext.Progress.WriteLine("1st close");
            MeetingProposal m1 = GetClientPuppeteer(client1).close("myTopic0");
            MeetingProposal m2 = GetClientPuppeteer(client1).getMeeting("myTopic0");
            Assert.False(m1.open);
            Assert.False(m2.open);
            //tries to close meeting again
            TestContext.Progress.WriteLine("2nd close");
            Assert.Throws( Is.TypeOf<ClientException>()
                .And.Message.EqualTo( "cannot close meeting" )
                .And.InnerException.InstanceOf(typeof(MeetingException))
                .And.InnerException.Message.EqualTo("Meeting is already closed"),
                delegate { GetClientPuppeteer(client1).close("myTopic0");} );

            TestContext.Progress.WriteLine("end test");
            Assert.Pass();
        }

        //sala com capacidade 3
        //[Test]
        public void MaxParticipantsExceededInRoom(){
            TestContext.Progress.WriteLine("start test: max participants exceeded in room");
            
            //clients create join and close meeting
            GetClientPuppeteer(client1).createMeeting(
                "create myTopic0 4 2 0 Lisboa,2020-01-02 Porto,2020-02-03".Split(' '));
            Thread.Sleep(1000);
            GetClientPuppeteer(client1).join("join myTopic0 1 Porto,2020-02-03".Split(' '));
            GetClientPuppeteer(client2).join("join myTopic0 1 Porto,2020-02-03".Split(' '));
            GetClientPuppeteer(client3).join("join myTopic0 1 Porto,2020-02-03".Split(' '));
            GetClientPuppeteer(client4).join("join myTopic0 1 Porto,2020-02-03".Split(' '));
            Thread.Sleep(1000);

            Assert.Throws( Is.TypeOf<ClientException>()
                .And.Message.EqualTo( "cannot close meeting" )
                .And.InnerException.InstanceOf(typeof(MeetingException))
                .And.InnerException.Message.EqualTo("No available rooms"),
                delegate { GetClientPuppeteer(client1).close("myTopic0");} );

            TestContext.Progress.WriteLine("end test");
            Assert.Pass();
        }

        //tries to close meeting that does not exist
        //[Test]
        public void MeetingDoesNotExist(){
            TestContext.Progress.WriteLine("start test: meeting does not exist");

            Assert.Throws( Is.TypeOf<ClientException>()
                .And.Message.EqualTo( "cannot close meeting" )
                .And.InnerException.InstanceOf(typeof(MeetingException))
                .And.InnerException.Message.EqualTo("meeting does not exist"),
                delegate { GetClientPuppeteer(client1).close("myTopic0");} );

            TestContext.Progress.WriteLine("end test");
            Assert.Pass();
        }

        [Test]
        public void FreezeTest(){
            TestContext.Progress.WriteLine("start test: frozen server");

            //clients create join and close meeting
            GetClientPuppeteer(client1).createMeeting(
                "create myTopic0 2 2 0 Lisboa,2020-01-02 Porto,2020-02-03".Split(' '));
            Thread.Sleep(1000);
            GetClientPuppeteer(client1).join("join myTopic0 1 Porto,2020-02-03".Split(' '));
            GetClientPuppeteer(client3).join("join myTopic0 1 Porto,2020-02-03".Split(' '));
            Thread.Sleep(1000);

            GetServerPuppeteer(server2).freeze();
            GetClientPuppeteer(client1).close("myTopic0");
            Assert.AreEqual(GetServerPuppeteer(server2).getMeeting("myTopic0").version, 3);
            Assert.AreEqual(GetClientPuppeteer(client3).getMeeting("myTopic0").version, 3);


            GetServerPuppeteer(server2).unfreeze();
            Thread.Sleep(1000);
            Assert.AreEqual(GetServerPuppeteer(server2).getMeeting("myTopic0").version, 4);
            Assert.AreEqual(GetClientPuppeteer(client3).getMeeting("myTopic0").version, 4);


            TestContext.Progress.WriteLine("end test");
            Assert.Pass();
        }


        [TearDown]
        public void TearDown(){
            TestContext.Progress.WriteLine("TearDown: press enter");
            Console.ReadLine();
            //puppetMaster.reset();
            puppetMaster.undoAll();
        }

        [OneTimeTearDown]
        public void finalTearDown(){
            puppetMaster.reset();
            CloseChannel();
        }
    }
}