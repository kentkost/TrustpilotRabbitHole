using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace TrustPilotRabbitHole
{
  class DAWG
  {
    internal Node root;
    //4624d200580677270a54ccff86b9610e = pastils turnout towy
    public List<string> anagrams = new List<string>();
    public List<string> hashes = new List<string>() {
      "e4820b45d2277f3844eac66c903e84be",
      "23170acc097c24edb98fc5488ab033fe",
      "665e5bcb0c20062fe8abaaf4628bb154"
    };

    private Dictionary<string, HashSet<string>> map = new Dictionary<string, HashSet<string>>();

    public DAWG(string path, Dictionary<string, HashSet<string>> m)
    {
      root = new Node('\0', null);
      map = m;
      List<string> words = File.ReadAllLines(path).ToList();
      Create(words);
    }

    private void CheckAnagram(List<string> phrase) {
      List<List<string>> phrases = new List<List<string>>();
      SwapWords(phrase, 0, phrases);
      foreach (List<string> phr in phrases) {
        List<string> permsPhrase = Permutations(phr);
        foreach (string s in permsPhrase) {
          string hash = MD5hash(s);
          for (int i = 0; i < hashes.Count; i++) {
            if (hash == hashes[i]) {
              Console.WriteLine(s + " : " + hash);
              hashes.RemoveAt(i);
              break;
            }
          }
        }
      }
    }

    public void SwapWords(List<string> sentence, int i, List<List<string>> results)
    {
      List<List<string>> combo = new List<List<string>>();
      if (i < sentence.Count && map.ContainsKey(sentence[i])) {
        foreach (string s in map[sentence[i]]) {
          List<string> temp = new List<string>(sentence);
          temp[i] = s;
          combo.Add(temp);
        }
        foreach (List<string> ls in combo) {
          SwapWords(new List<string>(ls), i + 1, results);
        }
      }
      else {
        results.Add(sentence);
      }
    }

    public string MD5hash(string s)
    {
      MD5 md5 = MD5.Create();
      byte[] inputBytes = Encoding.ASCII.GetBytes(s);
      byte[] hash = md5.ComputeHash(inputBytes);
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < hash.Length; i++) {
        sb.Append(hash[i].ToString("X2"));
      }
      return sb.ToString().ToLower();
    }

    private List<char> SubtractLetters(List<char> anagram, string guess)
    {
      foreach (char c in guess) {
        anagram.Remove(c);
      }
      return anagram;
    }


    public void FindAnagrams(string anagram, string guess)
    {
      List<char> inp = String.Concat(anagram.OrderBy(c => c)).ToList();
      inp.RemoveAll(item => item ==' ');
      inp = SubtractLetters(inp, guess);
      FindAnagram(new List<string>() { "" }, inp, guess);
    }

    private void FindAnagram(List<string> phrase, List<char> letters, string guess)
    {
      if (letters.Count == 0 && FindWord(phrase[phrase.Count - 1])) {
        List<string> newPhrase = new List<string>(phrase);
        newPhrase.Add(guess);
        CheckAnagram(newPhrase);
        return;
      }

      //Over distinct letters or else it will end up with duplicate branches
      foreach (char c in letters.Distinct()) {
        string str = phrase.Last();
        List<char> newLetters = new List<char>(letters);
        newLetters.Remove(c);

        if (WordIsPossible(str + c)) {
          List<string> newPhrase = new List<string>(phrase);
          newPhrase[newPhrase.Count - 1] += c;
          FindAnagram(newPhrase, newLetters, guess);
        }
        if (FindWord(str)) { //current word in phrase is a word and should therefore be spaced
          List<string> newPhrase = new List<string>(phrase);
          string temp = newPhrase.Last();
          if (temp[0] < c) { //Prevents permutations of a phrase. If an anagram is found we will test its permutations
            newPhrase.Add("" + c);
            FindAnagram(newPhrase, newLetters, guess);
          }
        }
      }
    }

    public bool WordIsPossible(string word)
    {
      Node current = root;
      foreach (char c in word) {
        current = current.FindChild(c);
        if (current == null) {
          return false;
        }
      }
      return true;
    }

    public bool FindWord(string word)
    {
      Node current = root;
      foreach (char c in word) {
        current = current.FindChild(c);
        if (current == null){
          return false;
        }
      }

      if (current.IsWord) {
        return true;
      } 
      else {
        return false;
      }
    }

    private List<string> Permutations(List<string> phrase)
    {
      List<string> results = new List<string>();
      int n = phrase.Count;
      Permute(phrase, 0, n - 1, ref results);
      return results;
    }

    private void Permute(List<string> phrase, int l, int r, ref List<string> results)
    {
      if (l == r) {
        results.Add(String.Join(" ", phrase.ToArray()));
      }
      else {
        for (int i = l; i <= r; i++) {
          phrase = Swap(phrase, l, i);
          Permute(phrase, l + 1, r, ref results);
          phrase = Swap(phrase, l, i);
        }
      }
    }

    private static List<string> Swap(List<String> words, int i, int j)
    {
      string temp;
      temp = words[i];
      words[i] = words[j];
      words[j] = temp;
      return words;
    }

    private void Insert(string word)
    {
      Node current = root;

      for (int i = 0; i < word.Length; i++){
        Node child = current.FindChild(word[i]);

        if (child == null){
          current = current.AddChild(word[i]);
        }
        else{
          current = child;
        }
        
        if (!current.IsWord){
          current.IsWord = (i == word.Length - 1);
        }
      }
    }

    private void Create(List<string> words)
    {
      foreach (string s in words){
        Insert(s);
      }
    }
  }
}
