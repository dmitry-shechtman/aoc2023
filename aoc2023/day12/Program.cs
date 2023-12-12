using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day12
{
    class Program
    {
        static void Main(string[] args)
        {
            var tt = Parse(args[0]);
            Console.WriteLine(Part1(tt));
            Console.WriteLine(Part2(tt));
        }

        private static long Part1((string, int[])[] tt) =>
            Solve(tt, 1);

        private static long Part2((string, int[])[] tt) =>
            Solve(tt, 5);

        private static long Solve((string, int[])[] tt, int n) =>
            tt.Sum(t => Count(t, n));

        private static long Count((string s, int[] lengths) t, int n)
        {
            string s = string.Join('?', Enumerable.Repeat(t.s, n));
            int[] lengths = Enumerable.Repeat(t.lengths, n).SelectMany(v => v).ToArray();
            return Count(s, lengths, new());
        }

        private static long Count(string s, int[] lens, Dictionary<int, long> counts, int key = 0)
        {
            if (counts.TryGetValue(key, out long count))
                return count;
            if (lens.Length == 0)
                return counts[key] = s.Any(c => c == '#') ? 0 : 1;
            int len = lens[0];
            int max = s.Length - lens.Length - Math.Max(len, lens.Sum() - 1);
            int k = s[..lens[0]].Count(c => c != '.');
            for (int first = 0, last = len; first <= max; )
            {
                char c = s[first++], d = s[last++];
                if (k == len && d != '#')
                    count += Count(s[last..], lens[1..], counts, key + last * 16 + 1);
                if (c == '#')
                    return counts[key] = count;
                k += (d == '.' ? 0 : 1) - (c == '.' ? 0 : 1);
            }
            if (k == len && lens.Length == 1)
                ++count;
            return counts[key] = count;
        }

        private static (string, int[])[] Parse(string path) =>
            File.ReadAllLines(path)
                .Select(s => s.Split(' '))
                .Select(ParseOne)
                .ToArray();

        private static (string, int[]) ParseOne(string[] ss) =>
            (ss[0], ss[1].Split(',').Select(int.Parse).ToArray());
    }
}
