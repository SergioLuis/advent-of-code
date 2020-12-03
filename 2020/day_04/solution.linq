<Query Kind="Program" />

void Main(string[] args)
{
	var inputPath = @".\input.txt";
	if (args?.Length > 0 && File.Exists(args[0]))
		inputPath = args[0];
	
	var mandatoryFields = new List<string>()
	{
		"byr",
		"iyr",
		"eyr",
		"hgt",
		"hcl",
		"ecl",
		"pid"
	};
	
	Func<List<string>, Dictionary<string, string>> extractPassport = (passportLines) =>
	{
		var result = new Dictionary<string, string>();
		passportLines
			.ForEach(line =>
			{
				line
					.Split(' ', StringSplitOptions.RemoveEmptyEntries)
					.ToList()
					.ForEach(field =>
					{
						var parts = field.Split(':');
						result.Add(parts[0], parts[1]);
					});
			});
		
		return result;
	};
	
	
	Func<Dictionary<string, string>, bool> isValidPassport = (dict) =>
		mandatoryFields
			.Select(f => dict.ContainsKey(f))
			.Aggregate(true, (acc, x) => acc && x);
	
	ReadPassports(ReadLines(inputPath))
		.Select(extractPassport)
		.Count(isValidPassport)
		.Dump("Solution 1");
		
	Func<Dictionary<string, string>, string, Func<string, bool>, bool> isValidField =
		(data, field, validation) =>
	{
		string value;
		if (!data.TryGetValue(field, out value))
			return false;
			
		return validation(value);
	};
	
	Func<string, int, int, bool> isNumericInRange = (str, minValue, maxValue) =>
	{
		int n;
		if (!int.TryParse(str, out n))
			return false;
			
		return minValue <= n && n <= maxValue;
	};
	
	Func<string, bool> isValidBirthYear = (year) =>
		year.Length == 4 && isNumericInRange(year, 1920, 2002);
	
	Func<string, bool> isValidIssueYear = (issueYear) =>
		issueYear.Length == 4 && isNumericInRange(issueYear, 2010, 2020);
	
	Func<string, bool> isValidExpirationYear = (expirationYear) =>
		expirationYear.Length == 4 && isNumericInRange(expirationYear, 2020, 2030);
	
	Func<string, string> extractUnit = (height) =>
		height.Substring(height.Length - 2, 2);
		
	Func<string, string> extractHeight = (height) =>
		height.Substring(0, height.Length - 2);
	
	Func<string, bool> isValidHeight = (height) =>
		(extractUnit(height)) switch 
		{
			"cm" => isNumericInRange(extractHeight(height), 150, 193),
			"in" => isNumericInRange(extractHeight(height), 59, 76),
			_ => false
		};
		
	Func<string, bool> isValidHairColor = (hairColor) =>
		hairColor.Length == 7
		&& hairColor[0] == '#'
		&& hairColor
			.Skip(1)
			.Select(h => (h >= '0' && h <= '9') || (h >= 'a' && h <= 'f'))
			.Aggregate(true, (acc, x) => acc && x);
			
	var validEyeColors = new List<string>
	{
		"amb", "blu", "brn", "gry", "grn", "hzl", "oth"
	};
	
	Func<string, bool> isValidEyeColor = (eyeColor) =>
		validEyeColors.Contains(eyeColor);
		
	Func<string, bool> isValidPassportId = (passportId) =>
		passportId.Length == 9
		&& passportId.Select(c => c >= '0' && c <= '9').Aggregate(true, (acc, x) => acc && x);
		
	var validationFunctions = new List<(string field, Func<string, bool> validation)>
	{
		("byr", isValidBirthYear),
		("iyr", isValidIssueYear),
		("eyr", isValidExpirationYear),
		("hgt", isValidHeight),
		("hcl", isValidHairColor),
		("ecl", isValidEyeColor),
		("pid", isValidPassportId)
	};
	
	isValidPassport = (dict) =>
		validationFunctions
			.Select(t => isValidField(dict, t.field, t.validation))
			.Aggregate(true, (acc, x) => acc && x);
		
	ReadPassports(ReadLines(inputPath))
		.Select(extractPassport)
		.Count(isValidPassport)
		.Dump("Solution 2");
}

IEnumerable<List<string>> ReadPassports(IEnumerable<string> lines)
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
