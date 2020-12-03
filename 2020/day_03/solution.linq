<Query Kind="Program" />

void Main(string[] args)
{
	var inputPath = @".\input.txt";
	
	int index = 0;
	ReadLines(inputPath)
		.Select(line => 
		{
			char result = line[index];
			index = (index + 3) % line.Length;
			return result;
		})
		.Count(c => c == '#')
		.Dump("Solution 1");

	new List<(int right, int down)>
	{
		(right: 1, down: 1),
		(right: 3, down: 1),
		(right: 5, down: 1),
		(right: 7, down: 1),
		(right: 1, down: 2)
	}.Select(slope => {
		int index = 0;
		return ReadLines(inputPath)
			.Where((_, i) => i % slope.down == 0)
			.Select(line => 
			{
				char result = line[index];
				index = (index + slope.right) % line.Length;
				return result;
			})
			.Count(c => c == '#');
	})
	.Aggregate(1, (acc, x) => acc * x)
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

