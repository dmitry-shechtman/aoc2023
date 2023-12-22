using System;
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
            Console.WriteLine(Part1(bricks));
            Console.WriteLine(Part2(bricks));
        }

        private static int Part1(Brick[] bricks) =>
            bricks.Count(b => bricks.All((_, i) => CountSupports(b, bricks, i) != 1));

        private static int Part2(Brick[] bricks) =>
            bricks.Sum((b, i) => CountAllSupports(b, bricks.ToArray(), i));

        private static void Drop(Brick[] bricks)
        {
            for (int i = 0; i < bricks.Length; i++)
                bricks[i] -= (0, 0, GetHeight(bricks[i], bricks));
        }

        private static int GetHeight(Brick brick, Brick[] bricks) =>
            bricks.Where(b => Below(b, brick))
                .TryMin(b => brick.Min.z - b.Max.z, brick.Min.z) - 1;

        private static int CountAllSupports(Brick brick, Brick[] bricks, int i)
        {
            bricks[i] = default;
            return Enumerable.Range(i, bricks.Length - i)
                .Where(j => CountSupports(brick, bricks, j) == 0)
                .Sum(j => 1 + CountAllSupports(bricks[j], bricks, j));
        }

        private static int CountSupports(Brick brick, Brick[] bricks, int i) =>
            Supports(brick, bricks[i])
                ? bricks[..i].Count(b => Supports(b, bricks[i]))
                : -1;

        private static bool Supports(Brick brick, Brick brick2) =>
            brick.Max.z == brick2.Min.z - 1 && IsMatch(brick2, brick);

        private static bool Below(Brick brick, Brick brick2) =>
            brick.Max.z < brick2.Min.z && IsMatch(brick2, brick);

        private static bool IsMatch(Brick brick, Brick brick2) =>
            ((VectorRange)brick).IsMatch((VectorRange)brick2);

        private static Brick[] Parse(string path) =>
            File.ReadAllLines(path)
                .Select(Brick.Parse)
                .OrderBy(r => r.Min.z)
                .ToArray();
    }
}
