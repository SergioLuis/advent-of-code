#!/usr/bin/env dotnet-script

var inputPath = Path.GetFullPath("input.txt");
if (Args?.Count > 0 && File.Exists(Args[0]))
    inputPath = Args[0];

List<Command> input = ReadLines(inputPath)
    .Select(Command.Parse)
    .ToList();

Dictionary<Direction, int> magnitudes = input
    .GroupBy(c => c.Direction)
    .ToDictionary(g => g.Key, g => g.Select(g => g.Magnitude).Sum());


Console.WriteLine($"Solution 1: {magnitudes[Direction.forward] * (magnitudes[Direction.down] - magnitudes[Direction.up])}");

int horizontal = 0;
int depth = 0;
int aim = 0;

input.ForEach(command => 
{
    switch (command.Direction)
    {
        case Direction.down:
            aim += command.Magnitude;
            break;

        case Direction.up:
            aim -= command.Magnitude;
            break;

        case Direction.forward:
            horizontal += command.Magnitude;
            depth += (command.Magnitude * aim);
            break;
    }
});

Console.WriteLine($"Solution 2: {horizontal * depth}");

enum Direction
{
    forward,
    down,
    up
}

readonly struct Command
{
    public readonly Direction Direction { get; }
    public readonly int Magnitude { get; }

    public static Command Parse(string line)
    {
        string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return new(Enum.Parse<Direction>(parts[0]), int.Parse(parts[1]));
    }

    public Command(Direction direction, int magnitude)
    {
        Direction = direction;
        Magnitude = magnitude;
    }
}

IEnumerable<string> ReadLines(string path)
{
    using FileStream fs = File.OpenRead(path);
    using StreamReader sr = new StreamReader(fs);

    string line;
    while ((line = sr.ReadLine()) != null)
        yield return line;
}
