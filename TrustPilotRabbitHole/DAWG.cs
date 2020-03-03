using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TrustPilotRabbitHole
{
  class DAWG
  {
    internal Node root;

    public DAWG(string path)
    {
      root = new Node('\0', null);
      List<string> words = File.ReadAllLines(path).ToList();
      Create(words);
    }

    //Add assumed words to parameter
    public string FindAnagrams(string anagram)
    {
      List<char> inp = String.Concat(anagram.OrderBy(c => c)).ToList();
      inp.RemoveAll(item => item ==' ');

     var s = FindAnagram(new List<string>() { "" }, inp);
      foreach (string i in s) {
        Console.WriteLine(i);
      }

      return anagram;
    }
    private List<string> FindAnagram(List<string> phrase, List<char> letters)
    {
      if (letters.Count == 0 && FindWord(phrase[phrase.Count - 1])) {
        foreach (string s in phrase) {
          Console.Write(s + " ");
        }
        Console.WriteLine("");
        return phrase;
      }

      //Over distint letters or else I will end up with duplicates
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
          if (temp[0] < c) { //Prevents permutations of a phrase
            newPhrase.Add("" + c);
            FindAnagram(newPhrase, newLetters);
          }
        }
      }
      return null;
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

    //Test traversal method
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
