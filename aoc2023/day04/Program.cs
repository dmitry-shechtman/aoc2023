using System;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day04
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = Parse(args[0]);
            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        private static int Part1(int[] input) =>
            input.Sum(c => 1 << c >> 1);

        private static int Part2(int[] input)
        {
            var output = Enumerable.Repeat(1, input.Length).ToArray();
            for (int i = 0; i < input.Length; ++i)
                for (int j = 1; j <= input[i] && i + j < input.Length; ++j)
                    output[i + j] += output[i];
            return output.Sum();
        }

        private static int[] Parse(string path) =>
            File.ReadAllLines(path)
                .Select(ParseOne)
                .Select(tt => tt[0].Intersect(tt[1]).Count())
                .ToArray();

        private static string[][] ParseOne(string s) =>
            s.Split(": ")[1].Split(" | ")
                .Select(s => s.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .ToArray();
    }
}
