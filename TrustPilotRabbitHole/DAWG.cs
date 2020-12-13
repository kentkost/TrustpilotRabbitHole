using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Concurrent;

namespace TrustPilotRabbitHole
{
    class DAWG
    {
        internal Node root;
        private Dictionary<string, HashSet<string>> map = new Dictionary<string, HashSet<string>>();
        private LetterFrequencyMatrix rankWords;
        public List<string> anagrams = new List<string>();
        private ConcurrentQueue<string> guessWords = new ConcurrentQueue<string>();
        public List<string> hashes = new List<string>() {
            "e4820b45d2277f3844eac66c903e84be",
            "23170acc097c24edb98fc5488ab033fe",
            "665e5bcb0c20062fe8abaaf4628bb154"
        };

        private DateTime then;

        //delete later
        static int numOfThreads = 3;

        public DAWG(string path, string anagram)
        {
            root = new Node('\0', null);
            if (!File.Exists(path))
            {
                Console.Write("Wordlist not found. Downloading it ");
                try
                {
                    GetWordList(path);
                    Console.WriteLine("\u221A");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Download failed. Aborting!");
                    Console.WriteLine(ex.Message);
                }
            }

            Console.Write("Excluding impossible words ");
            string newWordsFileName = CreateNewWords(path, anagram);
            Console.WriteLine("\u221A");

            Console.Write("Finding and ranking guesses ");
            rankWords = new LetterFrequencyMatrix(newWordsFileName, anagram);
            guessWords = new ConcurrentQueue<string>(rankWords.guessWords);
            Console.WriteLine("\u221A");

            Console.Write("Create directed acyclic word graph ");
            List<string> words = File.ReadAllLines(newWordsFileName).ToList();
            Create(words);
            Console.WriteLine("\u221A");

            Console.WriteLine("Searching for anagrams: ");
            then = DateTime.Now;
            //Start(anagram);
            ThreadedStart(anagram);
        }

        private void ThreadedStart(string anagram)
        {

            for (int i = 0; i < numOfThreads; i++)
            {
                ThreadStart ts = delegate
                {
                    Start(anagram);
                };
                Thread t = new Thread(ts);
                t.Name = "Thread " + i;
                t.Start();
            }
        }

        private void Start(string anagram)
        {
            while (!guessWords.IsEmpty)
            {
                string guess = "";
                if (hashes.Count == 0)
                {
                    Console.WriteLine("Found all hashes");
                    break;
                }
                if (guessWords.TryDequeue(out guess))
                {
                    this.FindAnagrams(anagram, guess);
                }
            }
        }

        private void GetWordList(string path)
        {
            string remoteUri = "https://followthewhiterabbit.trustpilot.com/cs/wordlist";
            using (var client = new WebClient())
            {
                client.DownloadFile(remoteUri, path);
            }
        }

