using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TrustPilotRabbitHole
{
  class Program
  {
    static void Main(string[] args)
    {
      string anagram = "poultry outwits ants";
      //string anagram = "dormitory";
      //string anagram = "funeral";
      if (!File.Exists("newWords.txt")) {
        SortWords("wordlist.txt", anagram);
      }
      DAWG dawg = new DAWG("newWords.txt");
      LetterFrequencyMatrix rankWords = new LetterFrequencyMatrix("newWords.txt", anagram);
      //DAWG dawg = new DAWG("testAnagrams.txt");
      //DAWG dawg = new DAWG("testAnagrams2.txt");
      foreach (string guess in rankWords.guessWords) {
        dawg.FindAnagrams(anagram, guess);
      }
      Console.WriteLine("Done");
      Console.ReadKey();
    }

    static void SortWords(string path, string input)
    {
      using (StreamWriter outfile = new StreamWriter("newWords.txt"))
      using (StreamReader file = new StreamReader(path))
      {
        string ln;
        string prev = "";
        while ((ln = file.ReadLine()) != null)
        {
          if (IsSubsetString(input, ln) && ln.Length > 2)
          {
            if (prev != ln) {
              outfile.WriteLine(ln);
              prev = ln;
            }
          }
        }
        file.Close();
        outfile.Close();
      }

    }

    static bool IsSubsetString(string input, string compare)
    {
      //remove whitespace
      input = Regex.Replace(input, @"\s+", "");
      compare = Regex.Replace(compare, @"\s+", "");

      List<char> inp = String.Concat(input.OrderBy(c => c)).ToList();

      foreach (char c in compare) {
        bool found = inp.Remove(c);
        if (!found) {
          return false;
        }
      }

      return true;
    }
  }
}
