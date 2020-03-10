using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;

namespace TrustPilotRabbitHole
{
  class Program
  {
    static void Main(string[] args)
    {
      string anagram = "poultry outwits ants";
      DAWG dawg = new DAWG("wordlist", anagram);

      Console.WriteLine("Program exited successfully");
      Console.ReadKey();
    }
  }
}