        private void CheckAnagram(List<string> phrase)
        {
            List<List<string>> phrases = new List<List<string>>();
            Combinations(phrase, 0, phrases);

            foreach (List<string> phr in phrases)
            {
                List<string> permsPhrase = Permutations(phr);

                foreach (string s in permsPhrase)
                {
                    string hash = MD5hash(s);

                    for (int i = 0; i < hashes.Count; i++)
                    {
                        if (hash == hashes[i])
                        {
                            var diffInSeconds = (DateTime.Now - then).TotalSeconds;
                            Console.Write(diffInSeconds);
                            Console.WriteLine("\t" + s + " : " + hash);
                            hashes.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        private string CreateNewWords(string path, string input)
        {
            string fileName = "newWords.txt";
            using (StreamWriter outfile = new StreamWriter(fileName))
            using (StreamReader file = new StreamReader(path))
            {
                string ln;
                string prev = "";
                string temp = "";

                while ((ln = file.ReadLine()) != null)
                {
                    temp = NormalizeString(ln);
                    if (IsSubsetString(input, temp) && ln.Length > 1)
                    {
                        if (!IdenticalStrings(ln, prev))
                        {
                            outfile.WriteLine(temp);
                            prev = ln;
                        }
                        else
                        {
                            string normPrev = NormalizeString(prev);
                            if (!map.ContainsKey(normPrev))
                            {
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
            return fileName;
        }

        private string NormalizeString(string str)
        {
            return Regex.Replace(str.Normalize(NormalizationForm.FormD), @"[^a-z]", "");
        }

        private bool IdenticalStrings(string str1, string str2)
        {
            str1 = NormalizeString(str1);
            str2 = NormalizeString(str2);
            return str2 == str1;
        }

        private bool IsSubsetString(string input, string compare)
        {
            //remove whitespace
            input = Regex.Replace(input, @"\s+", "");
            compare = Regex.Replace(compare, @"\s+", "");

            List<char> inp = String.Concat(input.OrderBy(c => c)).ToList();

            foreach (char c in compare)
            {
                bool found = inp.Remove(c);
                if (!found)
                {
                    return false;
                }
            }

            return true;
        }

        public void Combinations(List<string> sentence, int i, List<List<string>> results)
        {
            List<List<string>> combo = new List<List<string>>();
            if (i < sentence.Count && map.ContainsKey(sentence[i]))
            {

                foreach (string s in map[sentence[i]])
                {
                    List<string> temp = new List<string>(sentence);
                    temp[i] = s;
                    combo.Add(temp);
                }

                foreach (List<string> ls in combo)
                {
                    Combinations(new List<string>(ls), i + 1, results);
                }

            }
            else
            {
                results.Add(sentence);
            }
        }

        public string MD5hash(string s)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(s);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString().ToLower();
        }

        private List<char> SubtractLetters(List<char> anagram, string guess)
        {
            foreach (char c in guess)
            {
                anagram.Remove(c);
            }
            return anagram;
        }

        public void FindAnagramsThreaded(List<string> phrase, string anagram, string guess)
        {
            List<char> inp = String.Concat(anagram.OrderBy(c => c)).ToList();
            inp.RemoveAll(item => item == ' ');
            inp = SubtractLetters(inp, guess);
            FindAnagram(new List<string>() { "" }, inp, guess);
        }

        public void FindAnagrams(string anagram, string guess)
        {
            List<char> inp = String.Concat(anagram.OrderBy(c => c)).ToList();
            inp.RemoveAll(item => item == ' ');
            inp = SubtractLetters(inp, guess);
            FindAnagram(new List<string>() { "" }, inp, guess);
        }

        private void FindAnagram(List<string> phrase, List<char> letters, string guess)
        {
            if (letters.Count == 0 && FindWord(phrase[phrase.Count - 1]))
            {
                List<string> newPhrase = new List<string>(phrase);
                newPhrase.Add(guess);
                CheckAnagram(newPhrase);
                return;
            }

            //Over distinct letters or else it will end up with duplicate branches
            foreach (char c in letters.Distinct())
            {
                string str = phrase.Last();
                List<char> newLetters = new List<char>(letters);
                newLetters.Remove(c);

                if (WordIsPossible(str + c))
                {
                    List<string> newPhrase = new List<string>(phrase);
                    newPhrase[newPhrase.Count - 1] += c;
                    FindAnagram(newPhrase, newLetters, guess);
                }

                if (FindWord(str))
                { //current word in phrase is a word and should therefore be spaced
                    List<string> newPhrase = new List<string>(phrase);
                    string temp = newPhrase.Last();
                    if (temp[0] < c)
                    { //Prevents permutations of a phrase. If an anagram is found we will find its permutations
                        newPhrase.Add("" + c);
                        FindAnagram(newPhrase, newLetters, guess);
                    }
                }
            }
        }

        public bool WordIsPossible(string word)
        {
            Node current = root;
            foreach (char c in word)
            {
                current = current.FindChild(c);
                if (current == null)
                {
                    return false;
                }
            }
            return true;
        }

        public bool FindWord(string word)
        {
            Node current = root;
            foreach (char c in word)
            {
                current = current.FindChild(c);
                if (current == null)
                {
                    return false;
                }
            }

            if (current.IsWord)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool ShouldSwap(List<string> phr, int start, int curr)
        {
            for (int i = start; i < curr; i++)
            {
                if (phr[i] == phr[curr])
                {
                    return false;
                }
            }
            return true;
        }

        private static List<string> Permutations(List<string> phrase)
        {
            List<string> results = new List<string>();
            int n = phrase.Count;
            Permute(phrase, 0, n - 1, ref results);
            return results;
        }

        private static void Permute(List<string> phrase, int l, int r, ref List<string> results)
        {
            if (l == r)
            {
                results.Add(String.Join(" ", phrase.ToArray()));
            }
            else
            {
                for (int i = l; i <= r; i++)
                {
                    bool check = ShouldSwap(phrase, l, i);
                    if (check)
                    {
                        phrase = Swap(phrase, l, i);
                        Permute(phrase, l + 1, r, ref results);
                        phrase = Swap(phrase, l, i);
                    }
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

            for (int i = 0; i < word.Length; i++)
            {
                Node child = current.FindChild(word[i]);

                if (child == null)
                {
                    current = current.AddChild(word[i]);
                }
                else
                {
                    current = child;
                }

                //Statement inside is only ever executed once
                if (!current.IsWord)
                {
                    current.IsWord = (i == word.Length - 1);
                }
            }
        }

        private void Create(List<string> words)
        {
            foreach (string s in words)
            {
                Insert(s);
            }
        }
    }
}
