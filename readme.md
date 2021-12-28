# TrustPilot Rabbit Hole Challenge


### Before running

Please don't use this as a an attempt to get a job at Trustpilot. 
I just thought I'd sahre my solution after having kept it private for some years 
and it doesn't seem like they hire based on this challenge anymore. So don't waste 
their and your own time trying something so foolish.


### How to run it

- Just run as if. It will download the wordlist from the web if it doesn't exist.
- If it fails try downloading the wordlist and put in the same folder as the executable
- All anagrams should be found within a minute


### How it works

First it will exclude all the impossible words, that can't be part of the anagram.

From the remaining words it will create a list of guesses. Where longer words and words containing letters that are less occurent will have be prioritized.

It will then create a directed acyclic word graph. This makes it easier and faster to check if a constructed word exists.

It will then search for anagrams by continously constructing words using the letters from the initial anagram. It will only construct anagrams where the words are alphabetically ordered.

The words must be alphabetically ordered since it reduced a lot of double work for the program. Since we don't want to check if a sequence of words is anagram for a sequences that contain the same words.

Once an anagram has been found. The program will create all possible ways to rearrange the words in the anagram to create possible anagrams for that sequence of words. 

### How fast is it?

With an i5-7500. I usually get the first two hashes within 3 seconds. The last hash is around 15 seconds.
However when using multithreading the time is more than halved. (Oh yeah. Checkout the Multithreading branch)

### Extra

A nice feature of the program is that it doesn't consider special characters or diacritics to be their own letters. So this program will be able to find anagrams where words has an apostrophe or accented letters.

ie. "hello world" is an anagram for "hëllo wôrld"
