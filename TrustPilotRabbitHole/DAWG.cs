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
    public List<string> anagrams = new List<string>();
    public List<string> hashes = new List<string>() {
      "e4820b45d2277f3844eac66c903e84be",
      "23170acc097c24edb98fc5488ab033fe",
      "665e5bcb0c20062fe8abaaf4628bb154"
    };

    public DAWG(string path)
    {
      root = new Node('\0', null);
      List<string> words = File.ReadAllLines(path).ToList();
      Create(words);
    }

    private void CheckAnagram(List<string> phrase) {
      List<string> permsPhrase = Permutations(phrase);
      foreach (string s in permsPhrase) {
        string hash = MD5hash(s);
        for (int i = 0; i < hashes.Count; i++) {
          if (hash == hashes[i]) {
            Console.WriteLine(s+ " : "+hash);
            hashes.RemoveAt(i);
            break;
          }
        }
      }
    }

    public string MD5hash(string s)
    {
      MD5 md5 = MD5.Create();
      byte[] inputBytes = Encoding.ASCII.GetBytes(s);
      byte[] hash = md5.ComputeHash(inputBytes);

      // step 2, convert byte array to hex string
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < hash.Length; i++) {
        sb.Append(hash[i].ToString("X2"));
      }
      return sb.ToString().ToLower();
    }

    //Add assumed words to parameter
    public void FindAnagrams(string anagram)
    {
      List<char> inp = String.Concat(anagram.OrderBy(c => c)).ToList();
      inp.RemoveAll(item => item ==' ');

      FindAnagram(new List<string>() { "" }, inp);
      //foreach (string s in anagrams) {
      //  Console.WriteLine(s);
      //}
    }

    private void FindAnagram(List<string> phrase, List<char> letters)
    {
      if (letters.Count == 0 && FindWord(phrase[phrase.Count - 1])) {
        //anagrams.Add(string.Join(" ", phrase.ToArray()));
        CheckAnagram(phrase);
      }

      //Over distinct letters or else it will end up with duplicate branches
      foreach (char c in letters.Distinct()) {
        string str = phrase.Last();
        List<char> newLetters = new List<char>(letters);
        newLetters.Remove(c);

        if (WordIsPossible(str + c)) {
          List<string> newPhrase = new List<string>(phrase);
          newPhrase[newPhrase.Count - 1] += c;
          FindAnagram(newPhrase, newLetters);
        }
        if (FindWord(str)) { //current word in phrase is a word and should therefore be spaced
          List<string> newPhrase = new List<string>(phrase);
          string temp = newPhrase.Last();
          if (temp[0] < c) { //Prevents permutations of a phrase. If an anagram is found we will test its permutations
            newPhrase.Add("" + c);
            FindAnagram(newPhrase, newLetters);
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
