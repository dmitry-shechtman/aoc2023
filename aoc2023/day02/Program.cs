using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day02
{
    class Program
    {
        record Game(int Id, Dictionary<string, int>[] Sets);

        private static readonly Dictionary<string, int> Cubes = new()
            { { "red", 12 }, { "green", 13 }, { "blue", 14 } };

        static void Main(string[] args)
        {
            var games = Parse(args[0]);
            Console.WriteLine(Part1(games));
            Console.WriteLine(Part2(games));
        }

        private static int Part1(Game[] games) =>
            games.Sum(game => game.Sets.All(set =>
                !set.Any(kvp => kvp.Value > Cubes[kvp.Key])) ? game.Id : 0);

        private static int Part2(Game[] games) =>
            games.Sum(game => Cubes.Keys.Product(key =>
                game.Sets.Max(set => set.GetValueOrDefault(key))));

        private static Game[] Parse(string path) =>
            File.ReadAllLines(path)
                .Select(s => s.Split(": "))
                .Select(tt => new Game(
                    int.Parse(tt[0].Split(' ')[1]),
                    tt[1].Split("; ")
                        .Select(t => t.Split(", ")
                            .Select(u => u.Split(' '))
                            .ToDictionary(vv => vv[1], vv => int.Parse(vv[0])))
                        .ToArray()))
                .ToArray();
    }
}
