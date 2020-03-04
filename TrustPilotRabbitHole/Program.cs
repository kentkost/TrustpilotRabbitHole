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
      //DAWG dawg = new DAWG("testAnagrams.txt");
      //DAWG dawg = new DAWG("testAnagrams2.txt");
      dawg.FindAnagrams(anagram);
      Console.WriteLine("Done");
      Console.ReadKey();
    }

    static void SortWords(string path, string input)
    {
      using (StreamWriter outfile = new StreamWriter("newWords.txt"))
      using (StreamReader file = new StreamReader(path))
      {
        string ln;

        while ((ln = file.ReadLine()) != null)
        {
          if (IsSubsetString(input, ln) && ln.Length > 2)
          {
            outfile.WriteLine(ln);
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

      //sort both words into list 
      List<char> inp = String.Concat(input.OrderBy(c => c)).ToList();
      List<char> cmp = String.Concat(compare.OrderBy(c => c)).ToList();

      int i = 0, j = 0;
      while (j < cmp.Count)
      {
        //If I want special characters
        //if (!Char.IsLetter(cmp[j]))
        //{
        //    j++; continue;
        //}
        while (inp[i] != cmp[j])
        {
          i++;
          if (i > inp.Count - 1)
          {
            return false;
          }
        }
        j++;
      }
      return true;
    }
  }
}
