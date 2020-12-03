<Query Kind="Program" />

void Main(string[] args)
{
	var inputPath = @".\input.txt";
	if (args?.Length > 0 && File.Exists(args[0]))
		inputPath = args[0];
		
	Func<string, HashSet<char>> groupYesAnswers = (str) =>
	{
		var result = new HashSet<char>();
		str
			.Select(c => c)
			.ToList()
			.ForEach(c => result.Add(c));

		return result;
	};
	
	var customsAnswers = ReadAnswerGroup(ReadLines(inputPath)).ToList();
		
	customsAnswers
		.Select(l => string.Join(string.Empty, l))
		.Select(groupYesAnswers)
		.Select(yesAnswers => yesAnswers.Count)
		.Sum()
		.Dump("Solution 1");
		
	Func<(int people, string answers), int> countEveryoneAnsweredYes = (group) =>
		group.answers
			.Select(c => c)
			.GroupBy(c => c)
			.Where(grouping => grouping.Count() == group.people)
			.Count();
		
	ReadAnswerGroup(ReadLines(inputPath))
		.Select(l => (l.Count, string.Join(string.Empty, l)))
		.Select(countEveryoneAnsweredYes)
		.Sum()
		.Dump("Solution 2");
}

IEnumerable<List<string>> ReadAnswerGroup(IEnumerable<string> lines)
{
	List<string> currentList = new List<string>();
	foreach (string line in lines)
	{
		if (!string.IsNullOrEmpty(line))
		{
			currentList.Add(line);
			continue;
		}
		
		yield return currentList;
		currentList = new List<string>();
	}
	
	yield return currentList;
}

IEnumerable<string> ReadLines(string path)
{
	using FileStream fs = File.OpenRead(path);
	using StreamReader sr = new StreamReader(fs);
		
	string line;
	while ((line = sr.ReadLine()) != null)
		yield return line;
}
