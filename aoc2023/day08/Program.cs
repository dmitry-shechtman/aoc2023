﻿using System;
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

        private static long Part2(int[] dirs, Dictionary<string, string[]> map) =>
            map.Keys.Lcm(s => s[^1] == 'A' ? GetCycle(s, dirs, map) : 1);

        private static long GetCycle(string node, int[] dirs, Dictionary<string, string[]> map)
        {
            HashSet<(string, int)> steps = new();
            int step = 0;
            while (steps.Add((node, step)))
                (node, step) = (map[node][dirs[step]], (step + 1) % dirs.Length);
            return steps.Count - step;
        }

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
