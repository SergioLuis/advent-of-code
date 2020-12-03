<Query Kind="Program" />

void Main(string[] args)
{
	var inputPath = @".\input.txt";
	if (args?.Length > 0 && File.Exists(args[0]))
		inputPath = args[0];
		
	var parentBagRuleRegexStr = @"(?<bagColor>[a-zA-Z ]+) bags contain";
	var childBagRuleRegexStr = @"(?<number>[0-9])( (?<childBagColor>[a-zA-Z ]+) bag[s]?[,\.])";
	
	var parentBagRuleRegex = new Regex(
		parentBagRuleRegexStr, RegexOptions.Compiled);
	var childBagRuleRegex = new Regex(
		childBagRuleRegexStr, RegexOptions.Compiled);
		
	Func<string, string> extractParentBag = (str) =>
		parentBagRuleRegex.Matches(str)[0].Groups["bagColor"].Value;

	Func<string, List<(int count, string color)>> extractChildBags = (str) =>
	{
		var result = new List<(int count, string color)>();
		foreach (Match match in childBagRuleRegex.Matches(str))
		{
			result.Add((
				int.Parse(match.Groups["number"].Value),
				match.Groups["childBagColor"].Value));
		}
		
		return result;
	};

	var rulesFromTop = ReadLines(inputPath)
		.Select(l => (extractParentBag(l), extractChildBags(l)))
		.ToList();
		
	var indexedRulesFromTop = new Dictionary<string, List<(int, string)>>();
	foreach (var rule in rulesFromTop)
	{
		indexedRulesFromTop.Add(rule.Item1, rule.Item2);
	}

	var indexedRulesFromBottom = new Dictionary<string, List<string>>();
	foreach (var rule in rulesFromTop)
	{
		foreach (var childBag in rule.Item2)
		{
			List<string> canBeContainedOn;
			if (!indexedRulesFromBottom.TryGetValue(childBag.Item2, out canBeContainedOn))
			{
				canBeContainedOn = new List<string>();
				indexedRulesFromBottom.Add(childBag.Item2, canBeContainedOn);
			}
			
			canBeContainedOn.Add(rule.Item1);
		}
	}
	
	Func<string, int> getNumberOfBagsCanContainColor = (color) =>
	{
		int result = 0;
		var pendingColors = new Stack<string>();
		pendingColors.Push(color);
		
		var alreadyProcessed = new HashSet<string>();
		
		while (pendingColors.Count > 0)
		{
			var currentColor = pendingColors.Pop();
			if (!indexedRulesFromBottom.ContainsKey(currentColor))
				continue;

			foreach (string parentColor in indexedRulesFromBottom[currentColor])
			{
				if (alreadyProcessed.Contains(parentColor))
					continue;
				
				alreadyProcessed.Add(parentColor);
				result++;
				pendingColors.Push(parentColor);
			}
		}
		
		return result;
	};
	
	getNumberOfBagsCanContainColor("shiny gold")
		.Dump("Solution 1");
		
	Func<string, int> getNumberOfBagsColorMustContain = (color) =>
	{
		var pendingColors = new Stack<string>();
		foreach (var childBags in indexedRulesFromTop[color])
			for (int i = 0; i < childBags.Item1; i++)
				pendingColors.Push(childBags.Item2);
		
		var result = 0;
		while (pendingColors.Count > 0)
		{
			result++;
			string currentBag = pendingColors.Pop();
			
			if (!indexedRulesFromTop.ContainsKey(currentBag))
				continue;
				
			foreach (var childBags in indexedRulesFromTop[currentBag])
				for (int i = 0; i < childBags.Item1; i++)
					pendingColors.Push(childBags.Item2);
		}
		
		return result;
	};
	
	getNumberOfBagsColorMustContain("shiny gold").Dump("Solution 2");
}

IEnumerable<string> ReadLines(string path)
{
	using FileStream fs = File.OpenRead(path);
	using StreamReader sr = new StreamReader(fs);
		
	string line;
	while ((line = sr.ReadLine()) != null)
		yield return line;
}
