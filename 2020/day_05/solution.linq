<Query Kind="Program" />

void Main(string[] args)
{
	var inputPath = @".\input.txt";
	if (args?.Length > 0 && File.Exists(args[0]))
		inputPath = args[0];

	Func<(int l, int u), char, (int l, int u)> updateRange = (r, c) =>
		c switch
		{
			var x when x is 'F' || x is 'L' =>
				(r.l, r.l + (r.u - r.l) / 2),

			var x when x is 'B' || x is 'R' =>
				(r.l + (r.u - r.l) / 2 + 1, r.u),

			_ => (-1, -1)
		};
		
	Func<string, int> getSeatId = (boardingPass) =>
	{
		(int lower, int _) rowRange = (0, 127);
		boardingPass
			.Take(7)
			.Select(c => c)
			.ToList()
			.ForEach(c => rowRange = updateRange(rowRange, c));

		(int lower, int _) columnRange = (0, 7);
		boardingPass
			.Skip(7)
			.Select(c => c)
			.ToList()
			.ForEach(c => columnRange = updateRange(columnRange, c));

		return rowRange.lower * 8 + columnRange.lower;
	};
	
	List<int> seatIds = ReadLines(inputPath)
		.Select(getSeatId)
		.ToList();
	
	int minSeatId = seatIds.Min();
	int maxSeatId = seatIds.Max();
	var seatIdsSet = new HashSet<int>(seatIds);
	
	maxSeatId.Dump("Solution 1");
	Enumerable
		.Range(minSeatId, maxSeatId - minSeatId)
		.Where(x => !seatIdsSet.Contains(x))
		.Where(x => seatIdsSet.Contains(x - 1) && seatIdsSet.Contains(x + 1))
		.First()
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
