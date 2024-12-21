using aoc.Grids;
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
            var z = Size.FromField(s);
            var p = z.FindChar(s, 'S');
            var vv = Grid.Headings.Where(v => GetNext(s, p + v, v, z) != Zero);
            Grid grid = new();
            Console.WriteLine(Part1(s, p, vv, grid, z));
            Console.WriteLine(Part2(s, p, vv, grid, z));
        }

        private static int Part1(string s, Vector p, IEnumerable<Vector> vv, Grid grid, Size z) =>
            MaxDistance(s, p, vv.First(), grid, z);

        private static int Part2(string s, Vector p, IEnumerable<Vector> vv, Grid grid, Size z) =>
            vv.Zip(new[] { Matrix.RotateRight, Matrix.RotateLeft })
                .Sum(t => CountFill(s, p, t.First, t.Second, grid, z));

        private static int MaxDistance(string s, Vector p, Vector v, Grid grid, Size z)
        {
            int d = 0;
            for (p += v; v != Zero; p += v = GetNext(s, p, v, z), ++d)
                grid.Add(p);
            return d / 2;
        }

        private static int CountFill(string s, Vector p, Vector v, Matrix m, Grid grid, Size z)
        {
            int count = 0;
            for (p += v; v != Zero; p += v = GetNext(s, p, v, z))
                count += grid.FloodFill(p + v * m);
            return count;
        }

        private static Vector GetNext(string s, Vector p, Vector v, Size z) => z.GetChar(s, p) switch
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
