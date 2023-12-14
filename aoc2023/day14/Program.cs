using System;
using System.Collections.Generic;
using System.IO;

namespace aoc.aoc2023.day14
{
    class Program
    {
        private const char Empty = '.', Round = 'O', Square = '#';

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
            TiltWest (r);
            TiltSouth(r, width1);
            TiltEast (r);
            return new(r);
        }

        private static void TiltNorth(char[] r, int width1)
        {
            for (int x = 0; x < width1 - 1; ++x)
            {
                for (int i = x, j = x; i < r.Length; i += width1)
                {
                    switch (r[i])
                    {
                        case Round:
                            r[i] = Empty;
                            r[j] = Round;
                            j += width1;
                            break;
                        case Square:
                            j = i + width1;
                            break;
                    }
                }
            }
        }

        private static void TiltWest(char[] r)
        {
            for (int i = 0, j = 0; i < r.Length; ++i)
            {
                switch (r[i])
                {
                    case Round:
                        r[i] = Empty;
                        r[j++] = Round;
                        break;
                    case Square:
                    case '\n':
                        j = i + 1;
                        break;
                }
            }
        }

        private static void TiltSouth(char[] r, int width1)
        {
            for (int x = width1 - 2; x >= 0; --x)
            {
                for (int i = r.Length - width1 + x, j = i; i >= 0; i -= width1)
                {
                    switch (r[i])
                    {
                        case Round:
                            r[i] = Empty;
                            r[j] = Round;
                            j -= width1;
                            break;
                        case Square:
                            j = i - width1;
                            break;
                    }
                }
            }
        }

        private static void TiltEast(char[] r)
        {
            for (int i = r.Length - 1, j = i; i >= 0; --i)
            {
                switch (r[i])
                {
                    case Round:
                        r[i] = Empty;
                        r[j--] = Round;
                        break;
                    case Square:
                    case '\n':
                        j = i - 1;
                        break;
                }
            }
        }
    }
}
