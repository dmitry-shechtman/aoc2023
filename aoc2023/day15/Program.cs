using System;
using System.Collections.Generic;
using System.IO;

namespace aoc.aoc2023.day15
{
    class Program
    {
        record Step(int Offset, int LEnd, int SEnd);

        static void Main(string[] args)
        {
            var tt = Parse(args[0], out var bytes);
            Console.WriteLine(Part1(tt, bytes));
            Console.WriteLine(Part2(tt, bytes));
        }

        private static int Part1(List<Step> steps, byte[] bytes)
        {
            int total = 0;
            for (int i = 0; i < steps.Count; i++)
                total += GetHash(bytes, steps[i].Offset, steps[i].SEnd);
            return total;
        }

        private static int Part2(List<Step> steps, byte[] bytes)
        {
            var boxes = new List<Step>[256];
            for (int i = 0; i < 256; i++)
                boxes[i] = new();
            for (int i = 0; i < steps.Count; i++)
                Update(boxes, steps[i], bytes);
            int total = 0;
            for (int i = 0; i < 256; i++)
                for (int j = 0; j < boxes[i].Count; j++)
                    total += (i + 1) * (j + 1) * (bytes[boxes[i][j].LEnd + 1] - '0');
            return total;
        }

        private static int GetHash(byte[] bytes, int offset, int end)
        {
            int hash = 0;
            for (int i = offset; i < end; i++)
                hash = (hash + bytes[i]) * 17 % 256;
            return hash;
        }

        private static void Update(List<Step>[] boxes, Step step, byte[] bytes)
        {
            var box = boxes[GetHash(bytes, step.Offset, step.LEnd)];
            int index = FindIndex(box, step, bytes);
            if (index >= 0)
                box.RemoveAt(index);
            if (bytes[step.LEnd] == '=')
                box.Insert(index >= 0 ? index : box.Count, step);
        }

        private static int FindIndex(List<Step> box, Step step, byte[] bytes)
        {
            for (int i = 0; i < box.Count; i++)
                if (LabelEquals(box[i], step, bytes))
                    return i;
            return -1;
        }

        private static bool LabelEquals(Step left, Step right, byte[] bytes)
        {
            if (left.LEnd - left.Offset != right.LEnd - right.Offset)
                return false;
            for (int j = 0; j < right.LEnd - right.Offset; j++)
                if (bytes[left.Offset + j] != bytes[right.Offset + j])
                    return false;
            return true;
        }

        private static List<Step> Parse(string path, out byte[] bytes)
        {
            bytes = File.ReadAllBytes(path);
            List<Step> steps = new();
            for (int i = 0, j = 0; j < bytes.Length && bytes[j] != '\n'; ++j)
            {
                if (bytes[j] == '-' || bytes[j] == '=')
                {
                    int k = j + (bytes[j] == '-' ? 1 : 2);
                    steps.Add(new(i, j, k));
                    j = k;
                    i = j + 1;
                }
            }
            return steps;
        }
    }
}
