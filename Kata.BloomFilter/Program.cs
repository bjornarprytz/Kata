using Kata.BloomFilter;

const string wordListPath = @"E:\source\repos\Kata\wordlist.txt";

var filter = new BloomFilter(70000000, 10, Hash.Sha);

var words = File.ReadAllText(wordListPath).Split('\n');

foreach (var word in words)
{
    filter.Add(word);
}


filter.CheckWord("Aaliyah");
filter.CheckWord("flybys");
filter.CheckWord("ijalsdfjsd");
filter.CheckWord("asildnfla");
filter.CheckWord("foamiest");
filter.CheckWord("asildnflaasadas");
filter.CheckWord("asildnflaasisodjoa");
filter.CheckWord("asildnflaasisodjoa");

