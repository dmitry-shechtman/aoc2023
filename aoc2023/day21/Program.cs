using aoc.Grids;
using System;
using System.Collections.Generic;
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

        private static int Part1(BitSet bb, Vector start, Size t)
        {
            var pp = new HashSet<Vector>() { start };
            int i = 0;
            return Count(ref pp, ref i, Steps1, bb, t);
        }

        private static long Part2(BitSet bb, Vector start, Size t)
        {
            var pp = new HashSet<Vector> { start };
            var x = Steps2 / t.width;
            var y = Steps2 % t.width;
            var cc = new long[3];
            for (int n = 0, i = 0; n < cc.Length; ++n)
                cc[n] = Count(ref pp, ref i, n * t.width + y, bb, t);
            var c = cc[0];
            var aPlusB = cc[1] - c;
            var fourAPlusTwoB = cc[2] - c;
            var twoA = fourAPlusTwoB - 2 * aPlusB;
            var a = twoA / 2;
            return a * x * x + (aPlusB - a) * x + c;
        }

        private static int Count(ref HashSet<Vector> pp, ref int i, int steps, BitSet bb, Size t)
        {
            int dx = steps / t.width * t.width;
            int dy = steps / t.height * t.height;
            for (; i < steps; ++i)
                pp = new(pp.SelectMany(Grid.GetNeighbors)
                    .AsParallel()
                    .Where(p => bb[(p.x + dx) % t.width * t.width + (p.y + dy) % t.height]));
            return pp.Count;
        }

        private static BitSet Parse(string path, out Size t, out Vector start)
        {
            var s = File.ReadAllText(path);
            t = Size.FromField(s);
            start = t.FindChar(s, 'S');
            return new(s.Replace("\n", "")
                .Select(c => c != '#')
                .ToArray());
        }
    }
}
