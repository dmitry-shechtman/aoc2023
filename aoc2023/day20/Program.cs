using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day20
{
    abstract record Module(int[] Dests)
    {
        public abstract bool Transform(long state, bool pulse);
    }

    record RelayModule(int[] Dests) : Module(Dests)
    {
        public override bool Transform(long state, bool pulse) =>
            pulse;
    }

    record FlipFlopModule(long Mask, int[] Dests) : Module(Dests)
    {
        public override bool Transform(long state, bool pulse) =>
            (state & Mask) == 0;
    }

    record ConjunctionModule(int[] Dests) : Module(Dests)
    {
        public long SourceMask { get; set; }
        public override bool Transform(long state, bool pulse) =>
            (state & SourceMask) != SourceMask;
    }

    class Program
    {
        private const string StartKey = "broadcaster";

        static void Main(string[] args)
        {
            var modules = GetModules(args[0], out int start);
            var state = 0L;
            var counts = new int[2];
            var lengths = CreateLengths(modules);
            Console.WriteLine(Part1(modules, start, ref state, counts, lengths));
            Console.WriteLine(Part2(modules, start, ref state, counts, lengths));
        }

        private static int Part1(Module[] modules, int start, ref long state, int[] counts, long[] lengths)
        {
            for (int i = 1; i <= 1000; i++)
                Step(modules, start, ref state, counts, lengths, i);
            return counts.Product();
        }

        private static long Part2(Module[] modules, int start, ref long state, int[] counts, long[] lengths)
        {
            for (int i = 1001; !lengths.All(v => v > 0); i++)
                Step(modules, start, ref state, counts, lengths, i);
            return lengths.Lcm();
        }

        private static void Step(Module[] modules, int start, ref long state, int[] counts, long[] lengths, int step)
        {
            Queue<(bool, int)> queue = new();
            queue.Enqueue((false, start));
            while (queue.TryDequeue(out var tuple))
            {
                var (pulse, curr) = tuple;
                ++counts[pulse ? 1 : 0];
                var module = modules[curr];
                if (module is FlipFlopModule && pulse)
                    continue;
                pulse = module.Transform(state, pulse);
                state = pulse
                    ? state | 1L << curr
                    : state & ~(1L << curr);
                if (pulse && lengths[curr] == 0)
                    lengths[curr] = step;
                foreach (var next in module.Dests)
                    queue.Enqueue((pulse, next));
            }
        }

        private static Module[] GetModules(string path, out int start)
        {
            var tuples = Parse(path);
            tuples.SelectMany(t => t.dests)
                .Except(tuples.Select(GetKey))
                .ToList()
                .ForEach(d => tuples.Add((d, Array.Empty<string>())));
            var indices = tuples.Select()
                .ToDictionary(t => GetKey(t.Value), t => t.Index);
            var modules = tuples
                .Select((t, i) => CreateModule(i, t.key, t.dests, indices))
                .ToArray();
            for (int i = 0; i < modules.Length; i++)
                if (modules[i] is ConjunctionModule conjunction)
                    conjunction.SourceMask = GetSourceMask(i, modules);
            start = indices[StartKey];
            return modules;
        }

        private static long[] CreateLengths(Module[] modules) =>
            modules
                .Select(m => m is ConjunctionModule ? 0L : 1L)
                .ToArray();

        private static Module CreateModule(int index, string key, string[] dests, Dictionary<string, int> indices) =>
            CreateModule(index, key, GetDestinations(dests, indices));

        private static Module CreateModule(int index, string key, int[] dests) => key[0] switch
        {
            '%' => new FlipFlopModule(1L << index, dests),
            '&' => new ConjunctionModule(dests),
            _ => new RelayModule(dests),
        };

        private static string GetKey((string key, string[] dests) tuple) =>
            tuple.key.TrimStart('%', '&');

        private static int[] GetDestinations(string[] dests, Dictionary<string, int> indices) =>
            dests.Select(d => indices[d]).ToArray();

        private static long GetSourceMask(int index, Module[] modules) =>
            modules.Select()
                .Where(t => t.Value.Dests.Contains(index))
                .Select(t => 1L << t.Index)
                .Sum();

        private static List<(string key, string[] dests)> Parse(string path) =>
            File.ReadAllLines(path)
                .Select(s => s.Split(" -> "))
                .Select(ParseOne)
                .ToList();

        private static (string key, string[] dests) ParseOne(string[] tt) =>
            (tt[0], tt[1].Split(',', StringSplitOptions.TrimEntries));
    }
}
