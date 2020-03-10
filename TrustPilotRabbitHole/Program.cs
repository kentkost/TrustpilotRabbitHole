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
    static Dictionary<string, HashSet<string>> map = new Dictionary<string, HashSet<string>>();
    static void Main(string[] args)
    {
      string anagram = "poultry outwits ants";
      //string anagram = "dormitory";
      //string anagram = "funeral";
      if (!File.Exists("newWords.txt")) {
        SortWords("wordlist.txt", anagram);
      }

      DAWG dawg = new DAWG("newWords.txt", map);
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
      using (StreamReader file = new StreamReader(path)) {
        string ln;
        string prev = "";
        string temp = "";

        while ((ln = file.ReadLine()) != null) {
          temp = NormalizeString(ln);
          if (IsSubsetString(input, temp) && ln.Length > 1) {
            if (!IdenticalStrings(ln, prev)) {
              outfile.WriteLine(temp);
              prev = ln;
            }
            else {
              string normPrev = NormalizeString(prev);
              if (!map.ContainsKey(normPrev)) {
                map.Add(normPrev, new HashSet<string>());
                map[normPrev].Add(prev);
              }
              map[normPrev].Add(ln);
            }
          }
        }
        file.Close();
        outfile.Close();
      }
    }

    static string NormalizeString(string str)
    {
      return Regex.Replace(str.Normalize(NormalizationForm.FormD), @"[^a-z]", "");
    }

    static bool IdenticalStrings(string str1, string str2)
    {
      str1 = Regex.Replace(str1.Normalize(NormalizationForm.FormD), @"[^a-z]", "");
      str2 = Regex.Replace(str2.Normalize(NormalizationForm.FormD), @"[^a-z]", "");
      return str2 == str1;
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
