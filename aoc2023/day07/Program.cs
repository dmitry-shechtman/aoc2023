using System;
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
                .OrderBy(t => getType(t.hand) << 20 | GetTieKey(t.hand, cards))
                .Select((t, i) => t.bid * (hands.Length - i))
                .Sum();

        private static int GetTieKey(string hand, string cards) =>
            hand.Aggregate(0, (a, c) => a << 4 | cards.IndexOf(c));

        private static int GetType(string hand)
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

        private static int GetType2(string hand) =>
            hand.Min(c => GetType(hand.Replace('J', c)));

        private static (string, int)[] Parse(string path) =>
            File.ReadAllLines(path)
                .Select(s => s.Split(' '))
                .Select(tt => (tt[0], int.Parse(tt[1])))
                .ToArray();
    }
}
