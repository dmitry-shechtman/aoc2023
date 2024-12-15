using aoc.Grids;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day17
{
    class Program
    {
        private const int MinCount1  = 1, MaxCount1 = 3, MinCount2  = 4, MaxCount2 = 10;
        private const int MinHeading = 0, Reverse   = 2, MaxHeading = 3;
        private const int East       = 1, South     = 2;

        static void Main(string[] args)
        {
            var input = Parse(args[0], out var r);
            Console.WriteLine(Part1(input, r));
            Console.WriteLine(Part2(input, r));
        }

        private static int Part1(int[] input, VectorRange r) =>
            Solve(input, r, MinCount1, MaxCount1, Match1);

        private static int Part2(int[] input, VectorRange r) =>
            Solve(input, r, MinCount2, MaxCount2, Match2);

        private static bool Match1(int heading, int _, int i) =>
            i != (heading ^ Reverse);

        private static bool Match2(int heading, int count, int i) =>
            i == heading || count >= MinCount2;

        private static int Solve(int[] input, VectorRange r2d, int minCount, int maxCount, Func<int, int, int, bool> match)
        {
            (int x, int y, int heading, int count)
                max = (r2d.Max.x, r2d.Max.y, MaxHeading, maxCount),
                size = (max.x + 1, max.y + 1, max.heading + 1, max.count + 1);
            int[] losses = new int[size.x * size.y * size.heading * size.count];
            List<(int, int, int, int)> init = new();
            foreach ((int x, int y, int heading, int count) in new[]{ (0, 0, East, 0), (0, 0, South, 0) })
            {
                for (int i = 0; i < 4; i++)
                {
                    if (match(heading, count, i))
                    {
                        var (x2, y2) = Grid.Headings[i];
                        if (x2 >= 0 && y2 >= 0 && x2 < size.x && y2 < size.y)
                        {
                            int count2 = i == heading ? count + 1 : 1;
                            if (count2 < size.count)
                            {
                                var next = (x2, y2, i, count2);
                                losses[GetIndex(next, size)] = input[x2 + size.x * y2];
                                init.Insert(0, next);
                            }
                        }
                    }
                }
            }
            init.AsParallel().ForAll(v => Walk(input, losses, size, match, v));
            return new Vector4DRange((r2d.Max, MinHeading, minCount), max)
                .Select(p => GetIndex(p, size))
                .Select(i => losses[i])
                .Where(v => v > 0)
                .Min();
        }

        private static void Walk(int[] input, int[] losses, (int, int, int, int) size, Func<int, int, int, bool> match, params (int, int, int, int)[] init)
        {
            Queue<(int, int, int heading, int count)> candidates = new(init);
            while (candidates.TryDequeue(out var curr))
            {
                int index = GetIndex(curr, size);
                int loss = losses[index];
                for (int i = 0; i < 4; i++)
                    if (match(curr.heading, curr.count, i))
                        Process(curr, loss, i, input, candidates, losses, size);
            }
        }

        private static void Process((int, int, int, int) curr, int loss, int heading2, int[] input, Queue<(int, int, int, int)> candidates, int[] losses, (int x, int y, int heading, int count) size)
        {
            var (x, y, heading, count) = curr;
            int count2 = heading2 == heading ? count + 1 : 1;
            if (count2 < size.count)
            {
                var (x2, y2) = (x, y) + Grid.Headings[heading2];
                if (x2 >= 0 && y2 >= 0 && x2 < size.x && y2 < size.y)
                {
                    var next = (x2, y2, heading2, count2);
                    int loss1 = loss + input[x2 + size.x * y2];
                    int index2 = GetIndex(next, size);
                    int loss2 = losses[index2];
                    if (loss2 == 0 || loss2 > loss1)
                    {
                        losses[index2] = loss1;
                        candidates.Enqueue(next);
                    }
                }
            }
        }

        private static int GetIndex((int x, int y, int z, int w) p, (int x, int y, int z, int) size) =>
            p.x + size.x * (p.y + size.y * (p.z + size.z * p.w));

        private static int[] Parse(string path, out VectorRange r)
        {
            var s = File.ReadAllText(path);
            r = VectorRange.FromField(s);
            return s.Where(c => c != '\n')
                .Select(c => c - '0')
                .ToArray();
        }
    }
}
