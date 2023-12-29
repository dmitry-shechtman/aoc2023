using aoc.Grids;
using System;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day13
{
    class Program
    {
        static void Main(string[] args)
        {
            var grids = Parse(args[0]);
            Console.WriteLine(Part1(grids));
            Console.WriteLine(Part2(grids));
        }

        private static int Part1(Grid[] grids) =>
            grids.Sum(pp => Sum(pp, 0));

        private static int Part2(Grid[] grids) =>
            grids.Sum(pp => Sum(pp, 1));

        private static int Sum(Grid pp, int n) =>
            Sum(pp, n, p => p.x, (p, v) => (v, p.y)) +
            Sum(pp, n, p => p.y, (p, v) => (p.x, v)) * 100;

        private static int Sum(Grid pp, int n, Func<Vector, int> f, Func<Vector, int, Vector> g) =>
            Enumerable.Range(1, pp.Max(f)).Where(i =>
                Enumerable.Range(0, Math.Min(i, pp.Max(f) + 1 - i)).Sum(j =>
                    pp.Where(p => f(p) == i + j).Count(p => !pp.Contains(g(p, i - j - 1))) +
                    pp.Where(p => f(p) == i - j - 1).Count(p => !pp.Contains(g(p, i + j)))) == n)
            .Sum();

        private static Grid[] Parse(string path) =>
            File.ReadAllText(path).Split("\n\n")
                .Select(Grid.Parse)
                .ToArray();
    }
}
