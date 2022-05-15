// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Kata.Anagrams;

const string wordListPath = @"..\..\..\..\wordlist.txt";

var currentWorkingDir = Directory.GetCurrentDirectory();

var grouper = new AnagramsGrouper(File.ReadAllText(wordListPath));
var stopWatch = new Stopwatch();
stopWatch.Start();
grouper.Process();
stopWatch.Stop();

Console.WriteLine($"Processed in {stopWatch.ElapsedMilliseconds} ms");
Console.Write(grouper.DumpStats());

