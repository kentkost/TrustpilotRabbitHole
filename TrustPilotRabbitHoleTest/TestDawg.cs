using NUnit.Framework;
using TrustPilotRabbitHole;
using System;
using System.IO;
namespace TrustPilotRabbitHoleTest
{
    public class Tests
    {
        private DAWG dawg;
        [SetUp]
        public void Setup()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            dawg = new DAWG(Path.Combine(projectDirectory, "..", "testdata", "testWords.txt"), "airport airily");
        }

        [Test]
        public void TestFileExists()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            Assert.IsTrue(File.Exists(Path.Combine(projectDirectory, "..", "testdata", "testWords.txt")));
        }

        [Test]
        public void TestDawgPopulated()
        {
            Assert.Greater(dawg.root.Children.Count, 0);
        }

        [Test]
        public void TestWordNonLeaf()
        {
            bool wordExists = dawg.FindWord("airport");
            Assert.True(wordExists);
        }
        [Test]
        public void TestWordAtLeaf()
        {
            bool wordExists = dawg.FindWord("airily");
            Assert.True(wordExists);
        }
        [Test]
        public void TestWordNotExists()
        {
            bool wordExists = dawg.FindWord("airi");
            Assert.False(wordExists);
        }

        [Test]
        public void TestMD5Hash()
        {
            string s = "hello world";
            string hash = dawg.MD5hash(s);
            Assert.AreEqual("5eb63bbbe01eeed093cb22bb8f5acdc3", hash);
        }

    }
}