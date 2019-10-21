using NUnit.Framework;
using System;

namespace ClientNUnit
{
    [TestFixture]
    public class Tests
    {
        public string test = "0";

        [OneTimeSetUp]
        public void ClassSetup(){
            test = "1";

        }

        [SetUp]
        public void Setup(){
            //test = "2";
        }

        [Test]
        public void Test1(){
            Print(test);
            Assert.Pass();
        }

        public static void Print(string message){
            TestContext.Progress.WriteLine("log: " + message);
        }
    }
}