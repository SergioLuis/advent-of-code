#!/usr/bin/env dotnet-script

using System.Linq;

var inputPath = Path.GetFullPath("input.txt");
if (Args?.Count > 0 && File.Exists(Args[0]))
    inputPath = Args[0];

List<string> input = ReadLines(inputPath).ToList();

WaitingArea firstWaitingArea = new(
    input, 4, WaitingArea.AdjacentSeatsCalculation.ImmediateAdjacent);

while (true) if (!firstWaitingArea.Iterate()) break;
Console.WriteLine($"Solution 1: {firstWaitingArea.OccupiedSeats}");

WaitingArea secondWaitingArea = new(
    input, 5, WaitingArea.AdjacentSeatsCalculation.VisibleAdjacent);

while (true) if (!secondWaitingArea.Iterate()) break;
Console.WriteLine($"Solution 2: {secondWaitingArea.OccupiedSeats}");

class WaitingArea
{
    public int OccupiedSeats => mCurrentWaitingArea
        .Select(column => column.Where(seat => seat == OCCUPIED).Count()).Sum();

    public static class AdjacentSeatsCalculation
    {
        public static Func<char[][], int, int, int> ImmediateAdjacent
            = GetAdjacentOccupiedSeats;
        
        public static Func<char[][], int, int, int> VisibleAdjacent
            = GetAdjacentVisibleOccupiedSeats;
    }

    public WaitingArea(
        List<string> lines,
        int maxAdjacentOccupiedSeats,
        Func<char[][], int, int, int> adjacentSeatsCalculation)
    {
        mCurrentWaitingArea = lines
            .Where(l => !string.IsNullOrEmpty(l))
            .Select(l => l.Select(c => c).ToArray())
            .ToArray();

        mMaxAdjacentOccupiedSeats = maxAdjacentOccupiedSeats;
        mGetAdjacentSeats = adjacentSeatsCalculation;
        mRows = mCurrentWaitingArea.Length;
        mColumns = mCurrentWaitingArea[0].Length;
    }

    public override string ToString()
        => string.Join(Environment.NewLine, mCurrentWaitingArea.Select(r => string.Join("", r)));

    public bool Iterate()
    {
        char[][] newWaitingArea = Enumerable.Range(0, mRows)
            .Select(i => Enumerable.Range(0, mColumns)
                .Select(j => mCurrentWaitingArea[i][j]).ToArray()).ToArray();

        bool changed = false;
        for (int i = 0; i < mRows; i++)
        {
            for (int j = 0; j < mColumns; j++)
            {
                if (newWaitingArea[i][j] == FLOOR)
                    continue;

                int occupiedSeats = mGetAdjacentSeats(mCurrentWaitingArea, i, j);
                if (occupiedSeats == 0 && newWaitingArea[i][j] == EMPTY)
                {
                    newWaitingArea[i][j] = OCCUPIED;
                    changed = true;
                    continue;
                }

                if (occupiedSeats >= mMaxAdjacentOccupiedSeats && newWaitingArea[i][j] == OCCUPIED)
                {
                    newWaitingArea[i][j] = EMPTY;
                    changed = true;
                }
            }
        }

        mCurrentWaitingArea = newWaitingArea;
        return changed;
    }

    static int GetAdjacentOccupiedSeats(char[][] waitingArea, int row, int column)
    {
        int occupiedSeats = 0;
        for (int i = row - 1; i <= row + 1; i++)
        {
            for (int j = column - 1; j <= column + 1; j++)
            {
                if (i == row && j == column)
                    continue;

                if (i < 0 || i >= waitingArea.Length)
                    continue;

                if (j < 0 || j >= waitingArea[0].Length)
                    continue;

                if (waitingArea[i][j] == OCCUPIED)
                    occupiedSeats++;
            }
        }

        return occupiedSeats;
    }

    static int GetAdjacentVisibleOccupiedSeats(char[][] waitingArea, int row, int column)
    {
        int occupiedSeats = 0;

        // i, j-
        occupiedSeats += GetVisibleOccupiedSeatsInDirection(
            waitingArea, row, column, r => r, c => c - 1);

        // i-, j-
        occupiedSeats += GetVisibleOccupiedSeatsInDirection(
            waitingArea, row, column, r => r - 1, c => c - 1);

        // i+, j
        occupiedSeats += GetVisibleOccupiedSeatsInDirection(
            waitingArea, row, column, r => r + 1, c => c);

        // i-, j+
        occupiedSeats += GetVisibleOccupiedSeatsInDirection(
            waitingArea, row, column, r => r - 1, c => c + 1);

        // i, j+
        occupiedSeats += GetVisibleOccupiedSeatsInDirection(
            waitingArea, row, column, r => r, c => c + 1);

        // i+, j+
        occupiedSeats += GetVisibleOccupiedSeatsInDirection(
            waitingArea, row, column, r => r + 1, c => c + 1);

        // i-, j
        occupiedSeats += GetVisibleOccupiedSeatsInDirection(
            waitingArea, row, column, r => r - 1, c => c);

        // i+, j-
        occupiedSeats += GetVisibleOccupiedSeatsInDirection(
            waitingArea, row, column, r => r + 1, c => c - 1);

        return occupiedSeats;
    }

    static int GetVisibleOccupiedSeatsInDirection(
        char[][] waitingArea,
        int row,
        int column,
        Func<int, int> nextRow,
        Func<int, int> nextColumn)
    {
        while (true)
        {
            row = nextRow(row);
            column = nextColumn(column);

            if (row < 0 || row >= waitingArea.Length)
                break;

            if (column < 0 || column >= waitingArea[0].Length)
                break;

            if (waitingArea[row][column] == FLOOR)
                continue;

            return waitingArea[row][column] == OCCUPIED ? 1 : 0;
        }

        return 0;
    }

    static bool IsEmpty(char c) => c == EMPTY || c == FLOOR;

    char[][] mCurrentWaitingArea;
    readonly int mRows;
    readonly int mColumns;
    readonly int mMaxAdjacentOccupiedSeats;
    readonly Func<char[][], int, int, int> mGetAdjacentSeats;

    const char EMPTY = 'L';
    const char OCCUPIED = '#';
    const char FLOOR = '.';
}

IEnumerable<string> ReadLines(string path)
{
    using FileStream fs = File.OpenRead(path);
    using StreamReader sr = new StreamReader(fs);

    string line;
    while ((line = sr.ReadLine()) != null)
        yield return line;
}
