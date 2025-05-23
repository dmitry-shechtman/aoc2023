﻿using System;
using System.Collections.Generic;
using System.IO;

namespace aoc.aoc2023.day25
{
    public static class Program
    {
        public static void Main()
        {
            var input = File.ReadAllText("input.txt").Trim().Split('\n');
            var count = BuildGraph(input, out var edges);
            var karger = new Karger(count, edges.ToArray());
            var result = karger.FindMinCut(3);
            var part1 = result.Size1 * result.Size2;
            Console.WriteLine(part1);
        }

        private static int BuildGraph(string[] input, out List<Edge> edges)
        {
            edges = new();
            var vertices = new Dictionary<string, int>();
            foreach (var line in input)
            {
                var split = line.Split(':', StringSplitOptions.TrimEntries);
                var v = split[0];
                var index = vertices.GetOrAdd(v, vertices.Count);
                foreach (var u in split[1].Split(' '))
                    edges.Add(new(index, vertices.GetOrAdd(u, vertices.Count)));
            }
            return vertices.Count;
        }
    }
}
