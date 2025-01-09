using System;
using System.Globalization;
using System.IO;
using System.Linq;

using Brick = aoc.Vector3DRange;

namespace aoc.aoc2023.day22
{
    class Program
    {
        static void Main(string[] args)
        {
            var bricks = Parse(args[0]);
            Drop(bricks);
            var graph = CreateGraph(bricks);
            Console.WriteLine(Part1(graph));
            Console.WriteLine(Part2(graph));
        }

        private static int Part1(Graph<bool> graph) =>
            graph.Count(i => graph.All(j => CountSupports(graph, i, j) != 1));

        private static int Part2(Graph<bool> graph) =>
            graph.Sum(i => CountSupports(new(graph), i));

        private static void Drop(Brick[] bricks)
        {
            for (int i = 0; i < bricks.Length; i++)
                bricks[i] -= (0, 0, GetHeight(bricks[i], bricks));
        }

        private static Graph<bool> CreateGraph(Brick[] bricks) =>
            new((i, j) => Supports(bricks[i], bricks[j]), bricks.Length);

        private static int GetHeight(Brick brick, Brick[] bricks) =>
            bricks.Where(b => Below(b, brick))
                .MinOrDefault(b => brick.Min.z - b.Max.z, brick.Min.z) - 1;

        private static int CountSupports(Graph<bool> graph, int i) =>
            graph.Outgoing[i].Keys
                .Where(j => graph.Remove(i, j) && graph.Incoming[j].Count == 0)
                .Sum(j => 1 + CountSupports(graph, j));

        private static int CountSupports(Graph<bool> graph, int i, int j) =>
            graph.Outgoing[i].ContainsKey(j)
                ? graph.Incoming[j].Count
                : -1;

        private static bool Supports(Brick brick, Brick brick2) =>
            brick.Max.z == brick2.Min.z - 1 && IsMatch(brick2, brick);

        private static bool Below(Brick brick, Brick brick2) =>
            brick.Max.z < brick2.Min.z && IsMatch(brick2, brick);

        private static bool IsMatch(Brick brick, Brick brick2) =>
            ((VectorRange)brick).Overlaps((VectorRange)brick2);

        private static Brick[] Parse(string path) =>
            Brick.Builder.ParseAll(File.ReadAllText(path), CultureInfo.InvariantCulture)
                .OrderBy(r => r.Min.z)
                .ToArray();
    }
}
