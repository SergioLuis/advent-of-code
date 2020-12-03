#!/usr/bin/env dotnet-script

using System.Linq;

var inputPath = Path.GetFullPath("input.txt");
if (Args?.Count > 0 && File.Exists(Args[0]))
    inputPath = Args[0];

List<int> adapters = ReadLines(inputPath).Select(l => int.Parse(l)).ToList();
adapters.Sort();

List<int> jolts = new(adapters);
jolts.Insert(0, 0); // Power outlet 0 jolts
jolts.Add(jolts[jolts.Count - 1] + 3); // Built in adapter 3 higher than highets charger

Dictionary<int, int> ratingDifferenceCount = Enumerable
    .Range(0, jolts.Count - 1)
    .Select(i => jolts[i + 1] - jolts[i])
    .GroupBy(rating => rating)
    .ToDictionary(g => g.Key, g => g.Count());

Console.WriteLine($"Solution 1: {ratingDifferenceCount[1] * ratingDifferenceCount[3]}");

Dictionary<int, Adapter> indexedAdapters = jolts
    .Select(jolts => new Adapter(jolts))
    .GroupBy(adapter => adapter.Rating)
    .ToDictionary(g => g.Key, g => g.First());

foreach (var kvp in indexedAdapters)
{
    Adapter adapter = kvp.Value;
    for (int i = adapter.Rating + 1; i <= adapter.Rating + 3; i++)
    {
        Adapter possibleConnection;
        if (indexedAdapters.TryGetValue(i, out possibleConnection))
            adapter.PossibleConnections.Add(possibleConnection);
    }
}

Console.WriteLine($"Solution 2: {indexedAdapters[0].GetPathsToPhone()}");

class Adapter
{
    public int Rating { get; set; }
    public List<Adapter> PossibleConnections { get; set; } = new();

    public Adapter(int rating)
    {
        Rating = rating;
    }

    public long GetPathsToPhone()
    {
        if (PossibleConnections.Count == 0)
            return 1;

        if (mPathsToPhone != -1)
            return mPathsToPhone;

        mPathsToPhone = PossibleConnections.Select(adapter => adapter.GetPathsToPhone()).Sum();
        return mPathsToPhone;
    }

    long mPathsToPhone = -1;
}

IEnumerable<string> ReadLines(string path)
{
    using FileStream fs = File.OpenRead(path);
    using StreamReader sr = new StreamReader(fs);

    string line;
    while ((line = sr.ReadLine()) != null)
        yield return line;
}
