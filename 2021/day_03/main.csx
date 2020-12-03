#!/usr/bin/env dotnet-script

using System.Text;

var inputPath = Path.GetFullPath("input.txt");
if (Args?.Count > 0 && File.Exists(Args[0]))
    inputPath = Args[0];

List<string> input = ReadLines(inputPath).ToList();

int length = input[0].Length;

StringBuilder gammaStr = new(length);
StringBuilder epsilonStr = new(length);

for (int i = 0; i < length; i++)
{
    Dictionary<char, int> appearances = input
        .Select(number => number[i])
        .GroupBy(c => c)
        .ToDictionary(g => g.Key, g => g.Count());

    (char mostCommon, char leastCommon) =
        appearances['0'] > appearances['1'] ? ('0', '1') : ('1', '0');

    gammaStr.Append(mostCommon);
    epsilonStr.Append(leastCommon);
}

int gamma = Convert.ToInt32(gammaStr.ToString(), 2);
int epsilon = Convert.ToInt32(epsilonStr.ToString(), 2);

Console.WriteLine($"Solution 1: {gamma * epsilon}");

List<string> oxygenGeneratorRatingCandidates = new(input);
List<string> co2ScrubberRatingCandidates = new(input);

int bitIndex = 0;
while (oxygenGeneratorRatingCandidates.Count > 1 && bitIndex < length)
{
    oxygenGeneratorRatingCandidates =
        FilterNumbers(oxygenGeneratorRatingCandidates, bitIndex++, true, '1');
}

bitIndex = 0;
while (co2ScrubberRatingCandidates.Count > 1 && bitIndex < length)
{
    co2ScrubberRatingCandidates =
        FilterNumbers(co2ScrubberRatingCandidates, bitIndex++, false, '0');
}

int oxygenGeneratorRating = Convert.ToInt32(oxygenGeneratorRatingCandidates.First(), 2);
int co2ScrubberRating = Convert.ToInt32(co2ScrubberRatingCandidates.First(), 2);

Console.WriteLine($"Solution 2: {oxygenGeneratorRating * co2ScrubberRating}");

List<string> FilterNumbers(
    List<string> input,
    int bitIndex,
    bool useMostCommon,
    char predominantChar)
{
    Dictionary<char, int> bitAppearances = input
        .Select(number => number[bitIndex])
        .GroupBy(c => c)
        .ToDictionary(g => g.Key, g => g.Count());
    
    (char mostCommon, char leastCommon) = bitAppearances['0'] > bitAppearances['1']
        ? ('0', '1')
        : bitAppearances['0'] < bitAppearances['1']
            ? ('1', '0')
            : (predominantChar, predominantChar);

    return input
        .Where(n => n[bitIndex] == (useMostCommon ? mostCommon : leastCommon))
        .ToList();
}

IEnumerable<string> ReadLines(string path)
{
    using FileStream fs = File.OpenRead(path);
    using StreamReader sr = new StreamReader(fs);

    string line;
    while ((line = sr.ReadLine()) != null)
        yield return line;
}
