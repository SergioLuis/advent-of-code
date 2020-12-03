#!/usr/bin/env dotnet-script

var inputPath = Path.GetFullPath("input.txt");
if (Args?.Count > 0 && File.Exists(Args[0]))
    inputPath = Args[0];

const int PreambleLen = 25;

List<long> input = ReadLines(inputPath).Select(l => long.Parse(l)).ToList();

int firstIndexBreakingXmas = FindFirstIndexBreakingXmas(input, PreambleLen);
Console.WriteLine($"Solution 1: {input[firstIndexBreakingXmas]}");

List<long> contiguousSet = FindContiguousSet(input, firstIndexBreakingXmas);
Console.WriteLine($"Solution 2: {contiguousSet.Min() + contiguousSet.Max()}");

int FindFirstIndexBreakingXmas(List<long> input, int preambleLength)
{
    for (int i = 0; i < input.Count - PreambleLen; i++)
    {
        List<long> slidingWindow = input.Skip(i).Take(PreambleLen + 1).ToList();

        bool found = false;
        for (int j = 0; j < slidingWindow.Count - 2; j++)
        {
            long remainder = slidingWindow[PreambleLen] - slidingWindow[j];
            for (int k = j + 1; k < slidingWindow.Count - 1; k++)
            {
                if (slidingWindow[k] == remainder)
                {
                    found = true;
                    break;
                }
            }

            if (found)
                break;
        }

        if (found)
            continue;

        return i + preambleLength;
    }

    return -1;
}

List<long> FindContiguousSet(List<long> input, int corruptIndex)
{
    long offendingNumber = input[corruptIndex];
    for (int i = 0; i < corruptIndex - 1; i++)
    {
        for (int j = 2; j < corruptIndex - i; j++)
        {
            List<long> subset = input.Skip(i).Take(j).ToList();
            long sum = subset.Sum();
            if (sum == offendingNumber)
                return subset;

            if (sum > offendingNumber)
                break;
        }
    }

    return new List<long>(0);
}

IEnumerable<string> ReadLines(string path)
{
    using FileStream fs = File.OpenRead(path);
    using StreamReader sr = new StreamReader(fs);

    string line;
    while ((line = sr.ReadLine()) != null)
        yield return line;
}

