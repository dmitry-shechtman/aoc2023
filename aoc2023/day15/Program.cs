using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace aoc.aoc2023.day15
{
    class Program
    {
        private static readonly Regex Regex = new(@"^(([a-z]+)(-|=([1-9])))$");

        static void Main(string[] args)
        {
            var tt = Parse(args[0]);
            Console.WriteLine(Part1(tt));
            Console.WriteLine(Part2(tt));
        }

        private static int Part1(IEnumerable<(string step, string label, int length)> tt) =>
            tt.Sum(t => GetHash(t.step));

        private static int Part2(IEnumerable<(string step, string label, int length)> tt)
        {
            var boxes = Enumerable.Range(0, 256)
                .Select(_ => new List<(string, int)>())
                .ToArray();
            return tt.Sum(t => Update(boxes, t.label, t.length));
        }

        private static int GetHash(string s) =>
            Encoding.ASCII.GetBytes(s)
                .Aggregate(0, (a, b) => (a + b) * 17 % 256);

        private static int Update(List<(string label, int length)>[] boxes, string label, int length)
        {
            var boxIndex = GetHash(label);
            var box = boxes[boxIndex];
            var lensIndex = box.FindIndex(t => t.label == label);
            var delta = 0;
            if (lensIndex >= 0)
            {
                delta -= GetDelta(box, boxIndex, lensIndex);
                box.RemoveAt(lensIndex);
            }
            if (length > 0)
            {
                if (lensIndex < 0)
                    lensIndex = box.Count;
                box.Insert(lensIndex, (label, length));
                delta += GetDelta(box, boxIndex, lensIndex);
            }
            return delta;
        }

        private static int GetDelta(List<(string label, int length)> box, int boxIndex, int lensIndex) =>
            (boxIndex + 1) *
                (box.Skip(lensIndex).Sum(t => t.length) +
                lensIndex * box[lensIndex].length);

        private static IEnumerable<(string, string, int)> Parse(string path) =>
            File.ReadAllText(path).Trim().Split(',')
                .Select(s => Regex.Split(s))
                .Select(ParseOne);

        private static (string, string, int) ParseOne(string[] ss) =>
            (ss[1], ss[2], ss[4].Length > 0 ? int.Parse(ss[4]) : 0);
    }
}
