#!/usr/bin/env dotnet-script

var inputPath = Path.GetFullPath("input.txt");
if (Args?.Count > 0 && File.Exists(Args[0]))
    inputPath = Args[0];

List<int> input = ReadChars(inputPath).Select(c => c - (int)'0').ToList();

LanternfishSchool school80Days = new();
input.ForEach(school80Days.AddFish);

school80Days.EvolveSchoolNDays(80);
Console.WriteLine($"Solution 1: {school80Days.GetSchoolSize()}");

LanternfishSchool school256Days = new();
input.ForEach(school256Days.AddFish);

school256Days.EvolveSchoolNDays(256);
Console.WriteLine($"Solution 2: {school256Days.GetSchoolSize()}");



class LanternfishSchool
{
    public LanternfishSchool()
    {
        for (int i = 0; i < 9; i++)
            mSchoolIndexedByTimer[i] = 0;
    }

    public void AddFish(int internalTimer)
    {
        mSchoolIndexedByTimer[internalTimer]++;
    }

    public void EvolveSchoolNDays(int days)
    {
        for (int i = 0; i < days; i++)
            EvolveSchoolOneDay();
    }

    public void EvolveSchoolOneDay()
    {
        long dayZero = mSchoolIndexedByTimer[0];

        for (int i = 0; i < 8; i++)
            mSchoolIndexedByTimer[i] = mSchoolIndexedByTimer[i+1];

        mSchoolIndexedByTimer[8] = dayZero;
        mSchoolIndexedByTimer[6] += dayZero;
    }

    public long GetSchoolSize() => mSchoolIndexedByTimer.Select(kvp => kvp.Value).Sum();

    readonly Dictionary<int, long> mSchoolIndexedByTimer = new();
}

IEnumerable<int> ReadChars(string path)
{
    using FileStream fs = File.OpenRead(path);
    using StreamReader sr = new(fs);

    while (!sr.EndOfStream)
    {
        int c = sr.Read();
        if (c == (int)',' || c == (int)'\r' || c == (int)'\n')
            continue;

        yield return c;
    }
}
