using System;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day06
{
    class Program
    {
        static void Main(string[] args)
        {
            var ss = File.ReadAllLines(args[0])
                .Select(s => s.Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .ToArray();
            Console.WriteLine(Part1(ss));
            Console.WriteLine(Part2(ss));
        }

        private static int Part1(string[][] ss) =>
            ss[0].Select(long.Parse).Zip(ss[1].Select(long.Parse)).Product(Count);

        private static int Part2(string[][] ss) =>
            Count((long.Parse(string.Concat(ss[0])), long.Parse(string.Concat(ss[1]))));

        private static int Count((long time, long distance) tuple) =>
            Enumerable.Range(1, (int)tuple.time - 1).Count(i => (tuple.time - i) * i >= tuple.distance);
    }
}
