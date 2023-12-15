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

        private static int Part1(List<(string step, string label, int length)> tt) =>
            tt.Sum(t => GetHash(t.step));

        private static int Part2(List<(string step, string label, int length)> tt)
        {
            var boxes = Enumerable.Range(0, 256)
                .Select(_ => new List<(string, int length)>())
                .ToArray();
            tt.ForEach(t => Update(boxes, t.label, t.length));
            return boxes.Select((b, i) =>
                b.Select((l, j) =>
                    (i + 1) * (j + 1) * l.length).Sum())
                .Sum();
        }

        private static int GetHash(string s) =>
            Encoding.ASCII.GetBytes(s)
                .Aggregate(0, (a, b) => (a + b) * 17 % 256);

        private static void Update(List<(string label, int)>[] boxes, string label, int length)
        {
            var box = boxes[GetHash(label)];
            var index = box.FindIndex(t => t.label == label);
            if (index >= 0)
                box.RemoveAt(index);
            if (length > 0)
                box.Insert(index >= 0 ? index : box.Count, (label, length));
        }

        private static List<(string, string, int)> Parse(string path) =>
            File.ReadAllText(path).Trim().Split(',')
                .Select(s => Regex.Split(s))
                .Select(ParseOne)
                .ToList();

        private static (string, string, int) ParseOne(string[] ss) =>
            (ss[1], ss[2], ss[4].Length > 0 ? int.Parse(ss[4]) : 0);
    }
}
