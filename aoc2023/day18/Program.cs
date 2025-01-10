using aoc.Grids;
using System;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day18
{
    class Program
    {
        static void Main(string[] args)
        {
            var ss = File.ReadAllLines(args[0]);
            Console.WriteLine(Part1(ss));
            Console.WriteLine(Part2(ss));
        }

        private static long Part1(string[] ss) =>
            Solve(Parse1(ss));

        private static long Part2(string[] ss) =>
            Solve(Parse2(ss));

        private static long Solve(Vector[] vv)
        {
            long acc = 0;
            Vector p = Vector.Zero, q;
            for (int i = 0; i < vv.Length; p = q, ++i)
                acc += ((q = p + vv[i]) - p).Abs() + (long)p.x * q.y - (long)p.y * q.x;
            return acc / 2 + 1;
        }

        private static Vector[] Parse1(string[] ss) =>
            Grid.Path.Parse(ss, ' ')
                .Select(t => t.v * t.d)
                .ToArray();

        private static Vector[] Parse2(string[] ss) =>
            ss.Select(ParseOne)
                .ToArray();

        private static Vector ParseOne(string s)
        {
            var t = s.Split(' ')[2][2..^1];
            var bytes = Convert.FromHexString($"{t[4..]}{t[2..4]}{t[..2]}00");
            int i = BitConverter.ToInt32(bytes);
            return Grid.Headings[(i + 1) % 4] * (i >> 4);
        }
    }
}
