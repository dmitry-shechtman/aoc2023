using System;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day16
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = File.ReadAllText(args[0]);
            var r = VectorRange.FromField(s);
            Console.WriteLine(Part1(s, r));
            Console.WriteLine(Part2(s, r));
        }

        private static int Part1(string s, VectorRange r) =>
            Energize(Vector.Zero, 1, s, new int[r.Count], r);

        private static int Part2(string s, VectorRange r) =>
            r.Border()
                .Max(p => Enumerable.Range(0, 4)
                    .Max(h => Energize(p, h, s, new int[r.Count], r)));

        private static int Energize(Vector p, int h, string s, int[] a, VectorRange r)
        {
            if (!p.TryGetValue(a, r, out int m) ||
                p.SetValue(a, m | 1 << h, r) == m)
                    return 0;
            int i = "|-./?\\".IndexOf(p.GetChar(s, r));
            h = i > 1 ? h ^ i - 2 : h;
            return (m == 0 ? 1 : 0) + (((h & 1) ^ 1) != i
                ? Energize(p + Vector.Headings[h], h, s, a, r)
                : Energize(p, i, s, a, r) + Energize(p, i + 2, s, a, r));
        }
    }
}
