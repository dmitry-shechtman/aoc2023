using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static aoc.Vector;

namespace aoc.aoc2023.day10
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = File.ReadAllText(args[0]);
            Vector p = FindChar(s, 'S');
            var vv = Headings.Where(v => GetNext(s, p + v, v) != Zero);
            Dictionary<Vector, int> dd = new();
            Console.WriteLine(Part1(s, p, vv, dd));
            Console.WriteLine(Part2(s, p, vv, dd));
        }

        private static int Part1(string s, Vector p, IEnumerable<Vector> vv, Dictionary<Vector, int> dd) =>
            vv.Max(v => MaxDistance(s, p, v, dd));

        private static int Part2(string s, Vector p, IEnumerable<Vector> vv, Dictionary<Vector, int> dd)
        {
            HashSet<Vector> pp = new(dd.Keys) { p };
            return vv.Zip(new[] { Matrix.RotateRight, Matrix.RotateLeft })
                .Sum(t => CountFill(s, p, t.First, t.Second, pp));
        }

        private static int MaxDistance(string s, Vector p, Vector v, Dictionary<Vector, int> dd)
        {
            int d = 0, max = 0;
            for (Vector q = p + v; q != p; q += v = GetNext(s, q, v))
                if (!dd.TryAdd(q, ++d))
                    max = Math.Max(max, dd[q] = Math.Min(dd[q], d));
            return max;
        }

        private static int CountFill(string s, Vector p, Vector v, Matrix m, HashSet<Vector> pp)
        {
            int count = 0;
            for (Vector q = p + v; q != p; q += v = GetNext(s, q, v))
                count += FloodFill(q + v * m, pp);
            return count;
        }

        private static Vector GetNext(string s, Vector p, Vector v) => GetChar(p, s) switch
        {
            '|' when v.x == 0 => v,
            '-' when v.y == 0 => v,
            'L' => v.y == 0 ? North : East,
            'J' => v.y == 0 ? North : West,
            '7' => v.y == 0 ? South : West,
            'F' => v.y == 0 ? South : East,
            _ => Zero,
        };
    }
}
