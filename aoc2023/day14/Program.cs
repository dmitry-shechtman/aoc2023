using System;
using System.Collections.Generic;
using System.IO;

namespace aoc.aoc2023.day14
{
    class Program
    {
        static void Main(string[] args)
        {
            string s = File.ReadAllText(args[0]);
            Console.WriteLine(Part1(s));
            Console.WriteLine(Part2(s));
        }

        private static int Part1(string s)
        {
            Dish dish = new(s);
            var r = dish.TiltNorth();
            return dish.GetLoad(new(r));
        }

        private static int Part2(string s)
        {
            Dish dish = new(s);
            List<string> history = new();
            int i;
            for (; (i = history.IndexOf(s)) < 0; s = dish.GetNext())
                history.Add(s);
            return dish.GetLoad(history[(1000000000 - i) % (history.Count - i) + i]);
        }
    }

    class Dish
    {
        private const char Empty = '.', Round = 'O', Square = '#', NewLine = '\n';

        private readonly char[] r;
        private readonly int width1;

        public Dish(string input)
        {
            r = input.ToCharArray();
            width1 = input.IndexOf('\n') + 1;
        }

        public int GetLoad(string s)
        {
            int load = 0;
            for (int i = 0; i < s.Length; i++)
                if (s[i] == Round)
                    load += width1 - 1 - i / width1;
            return load;
        }

        public string GetNext()
        {
            TiltNorth();
            TiltWest();
            TiltSouth();
            TiltEast();
            return new(r);
        }

        public char[] TiltNorth() =>
            Tilt(0, 1, 0, width1);

        private void TiltWest() =>
            Tilt(0, 1);

        private void TiltSouth() =>
            Tilt(width1 - 2, -1, r.Length - width1, -width1);

        private void TiltEast() =>
            Tilt(r.Length - 1, -1);

        private char[] Tilt(int start, int step, int offset, int step2)
        {
            for (int i = start; i >= 0 && i < width1 - 1; i += step)
                Tilt(i + offset, step2);
            return r;
        }

        private void Tilt(int start, int step)
        {
            for (int i = start, j = i - step; i >= 0 && i < r.Length; i += step)
            {
                switch (r[i])
                {
                    case Round:
                        r[i] = Empty;
                        r[j += step] = Round;
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
