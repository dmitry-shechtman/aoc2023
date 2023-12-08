using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day08
{
    class Program
    {
        static void Main(string[] args)
        {
            Parse(args[0], out var dirs, out var map);
            Console.WriteLine(Part1(dirs, map));
            Console.WriteLine(Part2(dirs, map));
        }

        private static int Part1(int[] dirs, Dictionary<string, string[]> map)
        {
            int step = 0;
            for (var node = "AAA"; node != "ZZZ"; ++step)
                node = map[node][dirs[step % dirs.Length]];
            return step;
        }

        private static long Part2(int[] dirs, Dictionary<string, string[]> map)
        {
            var keys = map.Keys.ToArray();
            var values = map.Values
                .Select(t => t.Select(t => Array.IndexOf(keys, t)).ToArray())
                .ToArray();
            return keys
                .Select((s, i) => s[^1] == 'A' ? GetCycle(i, dirs, values) : 1)
                .Aggregate(Lcm);
        }

        private static long GetCycle(int start, int[] dirs, int[][] map)
        {
            HashSet<(int, int)> steps = new();
            var curr = (node: start, step: 0);
            while (steps.Add(curr))
                curr = (map[curr.node][dirs[curr.step]], (curr.step + 1) % dirs.Length);
            return steps.Count - curr.step;
        }

        private static long Lcm(long a, long b) =>
            a / Gcd(a, b) * b;

        private static long Gcd(long a, long b) =>
            b == 0 ? a : Gcd(b, a % b);

        private static void Parse(string path, out int[] dirs, out Dictionary<string, string[]> map)
        {
            var ss = File.ReadAllText(path).Split("\n\n");
            dirs = ss[0].Select(t => "LR".IndexOf(t)).ToArray();
            map = ss[1].Trim().Split('\n')
                .Select(t => t.Split(" = "))
                .ToDictionary(tt => tt[0], tt => tt[1][1..^1].Split(", "));
        }
    }
}
