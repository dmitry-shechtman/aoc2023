using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace aoc.aoc2023.day14
{
    class Program
    {
        private const byte Empty = 0x2E, Round = 0x4F, Square = 0x23, NewLine = 0x0A;

        static void Main(string[] args)
        {
            string s = File.ReadAllText(args[0]);
            Console.WriteLine(Part1(s));
            Console.WriteLine(Part2(s));
        }

        private static int Part1(string s)
        {
            int width1 = s.IndexOf('\n') + 1;
            var r = Encoding.ASCII.GetBytes(s);
            TiltNorth(r, width1);
            return GetLoad(Encoding.ASCII.GetString(r), width1);
        }

        private static int Part2(string s)
        {
            int width1 = s.IndexOf('\n') + 1;
            List<string> history = new();
            int i;
            for (var r = Encoding.ASCII.GetBytes(s); (i = history.IndexOf(s)) < 0; s = GetNext(r, width1))
                history.Add(s);
            return GetLoad(history[(1000000000 - i) % (history.Count - i) + i], width1);
        }

        private static int GetLoad(string s, int width1)
        {
            int load = 0;
            for (int i = 0; i < s.Length; i++)
                if (s[i] == Round)
                    load += width1 - 1 - i / width1;
            return load;
        }

        private static string GetNext(byte[] r, int width1)
        {
            TiltNorth(r, width1);
            TiltWest (r);
            TiltSouth(r, width1);
            TiltEast (r);
            return Encoding.ASCII.GetString(r);
        }

        private static void TiltNorth(byte[] r, int width1)
        {
            for (int x = 0; x < width1 - 1; ++x)
            {
                for (int i = x, j = i - width1; i < r.Length; i += width1)
                {
                    switch (r[i])
                    {
                        case Round:
                            r[i] = Empty;
                            r[j += width1] = Round;
                            break;
                        case Square:
                            j = i;
                            break;
                    }
                }
            }
        }

        private static void TiltWest(byte[] r)
        {
            for (int i = 0, j = -1; i < r.Length; ++i)
            {
                switch (r[i])
                {
                    case Round:
                        r[i] = Empty;
                        r[++j] = Round;
                        break;
                    case Square:
                    case NewLine:
                        j = i;
                        break;
                }
            }
        }

        private static void TiltSouth(byte[] r, int width1)
        {
            for (int x = width1 - 2; x >= 0; --x)
            {
                for (int j = r.Length + x, i = j - width1; i >= 0; i -= width1)
                {
                    switch (r[i])
                    {
                        case Round:
                            r[i] = Empty;
                            r[j -= width1] = Round;
                            break;
                        case Square:
                            j = i;
                            break;
                    }
                }
            }
        }

        private static void TiltEast(byte[] r)
        {
            for (int j = r.Length, i = j - 1; i >= 0; --i)
            {
                switch (r[i])
                {
                    case Round:
                        r[i] = Empty;
                        r[--j] = Round;
                        break;
                    case Square:
                    case NewLine:
                        j = i;
                        break;
                }
            }
        }
    }
}
