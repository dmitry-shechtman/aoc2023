using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day11
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = File.ReadAllText(args[0]);
            var pp = Vector.ParseField(s);
            Console.WriteLine(Part1(pp));
            Console.WriteLine(Part2(pp));
        }

        private static long Part1(HashSet<Vector> pp) =>
            Solve(pp.ToArray(), 1);

        private static long Part2(HashSet<Vector> pp) =>
            Solve(pp.ToArray(), 999999);

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
    }
}
