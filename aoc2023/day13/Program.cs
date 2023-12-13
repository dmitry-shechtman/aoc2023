using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day13
{
    class Program
    {
        static void Main(string[] args)
        {
            var ppp = Parse(args[0]);
            Console.WriteLine(Part1(ppp));
            Console.WriteLine(Part2(ppp));
        }

        private static int Part1(HashSet<Vector>[] ppp) =>
            ppp.Sum(pp => Sum(pp, 0));

        private static int Part2(HashSet<Vector>[] ppp) =>
            ppp.Sum(pp => Sum(pp, 1));

        private static int Sum(HashSet<Vector> pp, int n) =>
            Sum(pp, n, p => p.x, (p, v) => (v, p.y)) +
            Sum(pp, n, p => p.y, (p, v) => (p.x, v)) * 100;

        private static int Sum(HashSet<Vector> pp, int n, Func<Vector, int> f, Func<Vector, int, Vector> g) =>
            Enumerable.Range(0, pp.Max(f) + 1).Where(i =>
                Enumerable.Range(0, Math.Min(i, pp.Max(f) + 1 - i)).Sum(j =>
                    pp.Where(p => f(p) == i + j).Count(p => !pp.Contains(g(p, i - j - 1))) +
                    pp.Where(p => f(p) == i - j - 1).Count(p => !pp.Contains(g(p, i + j)))) == n)
            .Sum();

        private static HashSet<Vector>[] Parse(string path) =>
            File.ReadAllText(path).Split("\n\n")
                .Select(s => Vector.ParseField(s))
                .ToArray();
    }
}
