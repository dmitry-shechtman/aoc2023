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
            HashSet<Vector> pp = new();
            Console.WriteLine(Part1(s, p, vv, pp));
            Console.WriteLine(Part2(s, p, vv, pp));
        }

        private static int Part1(string s, Vector p, IEnumerable<Vector> vv, HashSet<Vector> pp) =>
            MaxDistance(s, p, vv.First(), pp);

        private static int Part2(string s, Vector p, IEnumerable<Vector> vv, HashSet<Vector> pp) =>
            vv.Zip(new[] { Matrix.RotateRight, Matrix.RotateLeft })
                .Sum(t => CountFill(s, p, t.First, t.Second, pp));

        private static int MaxDistance(string s, Vector p, Vector v, HashSet<Vector> pp)
        {
            int d = 0;
            for (p += v; v != Zero; p += v = GetNext(s, p, v), ++d)
                pp.Add(p);
            return d / 2;
        }

        private static int CountFill(string s, Vector p, Vector v, Matrix m, HashSet<Vector> pp)
        {
            int count = 0;
            for (p += v; v != Zero; p += v = GetNext(s, p, v))
                count += FloodFill(p + v * m, pp);
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
