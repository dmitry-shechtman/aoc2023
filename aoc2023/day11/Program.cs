using aoc.Grids;
using System;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day11
{
    class Program
    {
        static void Main(string[] args)
        {
            var grid = Parse(args[0]);
            Console.WriteLine(Part1(grid));
            Console.WriteLine(Part2(grid));
        }

        private static long Part1(Grid grid) =>
            Solve(grid.ToArray(), 1);

        private static long Part2(Grid grid) =>
            Solve(grid.ToArray(), 999999);

        private static long Solve(Span<Vector> a, int n) =>
            Expand(a, n, p => p.x, (p, v) => (p.x + v, p.y)) +
            Expand(a, n, p => p.y, (p, v) => (p.x, p.y + v));

        private static long Expand(Span<Vector> a, int n, Func<Vector, int> f, Func<Vector, int, Vector> g)
        {
            long total = 0;
            a.Sort((p, q) => f(p) - f(q));
            for (int i = 1; i < a.Length; i++)
            {
                int d = f(a[i] - a[i - 1]);
                if (d > 1)
                    for (int j = i; j < a.Length; j++)
                        a[j] = g(a[j], n * (d - 1));
                for (int j = 0; j < i; j++)
                    total += f(a[i]) - f(a[j]);
            }
            return total;
        }

        private static Grid Parse(string path) =>
            Grid.Parse(File.ReadAllText(path));
    }
}
