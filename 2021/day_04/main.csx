#!/usr/bin/env dotnet-script

var inputPath = Path.GetFullPath("input.txt");
if (Args?.Count > 0 && File.Exists(Args[0]))
    inputPath = Args[0];

BingoGame input = BingoGame.Parse(ReadLines(inputPath).ToList());

BingoGame.BingoBoard winnerBoard;
while (!input.DrawNumber(out winnerBoard));

Console.WriteLine($"Solution 1: {winnerBoard.CalculateScore()}");

input = BingoGame.Parse(ReadLines(inputPath).ToList());

BingoGame.BingoBoard loserBoard;
while (!input.DrawNumberLatestToWin(out loserBoard));

Console.WriteLine($"Solution 2: {loserBoard.CalculateScore()}");


public class BingoGame
{
    public class BingoBoard
    {
        public void AddRow(List<int> row)
        {
            mRows.Add(row);
            mDrawns.Add(Enumerable.Range(0, row.Count).Select(_ => false).ToList());
        }

        public void NumberDrawn(int n)
        {
            mLastDrawnNumber = n;

            for (int i = 0; i < mRows.Count; i++)
            {
                for (int j = 0; j < mRows[i].Count; j++)
                {
                    if (mRows[i][j] != n)
                        continue;

                    mDrawns[i][j] = true;
                    break;
                }
            }
        }

        public bool IsWinner()
        {
            // First check rows
            if (mDrawns.Where(row => row.All(drawn => drawn)).Any())
                return true;

            // Then check columns
            for (int i = 0; i < mDrawns[0].Count; i++)
            {
                if (mDrawns.Select(r => r[i]).All(drawn => drawn))
                    return true;
            }

            return false;
        }

        public int CalculateScore()
        {
            if (mLastDrawnNumber == -1)
                return -1;

            int sum = 0;
            for (int i = 0; i < mRows.Count; i++)
            {
                for (int j = 0; j < mRows[i].Count; j++)
                {
                    if (mDrawns[i][j])
                        continue;

                    sum += mRows[i][j];
                }
            }

            return sum * mLastDrawnNumber;
        }

        int mLastDrawnNumber = -1;
        readonly List<List<int>> mRows = new();
        readonly List<List<bool>> mDrawns = new();
    }

    public void AddNumbersToDraw(IEnumerable<int> numbers) => mNumbersToDraw.AddRange(numbers);
    
    public void AddBoard(BingoBoard board) => mBoards.Add(board);

    public bool DrawNumber(out BingoBoard winnerBoard)
    {
        int n = mNumbersToDraw[mNextNumberToDraw++];
        mBoards.ForEach(board => board.NumberDrawn(n));
        winnerBoard = mBoards.FirstOrDefault(b => b.IsWinner());

        return winnerBoard != null;
    }

    public bool DrawNumberLatestToWin(out BingoBoard loserBoard)
    {
        int n = mNumbersToDraw[mNextNumberToDraw++];
        mBoards.ForEach(board => board.NumberDrawn(n));

        if (mBoards.Count > 1)
        {
            mBoards.RemoveAll(b => b.IsWinner());
            loserBoard = null;
            return false;
        }

        loserBoard = mBoards.First();
        if (loserBoard.IsWinner())
            return true;

        loserBoard = null;
        return false;
    }

    public static BingoGame Parse(List<string> lines)
    {
        BingoGame result = new();

        // Numbers to draw in the first line
        result.AddNumbersToDraw(
            lines[0]
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse));

        BingoBoard currentBoard = new();

        // Boards start on line 2
        for (int i = 2; i < lines.Count; i++)
        {
            if (string.IsNullOrEmpty(lines[i]))
            {
                result.AddBoard(currentBoard);
                currentBoard = new();
                continue;
            }

            currentBoard.AddRow(lines[i]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList());
        }

        result.AddBoard(currentBoard);
        return result;
    }

    int mNextNumberToDraw = 0;
    readonly List<int> mNumbersToDraw = new();
    readonly List<BingoBoard> mBoards = new();
}

IEnumerable<string> ReadLines(string path)
{
    using FileStream fs = File.OpenRead(path);
    using StreamReader sr = new StreamReader(fs);

    string line;
    while ((line = sr.ReadLine()) != null)
        yield return line;
}
