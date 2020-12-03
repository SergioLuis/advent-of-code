#!/usr/bin/env dotnet-script

var inputPath = Path.GetFullPath("input.txt");
if (Args?.Count > 0 && File.Exists(Args[0]))
    inputPath = Args[0];

List<int> input = ReadLines(inputPath).Select(int.Parse).ToList();

int firstSolution = input
    .Select((elem, i) => (First: (i == 0 ? int.MaxValue : input[i - 1]), Second: elem))
    .Where(t => t.Second > t.First)
    .Count();

Console.WriteLine($"Solution 1: {firstSolution}");

List<int> slidingWindowSums = Enumerable
    .Range(0, input.Count - 2)
    .Select(i => input.Skip(i).Take(3).Sum())
    .ToList();

int secondSolution = slidingWindowSums
    .Select((elem, i) => (First: (i == 0 ? int.MaxValue : slidingWindowSums[i - 1]), Second: elem))
    .Where(t => t.Second > t.First)
    .Count();

Console.WriteLine($"Solution 2: {secondSolution}");

IEnumerable<string> ReadLines(string path)
{
    using FileStream fs = File.OpenRead(path);
    using StreamReader sr = new StreamReader(fs);

    string line;
    while ((line = sr.ReadLine()) != null)
        yield return line;
}
