using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day09
{
    class Program
    {
        static void Main(string[] args)
        {
            var values = Parse(args[0]);
            Console.WriteLine(Part1(values));
            Console.WriteLine(Part2(values));
        }

        private static int Part1(int[][] values) =>
            values.Select(GetDiffs)
                .Sum(aa => aa.Sum(a => a[^1]));

        private static int Part2(int[][] values) =>
            values.Select(GetDiffs)
                .Sum(aa => aa.Reverse().Aggregate(0, (a, v) => v[0] - a));

        private static IEnumerable<int[]> GetDiffs(int[] a)
        {
            yield return a;
            while (a.Any(v => v != 0))
                yield return a = a[1..].Select((v, i) => v - a[i]).ToArray();
        }

        private static int[][] Parse(string path) =>
            File.ReadAllLines(path)
                .Select(s => s.Split(' ').Select(int.Parse).ToArray())
                .ToArray();
    }
}
