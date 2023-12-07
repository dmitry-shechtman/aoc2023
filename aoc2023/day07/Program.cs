using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day07
{
    class Program
    {
        private const char Joker = 'J';

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

        private static int Solve((string hand, int bid)[] hands, string cards, Func<IEnumerable<char>, int> getType) =>
            hands
                .OrderBy(t => getType(t.hand) << 20 | t.hand.Aggregate(0, (a, c) => a << 4 | cards.IndexOf(c)))
                .Select((t, i) => t.bid * (hands.Length - i))
                .Sum();

        private static int GetType(IEnumerable<char> hand)
        {
            var group = hand.GroupBy(c => c)
                .Select(g => g.Count())
                .OrderBy(g => g)
                .ToArray();
            return group.Length switch
            {
                2 when group[^1] is 4 => 3,
                3 when group[^1] is 3 => 5,
                _ => group.Length * 2,
            };
        }

        private static int GetType2(IEnumerable<char> hand) =>
            hand.Min(c => GetType(hand.Select(d => d == Joker ? c : d)));

        private static (string, int)[] Parse(string path) =>
            File.ReadAllLines(path)
                .Select(s => s.Split(' '))
                .Select(tt => (tt[0], int.Parse(tt[1])))
                .ToArray();
    }
}
