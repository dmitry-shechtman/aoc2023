﻿using aoc.Grids;
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
            Vector4D min = (r2d.Max, MinHeading, minCount);
            Vector4D max = (r2d.Max, MaxHeading, maxCount);
            Vector4DRange range = (min, max);
            Size4D size = new(range);
            int[] losses = new int[size.Length];
            Vector4D[] init = { ((0, 0), East, 0), ((0, 0), South, 0) };
            init.AsParallel().ForAll(v => Walk(input, losses, size, match, v));
            return range
                .Select(p => size.GetValue(losses, p))
                .Where(v => v > 0)
                .Min();
        }

        private static void Walk(int[] input, int[] losses, Size4D size, Func<int, int, int, bool> match, params Vector4D[] init)
        {
            Queue<Vector4D> candidates = new(init);
            while (candidates.TryDequeue(out var curr))
            {
                int loss = size.GetValue(losses, curr);
                for (int i = 0; i <= MaxHeading; i++)
                    if (match(curr.z, curr.w, i))
                        Process(curr, loss, i, input, candidates, losses, size);
            }
        }

        private static void Process(Vector4D curr, int loss, int heading2, int[] input, Queue<Vector4D> candidates, int[] losses, Size4D size)
        {
            var (curr2d, heading, count) = curr;
            Vector next2d = curr2d + Grid.Headings[heading2];
            int count2 = heading2 == heading ? count + 1 : 1;
            Vector4D next = (next2d, heading2, count2);
            if (size.Contains(next))
            {
                int loss1 = loss + ((Size)size).GetValue(input, next2d);
                int index2 = size.GetIndex(next);
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
