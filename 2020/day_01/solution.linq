<Query Kind="Program" />

void Main(string[] args)
{
	var inputPath = @".\input.txt";
	if (args?.Length > 0 && File.Exists(args[0]))
		inputPath = args[0];
		
	List<int> numbers = ReadLines(inputPath)
		.Select(int.Parse)
		.ToList();

	int number = numbers
		.Where(n => numbers.Contains(2020 - n))
		.First();
	
	(number * (2020 - number)).Dump("Solution 1");
	
	foreach (int x in numbers)
	{
		int r = numbers
			.Where(y => numbers.Contains(2020 - y - x))
			.FirstOrDefault();
		
		if (r == default)
			continue;
			
		(x * r * (2020 - x - r)).Dump("Solution 2");
		break;
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
