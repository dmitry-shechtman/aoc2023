using System;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day01
{
    class Program
    {
        private static readonly string[] Digits =
        {
            "1",   "2",   "3",     "4",    "5",    "6",   "7",     "8",     "9",
            "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"
        };

        static void Main(string[] args)
        {
            var ss = File.ReadAllLines(args[0]);
            Console.WriteLine(Part1(ss));
            Console.WriteLine(Part2(ss));
        }

        private static int Part1(string[] ss) =>
            ss.Sum(s => int.Parse($"{s.First(char.IsDigit)}{s.Last(char.IsDigit)}"));

        private static int Part2(string[] ss) =>
            ss.Sum(GetFirst) * 10 + ss.Sum(GetLast);

        private static int GetFirst(string s) =>
            Digits.Select((t, v) => (v, s.Contains(t) ? s.IndexOf(t) : s.Length))
                .OrderBy(t => t.Item2)
                .First().v % 9 + 1;

        private static int GetLast(string s) =>
            Digits.Select((t, v) => (v, s.LastIndexOf(t)))
                .OrderByDescending(t => t.Item2)
                .First().v % 9 + 1;
    }
}
