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
            Solve(input, r, MinCount1, MaxCount1, Predicate1);

        private static int Part2(int[] input, VectorRange r) =>
            Solve(input, r, MinCount2, MaxCount2, Predicate2);

        private static bool Predicate1(int heading, int _, int i) =>
            i != (heading ^ Reverse);

        private static bool Predicate2(int heading, int count, int i) =>
            i == heading || count >= MinCount2;

        private static int Solve(int[] input, VectorRange r2d, int minCount, int maxCount, Func<int, int, int, bool> predicate)
        {
            Vector4D max = (r2d.Max, MaxHeading, maxCount);
            Vector4D size = max + (1, 1, 1, 1);
            int[] losses = new int[size.Count()];
            List<Vector4D> init = new();
            foreach (Vector4D curr in new[]{ ((0, 0), East, 0), ((0, 0), South, 0) })
            {
                var (curr2d, heading, count) = curr;
                for (int i = 0; i < 4; i++)
                {
                    if (predicate(curr.z, curr.w, i))
                    {
                        Vector next2d = curr2d + Vector.Headings[i];
                        int count2 = i == heading ? count + 1 : 1;
                        Vector4D next = (next2d, i, count2);
                        if (next.x >= 0 && next.y >= 0 && next < size)
                        {
                            int loss1 = next2d.GetValue(input, r2d);
                            next.SetValue(losses, loss1, size);
                            init.Insert(0, next);
                        }
                    }
                }
            }
            init.AsParallel().ForAll(v => Walk(input, losses, size, predicate, v));
            return new Vector4DRange((r2d.Max, MinHeading, minCount), max)
                .Select(p => p.GetValue(losses, size))
                .Where(v => v > 0)
                .Min();
        }

        private static void Walk(int[] input, int[] losses, Vector4D size, Func<int, int, int, bool> predicate, params Vector4D[] init)
        {
            Queue<Vector4D> candidates = new(init);
            while (candidates.TryDequeue(out var curr))
            {
                int loss = curr.GetValue(losses, size);
                for (int i = 0; i <= MaxHeading; i++)
                    if (predicate(curr.z, curr.w, i))
                        Process(curr, loss, i, input, candidates, losses, size);
            }
        }

        private static void Process(Vector4D curr, int loss, int heading2, int[] input, Queue<Vector4D> candidates, int[] losses, Vector4D size)
        {
            var (curr2d, heading, count) = curr;
            Vector next2d = curr2d + Vector.Headings[heading2];
            int count2 = heading2 == heading ? count + 1 : 1;
            Vector4D next = (next2d, heading2, count2);
            if (next.x >= 0 && next.y >= 0 && next < size)
            {
                int loss1 = loss + next2d.GetValue(input, (Vector)size);
                int index2 = next.GetIndex(size);
                int loss2 = losses[index2];
                if (loss2 == 0 || loss2 > loss1)
                {
                    losses[index2] = loss1;
                    candidates.Enqueue(next);
                }
            }
        }

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
