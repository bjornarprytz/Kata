using Kata.Trigrams;

const string crimeAndPunishment = "https://www.gutenberg.org/files/2554/2554-0.txt";
var client = new HttpClient();
var trigrams = new Trigrams();

await using var bookStream = await client.GetStreamAsync(crimeAndPunishment);

var reader = new StreamReader(bookStream);

var corpus = await reader.ReadToEndAsync();

trigrams.Analyse(corpus);

Console.WriteLine(trigrams.PredictNWordNovelFrom(20, "To", "begin"));
Console.WriteLine(trigrams.PredictNWordNovelFrom(100, "It", "was"));
Console.WriteLine(trigrams.PredictNWordNovelFrom(100, "He", "had"));
Console.WriteLine(trigrams.PredictNWordNovelFrom(100, "At", "last"));