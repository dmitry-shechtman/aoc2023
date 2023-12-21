﻿using System;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day21
{
    class Program
    {
        private const int Steps1 = 64;
        private const int Steps2 = 26501365;

        static void Main(string[] args)
        {
            var bb = Parse(args[0], out var t, out var start);
            Console.WriteLine(Part1(bb, start, t));
            Console.WriteLine(Part2(bb, start, t));
        }

        private static int Part1(bool[] bb, Vector start, Vector t)
        {
            var pp = new[] { start };
            int i = 0;
            return Count(ref pp, ref i, Steps1, bb, t);
        }

        private static long Part2(bool[] bb, Vector start, Vector t)
        {
            var pp = new[] { start };
            var x = Steps2 / t.x;
            var y = Steps2 % t.x;
            var cc = new long[3];
            for (int n = 0, i = 0; n < cc.Length; ++n)
                cc[n] = Count(ref pp, ref i, n * t.x + y, bb, t);
            var c = cc[0];
            var aPlusB = cc[1] - c;
            var fourAPlusTwoB = cc[2] - c;
            var twoA = fourAPlusTwoB - 2 * aPlusB;
            var a = twoA / 2;
            return a * x * x + (aPlusB - a) * x + c;
        }

        private static int Count(ref Vector[] pp, ref int i, int steps, bool[] bb, Vector t)
        {
            int dx = steps / t.x * t.x;
            int dy = steps / t.y * t.y;
            for (; i < steps; ++i)
                pp = pp.SelectMany(Vector.GetNeighborsJVN)
                    .AsParallel()
                    .Where(p => Vector.GetValue(((p.x + dx) % t.x, (p.y + dy) % t.y), bb, t))
                    .Distinct()
                    .ToArray();
            return pp.Length;
        }

        private static bool[] Parse(string path, out Vector t, out Vector start)
        {
            var s = File.ReadAllText(path);
            t = Vector.FromField(s);
            start = Vector.FindChar(s, 'S');
            return s.Replace("\n", "")
                .Select(c => c != '#')
                .ToArray();
        }
    }
}
