#!/usr/bin/env dotnet-script

var inputPath = Path.GetFullPath("input.txt");
if (Args?.Count > 0 && File.Exists(Args[0]))
    inputPath = Args[0];

List<Segment> input = ReadLines(inputPath).Select(ParseSegment).ToList();
List<Segment> horizontalOrVertical = input
    .Where(s => s.IsHorizontal || s.IsVertical)
    .ToList();

HashSet<Point> firstSolutionPoints = GetIntersections(horizontalOrVertical);
Console.WriteLine($"Solution 1: {firstSolutionPoints.Count}");

HashSet<Point> secondSolutionPoints = GetIntersections(input);
Console.WriteLine($"Solution 2: {secondSolutionPoints.Count}");

static HashSet<Point> GetIntersections(List<Segment> segments)
{
    HashSet<Point> intersectionPoints = new();
    for (int i = 0; i < segments.Count - 1; i++)
    {
        for (int j = i + 1; j < segments.Count; j++)
        {
            if (segments[i].Intersects(segments[j], out List<Point> intersections))
            {
                intersections.ForEach(p => intersectionPoints.Add(p));
            }
        }
    }

    return intersectionPoints;
}

readonly struct Point
{
    public readonly int X;
    public readonly int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return X.GetHashCode() * Y.GetHashCode();
        }
    }

    public override bool Equals(object obj)
    {
        Point other = (Point)obj;
        return X == other.X && Y == other.Y;
    }

    public override string ToString() => $"{X},{Y}";
}

class Segment
{
    interface ILineEquation
    {
        bool Intersects(ILineEquation other, out Point? intersectionPoint);
    }

    class SlopeInterceptEquation : ILineEquation
    {
        public int Slope { get; private set; }
        public int Intercept { get; private set; }
        
        public SlopeInterceptEquation(Point p1, Point p2)
        {
            Slope = (p2.Y - p1.Y) / (p2.X - p1.X);
            Intercept = p1.Y - Slope * p1.X;
        }

        public int CalculateX(int y) => (y - Intercept) / Slope;

        public int CalculateY(int x) => Slope * x + Intercept;

        bool ILineEquation.Intersects(ILineEquation other, out Point? intersectionPoint)
        {
            if (other is SlopeInterceptEquation siOther)
            {
                if (Slope != siOther.Slope)
                {
                    int intersectX = (Intercept - siOther.Intercept) / (siOther.Slope - Slope);
                    int intersectY = Slope * intersectX + Intercept;

                    intersectionPoint = new(intersectX, intersectY);
                    return true;
                }

                if (Intercept == siOther.Intercept)
                {
                    intersectionPoint = new(int.MaxValue, int.MaxValue);
                    return true;
                }

                intersectionPoint = null;
                return false;
            }

            if (other is VerticalEquation || other is HorizontalEquation)
                return other.Intersects(this, out intersectionPoint);

            throw new InvalidOperationException();
        }

        public override string ToString() => $"Y = {Slope} * X + {Intercept}";
    }

    class VerticalEquation : ILineEquation
    {
        public int X { get; private set; }

        public VerticalEquation(int x)
        {
            X = x;
        }

        bool ILineEquation.Intersects(ILineEquation other, out Point? intersectionPoint)
        {
            if (other is HorizontalEquation hOther)
            {
                intersectionPoint = new(X, hOther.Y);
                return true;
            }

            if (other is VerticalEquation vOther)
            {
                if (X == vOther.X)
                {
                    intersectionPoint = new(X, int.MaxValue);
                    return true;
                }

                intersectionPoint = null;
                return false;
            }

            if (other is SlopeInterceptEquation siOther)
            {
                intersectionPoint = new(X, siOther.CalculateY(X));
                return true;
            }

            throw new InvalidOperationException();
        }

        public override string ToString() => $"X = {X}";
    }

    class HorizontalEquation : ILineEquation
    {
        public int Y { get; private set; }

        public HorizontalEquation(int y)
        {
            Y = y;
        }

        bool ILineEquation.Intersects(ILineEquation other, out Point? intersectionPoint)
        {
            if (other is VerticalEquation vOther)
            {
                intersectionPoint = new(vOther.X, Y);
                return true;
            }

            if (other is HorizontalEquation hOther)
            {
                if (Y == hOther.Y)
                {
                    intersectionPoint = new(int.MaxValue, Y);
                    return true;
                }

                intersectionPoint = null;
                return false;
            }

            if (other is SlopeInterceptEquation siOther)
            {
                intersectionPoint = new(siOther.CalculateX(Y), Y);
                return true;
            }

            throw new InvalidOperationException();
        }

        public override string ToString() => $"Y = {Y}";
    }

    public readonly Point P1;
    public readonly Point P2;

