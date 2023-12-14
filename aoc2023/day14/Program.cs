using System;
using System.Collections.Generic;
using System.IO;

namespace aoc.aoc2023.day14
{
    class Program
    {
        private const char Empty = '.', Round = 'O';

        static void Main(string[] args)
        {
            string s = File.ReadAllText(args[0]);
            Console.WriteLine(Part1(s));
            Console.WriteLine(Part2(s));
        }

        private static int Part1(string s)
        {
            int width1 = s.IndexOf('\n') + 1;
            var r = s.ToCharArray();
            TiltNorth(r, width1);
            return GetLoad(r, width1);
        }

        private static int Part2(string s)
        {
            int width1 = s.IndexOf('\n') + 1;
            List<string> history = new();
            int i;
            for (char[] r = s.ToCharArray(); (i = history.IndexOf(s)) < 0; s = GetNext(r, width1))
                history.Add(s);
            return GetLoad(history[(1000000000 - i) % (history.Count - i) + i], width1);
        }

        private static int GetLoad(ReadOnlySpan<char> s, int width1)
        {
            int load = 0;
            for (int i = 0; i < s.Length; i++)
                if (s[i] == Round)
                    load += width1 - 1 - i / width1;
            return load;
        }

        private static string GetNext(char[] r, int width1)
        {
            TiltNorth(r, width1);
            TiltWest (r, width1);
            TiltSouth(r, width1);
            TiltEast (r, width1);
            return new(r);
        }

        private static void TiltNorth(char[] r, int width1)
        {
            for (int i = 0, j; i < r.Length; i++)
            {
                if (r[i] == Round)
                {
                    for (j = i; j >= width1; j -= width1)
                        if (r[j - width1] != Empty)
                            break;
                    r[i] = Empty;
                    r[j] = Round;
                }
            }
        }

        private static void TiltWest(char[] r, int width1)
        {
            for (int i = 0, j; i < r.Length; i++)
            {
                if (r[i] == Round)
                {
                    for (j = i; j % width1 > 0; --j)
                        if (r[j - 1] != Empty)
                            break;
                    r[i] = Empty;
                    r[j] = Round;
                }
            }
        }

        private static void TiltSouth(char[] r, int width1)
        {
            for (int i = r.Length - 1, j; i >= 0; i--)
            {
                if (r[i] == Round)
                {
                    for (j = i; j < r.Length - width1; j += width1)
                        if (r[j + width1] != Empty)
                            break;
                    r[i] = Empty;
                    r[j] = Round;
                }
            }
        }

        private static void TiltEast(char[] r, int width1)
        {
            for (int i = r.Length - 1, j; i >= 0; i--)
            {
                if (r[i] == Round)
                {
                    for (j = i; j % width1 + 1 < width1; ++j)
                        if (r[j + 1] != Empty)
                            break;
                    r[i] = Empty;
                    r[j] = Round;
                }
            }
        }
    }
}
