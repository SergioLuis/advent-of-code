<Query Kind="Program" />

void Main(string[] args)
{
	var inputPath = @".\input.txt";
	if (args?.Length > 0 && File.Exists(args[0]))
		inputPath = args[0];

	var program = ReadLines(inputPath)
		.Select(HandheldConsole.Instruction.Parse)
		.ToList();

	var handheldConsole = new HandheldConsole(program);	
	handheldConsole.RunUntilCompletion();
	handheldConsole.GlobalAcc.Dump("Solution 1");
	
	GenerateProgramVariations(program)
		.Select(program => new HandheldConsole(program))
		.First(console =>
			console.RunUntilCompletion() == HandheldConsole.HaltCause.CompletedOk)
		.GlobalAcc
		.Dump("Solution 2");
}

IEnumerable<List<HandheldConsole.Instruction>> GenerateProgramVariations(
	List<HandheldConsole.Instruction> program)
{
	Func<HandheldConsole.Instruction, HandheldConsole.Instruction> swapInstruction =
		(instruction) => new HandheldConsole.Instruction(
			instruction.OpCode == HandheldConsole.Instruction.OpCodes.Nop
				? HandheldConsole.Instruction.OpCodes.Jmp
				: HandheldConsole.Instruction.OpCodes.Nop,
			instruction.OpValue);

	for (int i = 0; i < program.Count; i++)
	{
		if (program[i].OpCode == HandheldConsole.Instruction.OpCodes.Acc)
			continue;
	
		var result = new List<HandheldConsole.Instruction>();

		result.AddRange(program.Take(i));
		result.Add(swapInstruction(program[i]));
		result.AddRange(program
			.Skip(i + 1)
			.Take(program.Count));
			
		yield return result;
	}
}

class HandheldConsole
{
	public struct Instruction
	{
		public enum OpCodes
		{
			Nop,
			Acc,
			Jmp,
			Invalid
		}
		
		public readonly OpCodes OpCode;
		public readonly int OpValue;
		
		public Instruction(OpCodes opCode, int opValue)
		{
			OpCode = opCode;
			OpValue = opValue;
		}
		
		public static Instruction Parse(string s)
		{
			string[] parts = s.Split(' ');
			OpCodes opCode = parts[0] switch {
				"nop" => OpCodes.Nop,
				"acc" => OpCodes.Acc,
				"jmp" => OpCodes.Jmp,
				_ => OpCodes.Invalid
			};
			
			return new Instruction(opCode, int.Parse(parts[1]));
		}
	}
	
	public enum HaltCause
	{
		CompletedOk,
		ProgramCounterOutsideOfBounds,
		LoopDetected
	}

	public int GlobalAcc;
	
	public HandheldConsole(List<Instruction> program)
	{
		GlobalAcc = 0;
		mProgramCounter = 0;
		mProgram = program;
		mVisitedInstructions = new HashSet<int>();
	}
	
	public HaltCause RunUntilCompletion()
	{
		while (!mVisitedInstructions.Contains(mProgramCounter))
		{
			if (mProgramCounter < 0 || mProgramCounter >= mProgram.Count)
				return HaltCause.ProgramCounterOutsideOfBounds;

			mVisitedInstructions.Add(mProgramCounter);
			Instruction instruction = mProgram[mProgramCounter];

			int nextPcIncrement = 1;
			switch (instruction.OpCode)
			{
				case Instruction.OpCodes.Nop:
					break;
					
				case Instruction.OpCodes.Acc:
					GlobalAcc += instruction.OpValue;
					break;
					
				case Instruction.OpCodes.Jmp:
					nextPcIncrement = instruction.OpValue;
					break;
					
				default:
					break;
			}
			
			mProgramCounter += nextPcIncrement;

			if (mProgramCounter == mProgram.Count)
				return HaltCause.CompletedOk;
		}
		
		return HaltCause.LoopDetected;
	}

	int mProgramCounter;
	
	readonly List<Instruction> mProgram;
	readonly HashSet<int> mVisitedInstructions;
}

IEnumerable<string> ReadLines(string path)
{
	using FileStream fs = File.OpenRead(path);
	using StreamReader sr = new StreamReader(fs);
		
	string line;
	while ((line = sr.ReadLine()) != null)
		yield return line;
}
