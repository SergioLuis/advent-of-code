#!/usr/bin/env dotnet-script

var inputPath = Path.GetFullPath("input.txt");
if (Args?.Count > 0 && File.Exists(Args[0]))
    inputPath = Args[0];

List<int> input = ReadNumbers(inputPath).ToList();

CalculatePerfectPoint(
    input,
    GetTotalFuelForPosition_Solution1,
    out int minimumTotalFuel,
    out int minimumTotalFuelPosition);

Console.WriteLine($"Solution 1: {minimumTotalFuel}");

CalculatePerfectPoint(
    input,
    GetTotalFuelForPosition_Solution2,
    out minimumTotalFuel,
    out minimumTotalFuelPosition);

Console.WriteLine($"Solution 2: {minimumTotalFuel}");

void CalculatePerfectPoint(
    List<int> horizontalPositions,
    Func<List<int>, int, int> calculateTotalFuelForPosition,
    out int minimumTotalFuel,
    out int minimumTotalFuelPosition)
{
    int max = horizontalPositions.Max();
    int min = horizontalPositions.Min();

    minimumTotalFuel = int.MaxValue;
    minimumTotalFuelPosition = -1;

    for (int i = min; i <= max; i++)
    {
        int fuelForPosition = calculateTotalFuelForPosition(horizontalPositions, i);
        if (fuelForPosition > minimumTotalFuel)
            continue;

        minimumTotalFuel = fuelForPosition;
        minimumTotalFuelPosition = i;
    }
}

int GetTotalFuelForPosition_Solution1(List<int> horizontalPositions, int position)
{
    int totalFuel = 0;
    for (int i = 0; i < horizontalPositions.Count; i++)
    {
        totalFuel += Math.Abs(horizontalPositions[i] - position);
    }

    return totalFuel;
}

int GetTotalFuelForPosition_Solution2(List<int> horizontalPositions, int position)
{
    int totalFuel = 0;
    for (int i = 0; i < horizontalPositions.Count; i++)
    {
        totalFuel += Enumerable.Range(1, Math.Abs(horizontalPositions[i] - position)).Sum();
    }

    return totalFuel;
}

IEnumerable<int> ReadNumbers(string path)
{
    using FileStream fs = File.OpenRead(path);
    using StreamReader sr = new(fs);

    int currentNumber = -1;
    while (!sr.EndOfStream)
    {
        int c = sr.Read();
        if (c == (int)',' || c == (int)'\r' || c == (int)'\n')
        {
            yield return currentNumber;
            currentNumber = -1;
            continue;
        }

        if (currentNumber == -1)
        {
            currentNumber = c - (int)'0';
            continue;
        }

        currentNumber *= 10;
        currentNumber += c - (int)'0';
    }
}
