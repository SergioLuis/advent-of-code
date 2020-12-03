<Query Kind="Program" />

void Main(string[] args)
{
	var inputPath = @".\input.txt";

	var policyRegex = new Regex(
		"(?<n0>[0-9]+)-(?<n1>[0-9]+) (?<c>[a-z]): (?<pwd>[a-z]+)",
		RegexOptions.Compiled);
		
	Func<(int minTimes, int maxTimes, char chr, string pwd), bool> firstPolicy = (t) =>
		Enumerable.Range(t.minTimes, t.maxTimes - t.minTimes + 1).Contains(t.pwd.Count(c => c == t.chr));
	
	Func<(int firstIdx, int secondIdx, char chr, string pwd), bool> secondPolicy = (t) =>
		t.pwd[t.firstIdx] == t.chr ^ t.pwd[t.secondIdx] == t.chr;
	
	ReadLines(inputPath)
		.Select(l => policyRegex.Matches(l)[0].Groups)
		.Select(g => (
			int.Parse(g["n0"].Value),
			int.Parse(g["n1"].Value),
			g["c"].Value[0],
			g["pwd"].Value))
		.Where(firstPolicy)
		.Count()
		.Dump("Solution 1");

	ReadLines(inputPath)
		.Select(l => policyRegex.Matches(l)[0].Groups)
		.Select(g => (
			int.Parse(g["n0"].Value) - 1,
			int.Parse(g["n1"].Value) - 1,
			g["c"].Value[0],
			g["pwd"].Value))
		.Where(secondPolicy)
		.Count()
		.Dump("Solution 2");
}

IEnumerable<string> ReadLines(string path)
{
	using FileStream fs = File.OpenRead(path);
	using StreamReader sr = new StreamReader(fs);
		
	string line;
	while ((line = sr.ReadLine()) != null)
		yield return line;
}