    public bool IsHorizontal => P1.Y == P2.Y;
    public bool IsVertical => P1.X == P2.X;

    public Segment(Point p1, Point p2)
    {
        P1 = p1;
        P2 = p2;
        mEquation = BuildEquation(p1, p2);
    }

    public override string ToString() => $"[{P1} -> {P2} ({mEquation})]";

    public bool Intersects(Segment other, out List<Point> intersectionPoints)
    {
        if (!mEquation.Intersects(other.mEquation, out Point? ip))
        {
            intersectionPoints = new(Array.Empty<Point>());
            return false;
        }

        Point intersection = ip.Value;

        // Just the one point of intersection
        if (ip.Value.X != int.MaxValue && ip.Value.Y != int.MaxValue)
        {
            // Need to check if the intersection point is within both segments
            if (!ContainsPoint(ip.Value) || !other.ContainsPoint(ip.Value))
            {
                intersectionPoints = new(Array.Empty<Point>());
                return false;
            }

            intersectionPoints = new List<Point>() { ip.Value };
            return true;
        }

        // Points of intersection in X
        if (ip.Value.X == int.MaxValue && ip.Value.Y != int.MaxValue)
        {
            if (!IsHorizontal || !other.IsHorizontal)
                throw new InvalidOperationException();

            (int minX, int maxX) = P1.X < P2.X ? (P1.X, P2.X) : (P2.X, P1.X);
            intersectionPoints = Enumerable
                .Range(minX, maxX - minX + 1)
                .Select(x => new Point(x, P1.Y))
                .Where(other.ContainsPoint)
                .ToList();
            return intersectionPoints.Any();
        }

        // Points of intersection in Y
        if (ip.Value.X != int.MaxValue && ip.Value.Y == int.MaxValue)
        {
            if (!IsVertical || !other.IsVertical)
                throw new InvalidOperationException();

            (int minY, int maxY) = P1.Y < P2.Y ? (P1.Y, P2.Y) : (P2.Y, P1.Y);
            intersectionPoints = Enumerable
                .Range(minY, maxY - minY + 1)
                .Select(y => new Point(P1.X, y))
                .Where(other.ContainsPoint)
                .ToList();
            return intersectionPoints.Any();
        }

        // Points of intersection in X and Y
        if (ip.Value.X == int.MaxValue && ip.Value.Y == int.MaxValue)
        {
            SlopeInterceptEquation eq = mEquation as SlopeInterceptEquation;
            if (eq == null)
                throw new InvalidOperationException();

            (int minX, int maxX) = P1.X < P2.X ? (P1.X, P2.X) : (P2.X, P1.X);
            intersectionPoints = Enumerable
                .Range(minX, maxX - minX + 1)
                .Select(x => new Point(x, eq.CalculateY(x)))
                .Where(other.ContainsPoint)
                .ToList();
            return intersectionPoints.Any();
        }

        throw new InvalidOperationException();
    }

    public bool ContainsPoint(Point p)
    {
        int crossProduct = (p.Y - P1.Y) * (P2.X - P1.X) - (p.X - P1.X) * (P2.Y - P1.Y);
        if (crossProduct != 0)
            return false;

        int dotProduct = (p.X - P1.X) * (P2.X - P1.X) + (p.Y - P1.Y) * (P2.Y - P1.Y);
        if (dotProduct < 0)
            return false;

        int squaredLengthP2P1 = (P2.X - P1.X) * (P2.X - P1.X) + (P2.Y - P1.Y) * (P2.Y - P1.Y);
        if (dotProduct > squaredLengthP2P1)
            return false;

        return true;
    }

    static ILineEquation BuildEquation(Point p1, Point p2)
    {
        if (p1.X == p2.X)
            return new VerticalEquation(p1.X);

        if (p1.Y == p2.Y)
            return new HorizontalEquation(p1.Y);

        return new SlopeInterceptEquation(p1, p2);
    }

    readonly ILineEquation mEquation;
}

Segment ParseSegment(string line)
{
    const string separator = " -> ";
    string[] segmentParts = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
    string[] startParts = segmentParts[0].Split(',');
    string[] endParts = segmentParts[1].Split(',');

    Point start = new(int.Parse(startParts[0]), int.Parse(startParts[1]));
    Point end = new(int.Parse(endParts[0]), int.Parse(endParts[1]));

    if (start.X > end.X)
    {
        Point temp = start;
        start = end;
        end = temp;
    }

    return new(start, end);
}

IEnumerable<string> ReadLines(string path)
{
    using FileStream fs = File.OpenRead(path);
    using StreamReader sr = new StreamReader(fs);

    string line;
    while ((line = sr.ReadLine()) != null)
        yield return line;
}
