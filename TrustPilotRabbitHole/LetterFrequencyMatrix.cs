using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TrustPilotRabbitHole
{
  class LetterFrequencyMatrix
  {
    Dictionary<char, LetterVector> letterFrequencies = new Dictionary<char,LetterVector>();
    int totalWords = 0;
    private List<WordScore> wordsScored = new List<WordScore>();
    public List<string> guessWords = new List<string>();

    public LetterFrequencyMatrix(string path, string anagram)
    {
      InitializeLetterFreqs(anagram);
      Calculate(path);
      foreach (KeyValuePair<char, LetterVector> kvp in letterFrequencies) {
        kvp.Value.CalculateScore(totalWords);
      }
      ScoreWords(path);

      wordsScored = wordsScored.OrderByDescending(x => x.Score).ToList();

      double tempScore = 0.0f;
      char letterLowestFreq='\0';
      foreach (KeyValuePair<char, LetterVector> kvp in letterFrequencies) {
        Console.WriteLine(kvp.Key + " : " + kvp.Value.idf);
        if (kvp.Value.idf > tempScore) {
          tempScore = kvp.Value.idf;
          letterLowestFreq = kvp.Key;
        }
      }

      foreach (WordScore ws in wordsScored) {
        if (ws.Score > tempScore){
          guessWords.Add(ws.Word);
        }
      }
    }

    private void ScoreWords(string path)
    {
      List<string> words = File.ReadAllLines(path).ToList();
      foreach (string s in words) {
        double score = 0.0f;
        foreach (char c in s) {
          score += letterFrequencies[c].idf;
        }
        wordsScored.Add(new WordScore(s, score));
      }
    }

    private void InitializeLetterFreqs(string anagram)
    {
      List<char> inp = String.Concat(anagram.OrderBy(c => c)).ToList();
      inp.RemoveAll(item => item == ' ');
      foreach (char c in inp.Distinct()) {
        letterFrequencies.Add(c, new LetterVector(c));
      }
    }

    private void Calculate(string path)
    {
      List<string> words = File.ReadAllLines(path).ToList();
      totalWords = words.Count;
      for (int id = 0; id < words.Count; id++) {
        foreach (char c in words[id]) {
          LetterVector lv = letterFrequencies[c];
          lv.frequency++;
          lv.wordFrequency.Add(id);
        }
      }
    }
  }

  class LetterVector
  {
    public int frequency = 0;
    public char letter;
    public HashSet<int> wordFrequency = new HashSet<int>();
    public double idf;

    public LetterVector(char c)
    {
      letter = c;
    }

    public void CalculateScore(int totalWords)
    {
      idf = Math.Log10(totalWords / wordFrequency.Count);
    }
  }

  class WordScore
  {
    private double score;
    private string word;
    public double Score { get => score;}
    public string Word { get => word;}

    public WordScore(string w, double s)
    {
      word = w;
      score = s;
    }
  }
}
