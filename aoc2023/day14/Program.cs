using System;
using System.Collections.Generic;
using System.IO;

namespace aoc.aoc2023.day14
{
    class Program
    {
        static void Main(string[] args)
        {
            var rocks = Parse(args[0]);
            Console.WriteLine(Part1(rocks));
            Console.WriteLine(Part2(rocks));
        }

        private static int Part1(int[,] rocks) =>
            GetLoad(TiltNorth(rocks));

        private static int Part2(int[,] rocks)
        {
            List<int[,]> history = new();
            int i;
            for (; (i = history.FindIndex(r => ArrayEquals(rocks, r))) < 0; rocks = GetNext(rocks))
                history.Add(rocks);
            return GetLoad(history[(1000000000 - i) % (history.Count - i) + i]);
        }

        private static int GetLoad(int[,] rocks)
        {
            int load = 0;
            int width = rocks.GetLength(0), height = rocks.GetLength(1);
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    if (rocks[x, y] == 2)
                        load += height - y;
            return load;
        }

        private static bool ArrayEquals(int[,] left, int[,] right)
        {
            int width = left.GetLength(0), height = left.GetLength(1);
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    if (left[x, y] != right[x, y])
                        return false;
            return true;
        }

        private static int[,] GetNext(int[,] rocks) =>
            TiltEast(TiltSouth(TiltWest(TiltNorth(rocks))));

        private static int[,] TiltNorth(int[,] rocks)
        {
            int width = rocks.GetLength(0), height = rocks.GetLength(1);
            var rocks2 = new int[width, height];
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    if (rocks[x, y] == 2)
                        rocks2[x, SlideNorth(rocks2, x, y)] = 2;
                    else
                        rocks2[x, y] = rocks[x, y];
            return rocks2;
        }

        private static int[,] TiltWest(int[,] rocks)
        {
            int width = rocks.GetLength(0), height = rocks.GetLength(1);
            var rocks2 = new int[width, height];
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    if (rocks[x, y] == 2)
                        rocks2[SlideWest(rocks2, x, y), y] = 2;
                    else
                        rocks2[x, y] = rocks[x, y];
            return rocks2;
        }

        private static int[,] TiltSouth(int[,] rocks)
        {
            int width = rocks.GetLength(0), height = rocks.GetLength(1);
            var rocks2 = new int[width, height];
            for (int x = 0; x < width; ++x)
                for (int y = height - 1; y >= 0; --y)
                    if (rocks[x, y] == 2)
                        rocks2[x, SlideSouth(rocks2, x, y, height)] = 2;
                    else
                        rocks2[x, y] = rocks[x, y];
            return rocks2;
        }

        private static int[,] TiltEast(int[,] rocks)
        {
            int width = rocks.GetLength(0), height = rocks.GetLength(1);
            var rocks2 = new int[width, height];
            for (int x = width - 1; x >= 0; --x)
                for (int y = 0; y < height; ++y)
                    if (rocks[x, y] == 2)
                        rocks2[SlideEast(rocks2, x, y, width), y] = 2;
                    else
                        rocks2[x, y] = rocks[x, y];
            return rocks2;
        }

        private static int SlideNorth(int[,] rocks, int x, int y)
        {
            for (int i = y - 1; i >= 0; --i)
                if (rocks[x, i] != 0)
                    return i + 1;
            return 0;
        }

        private static int SlideWest(int[,] rocks, int x, int y)
        {
            for (int i = x - 1; i >= 0; --i)
                if (rocks[i, y] != 0)
                    return i + 1;
            return 0;
        }

        private static int SlideSouth(int[,] rocks, int x, int y, int height)
        {
            for (int i = y + 1; i < height; ++i)
                if (rocks[x, i] != 0)
                    return i - 1;
            return height - 1;
        }

        private static int SlideEast(int[,] rocks, int x, int y, int width)
        {
            for (int i = x + 1; i < width; ++i)
                if (rocks[i, y] != 0)
                    return i - 1;
            return width - 1;
        }

        private static int[,] Parse(string path)
        {
            var ss = File.ReadAllLines(path);
            int width = ss[0].Length, height = ss.Length;
            var rocks = new int[width, height];
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                    rocks[x, y] = ".#O".IndexOf(ss[y][x]);
            return rocks;
        }
    }
}
