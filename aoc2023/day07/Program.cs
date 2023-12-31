﻿using System;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day07
{
    class Program
    {
        static void Main(string[] args)
        {
            var hands = Parse(args[0]);
            Console.WriteLine(Part1(hands));
            Console.WriteLine(Part2(hands));
        }

        private static int Part1((string, int)[] hands) =>
            Solve(hands, "AKQJT98765432", GetType);

        private static int Part2((string, int)[] hands) =>
            Solve(hands, "AKQT98765432J", GetType2);

        private static int Solve((string hand, int bid)[] hands, string cards, Func<string, int> getType) =>
            hands
                .OrderBy(t => GetKey(t.hand, cards, getType))
                .Select((t, i) => t.bid * (hands.Length - i))
                .Sum();

        private static int GetKey(string hand, string cards, Func<string, int> getType) =>
            hand.Aggregate(getType(hand), (a, c) => a << 4 | cards.IndexOf(c));

        private static int GetType(string hand)
        {
            var group = hand.GroupBy(c => c)
                .Select(g => g.Count())
                .OrderBy(v => v)
                .ToArray();
            return group.Length * 5 - group[^1];
        }

        private static int GetType2(string hand) =>
            hand.Min(c => GetType(hand.Replace('J', c)));

        private static (string, int)[] Parse(string path) =>
            File.ReadAllLines(path)
                .Select(s => s.Split(' '))
                .Select(tt => (tt[0], int.Parse(tt[1])))
                .ToArray();
    }
}
