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
      dawg = new DAWG(Path.Combine(projectDirectory,"..","testdata","testWords.txt"));
    }
    [Test]
    public void TestDawgPopulated()
    {
      Assert.True(dawg.root.Children.Count > 0);
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
  }
}