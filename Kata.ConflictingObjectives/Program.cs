


using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Kata.ConflictingObjectives;
const string wordListPath = @"..\..\..\..\wordlist.txt";
var words = File.ReadAllText(wordListPath);



var readableSW = new Stopwatch();

var readable = new Readable();
readableSW.Start();
readable.Process(words);
readableSW.Stop();


var fastSW = new Stopwatch();
var fast = new Fast();
fastSW.Start();
fast.Process(words);
fastSW.Stop();

var extensibleSW = new Stopwatch();
var extensible = new Extensible();
extensibleSW.Start();
extensible.Process(words, 6, 2);
extensibleSW.Stop();


Console.WriteLine($"Readable: {readableSW.ElapsedMilliseconds} ms");
Console.WriteLine($"Fast: {fastSW.ElapsedMilliseconds} ms");
Console.WriteLine($"Extensible: {extensibleSW.ElapsedMilliseconds} ms");