using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day03
{
    class Program
    {
        static void Main(string[] args)
        {
            string s = File.ReadAllText(args[0]).Trim();
            int width = s.IndexOf('\n') + 1;
            int height = s.Length / width;
            Console.WriteLine(Part1(s, width, height));
            Console.WriteLine(Part2(s, width, height));
        }

        private static int Part1(string s, int width, int height) =>
            GetParts(s, width, height)
                .Sum(p => p.num);

        private static int Part2(string s, int width, int height)
        {
            var parts = GetParts(s, width, height).ToArray();
            int total = 0, x, y;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '*')
                {
                    x = i % width;
                    y = i / width;
                    var adj = parts.Where(p => y >= p.y - 1 && y <= p.y + 1 && x >= p.x1 - 1 && x <= p.x2);
                    if (adj.Count() == 2)
                        total += adj.Product(p => p.num);
                }
            }
            return total;
        }

        private static IEnumerable<(int x1, int x2, int y, int num)> GetParts(string s, int width, int height)
        {
            (int x1, int x, int y, int num) p = (-1, 0, 0, 0);
            for (int i = 0; i <= s.Length; i++)
            {
                char c = i < s.Length ? s[i] : default;
                if (!char.IsDigit(c) && IsPartNumber(p, s, width, height))
                    yield return p;
                p = char.IsDigit(c)
                    ? (p.x1 >= 0 ? p.x1 : p.x, p.x + 1, p.y, p.num * 10 + c - '0')
                    : c != '\n' ? (-1, p.x + 1, p.y, 0) : (-1, 0, p.y + 1, 0);
            }
        }

        private static bool IsPartNumber((int x1, int x2, int y, int) p, string s, int width, int height)
        {
            if (p.x1 >= 0)
                for (int y = Math.Max(0, p.y - 1); y <= Math.Min(height - 1, p.y + 1); y++)
                    for (int x = Math.Max(0, p.x1 - 1), i = y * width + x; x <= Math.Min(width - 2, p.x2); x++, i++)
                        if (s[i] != '.' && !char.IsDigit(s[i]))
                            return true;
            return false;
        }
    }
}
