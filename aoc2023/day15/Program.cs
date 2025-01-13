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
        static void Main(string[] args)
        {
            var tt = Item.ParseAll(args[0]);
            Console.WriteLine(Part1(tt));
            Console.WriteLine(Part2(tt));
        }

        private static int Part1(List<Item> tt) =>
            tt.Sum(t => GetHash(t.Step));

        private static int Part2(List<Item> tt)
        {
            var boxes = Enumerable.Range(0, 256)
                .Select(_ => new LinkedList<Slot>())
                .ToArray();
            tt.ForEach(t => t.Update(boxes[GetHash(t.Value.Label)]));
            return boxes.Sum((b, i) =>
                b.Sum((l, j) =>
                    (i + 1) * (j + 1) * l.Length));
        }

        private static int GetHash(string s) =>
            Encoding.ASCII.GetBytes(s)
                .Aggregate(0, (a, b) => (byte)((a + b) * 17));
    }

    readonly record struct Slot(string Label, int Length);

    readonly record struct Item(string Step, Slot Value)
    {
        private static readonly Regex Regex = new(@"^(([a-z]+)(-|=([1-9])))$");

        public void Update(LinkedList<Slot> box)
        {
            var (label, length) = Value;
            var node = box.Find(t => t.Label == label);
            if (node is not null)
                if (length == 0)
                    box.Remove(node);
                else
                    node.Value = Value;
            else if (length > 0)
                box.AddLast(Value);
        }

        public static List<Item> ParseAll(string path) =>
            File.ReadAllText(path).Trim().Split(',')
                .Select1(Regex.Split)
                .Select(ParseOne)
                .ToList();

        public static Item ParseOne(string[] ss) =>
            new(ss[1], new(ss[2], ss[4].Length > 0 ? int.Parse(ss[4]) : 0));
    }
}
