#!/usr/bin/env dotnet-script

var inputPath = Path.GetFullPath("input.txt");
if (Args?.Count > 0 && File.Exists(Args[0]))
    inputPath = Args[0];

List<InputLine> input = ReadLines(inputPath).Select(InputLine.Parse).ToList();

int firstSolution = input
    .Select(line => line.Output)
    .SelectMany(o => o)
    .Count(output => output.Length == 2
        || output.Length == 4
        || output.Length == 3
        || output.Length == 7);

Console.WriteLine($"Solution 1: {firstSolution}");

int secondSolution = input
    .Select(line => line.CalculateOutput(line.MatchSegments()))
    .Sum();

Console.WriteLine($"Solution 2: {secondSolution}");

class InputLine
{
    public List<string> SignalPatterns { get; private set; } = new();
    public List<string> Output { get; private set; } = new();

    public static InputLine Parse(string line)
    {
        string[] parts = line.Split('|', StringSplitOptions.RemoveEmptyEntries);

        InputLine result = new();
        result.SignalPatterns.AddRange(parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries));
        result.Output.AddRange(parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries));

        return result;
    }

    public int CalculateOutput(Dictionary<string, int> segments)
    {
        int result = 0;
        for (int i = 0; i < Output.Count; i++)
        {
            string key = String.Concat(Output[i].OrderBy(c => c));
            result *= 10;
            result += segments[key];
        }

        return result;
    }

    public Dictionary<string, int> MatchSegments()
    {
        Dictionary<string, int> result = new();

        string one = SignalPatterns.First(s => s.Length == 2);
        string four = SignalPatterns.First(s => s.Length == 4);

        foreach (string missing in SignalPatterns)
        {
            string signal = String.Concat(missing.OrderBy(c => c));
            
            if (signal.Length == 2)
            {
                result.Add(signal, 1);
                continue;
            }

            if (signal.Length == 3)
            {
                result.Add(signal, 7);
                continue;
            }

            if (signal.Length == 4)
            {
                result.Add(signal, 4);
                continue;
            }

            if (signal.Length == 5)
            {
                if (one.All(c => signal.Contains(c)))
                {
                    result.Add(signal, 3);
                    continue;
                }

                int matches = four.Where(c => signal.Contains(c)).Count();
                if (matches == 2)
                {
                    result.Add(signal, 2);
                    continue;
                }

                if (matches == 3)
                {
                    result.Add(signal, 5);
                    continue;
                }
            }

            if (signal.Length == 6)
            {
                if (four.All(c => signal.Contains(c)))
                {
                    result.Add(signal, 9);
                    continue;
                }

                if (one.All(c => signal.Contains(c)))
                {
                    result.Add(signal, 0);
                    continue;
                }

                result.Add(signal, 6);
                continue;
            }

            if (signal.Length == 7)
            {
                result.Add(signal, 8);
                continue;
            }

            throw new InvalidOperationException();
        }

        return result;
    }
}

IEnumerable<string> ReadLines(string path)
{
    using FileStream fs = File.OpenRead(path);
    using StreamReader sr = new(fs);

    string line;
    while ((line = sr.ReadLine()) != null)
        yield return line;
}
