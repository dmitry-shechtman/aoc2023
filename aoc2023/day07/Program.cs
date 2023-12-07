using System;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day07
{
    class Program
    {
        private const int Joker = 12;

        static void Main(string[] args)
        {
            var hands = Parse(args[0]);
            Console.WriteLine(Part1(hands));
            Console.WriteLine(Part2(hands));
        }

        private static int Part1((string, int)[] hands) =>
            Solve(hands, "AKQJT98765432", GetType);

        private static int Part2((string, int)[] hands) =>
            Solve(hands, "AKQT98765432J", t => GetType(t, 0));

        private static int Solve((string hand, int bid)[] hands, string cards, Func<int[], int> getType) =>
            hands
                .Select(t => (hand: t.hand.Select(c => cards.IndexOf(c)).ToArray(), t.bid))
                .OrderBy(t => getType(t.hand) << 20 | t.hand.Aggregate(0, (a, c) => a << 4 | c))
                .Select((t, i) => t.bid * (hands.Length - i))
                .Sum();

        private static int GetType(int[] hand)
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

        private static int GetType(int[] hand, int index) =>
            index == hand.Length
                ? GetType(hand)
                : hand[index] < Joker
                    ? GetType(hand, index + 1)
                    : Enumerable.Range(0, Joker)
                        .Min(j => GetType(hand, index, j));

        private static int GetType(int[] hand, int index, int card)
        {
            hand[index] = card;
            int result = GetType(hand, index + 1);
            hand[index] = Joker;
            return result;
        }

        private static (string, int)[] Parse(string path) =>
            File.ReadAllLines(path)
                .Select(s => s.Split(' '))
                .Select(tt => (tt[0], int.Parse(tt[1])))
                .ToArray();
    }
}
