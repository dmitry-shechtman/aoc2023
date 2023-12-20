using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day20
{
    abstract record Module(int[] Dests)
    {
        public abstract bool Transform(BitArray state, bool pulse);
    }

    record RelayModule(int[] Dests) : Module(Dests)
    {
        public override bool Transform(BitArray state, bool pulse) =>
            pulse;
    }

    record FlipFlopModule(int Index, int[] Dests) : Module(Dests)
    {
        public override bool Transform(BitArray state, bool pulse) =>
            !state[Index];
    }

    record ConjunctionModule(int[] Dests) : Module(Dests)
    {
        public int[] Sources { get; set; }
        public override bool Transform(BitArray state, bool pulse) =>
            !Sources.All(src => state[src]);
    }

    class Program
    {
        private const string StartKey = "broadcaster";

        static void Main(string[] args)
        {
            var (modules, indices) = GetModules(args[0]);
            var start = indices[StartKey];
            var state = new BitArray(modules.Length);
            var lengths = CreateLengths(modules);
            var counts = new int[2];
            Console.WriteLine(Part1(modules, start, state, counts, lengths));
            Console.WriteLine(Part2(modules, start, state, counts, lengths));
        }

        private static int Part1(Module[] modules, int start, BitArray state, int[] counts, long[] lengths)
        {
            for (int i = 1; i <= 1000; i++)
                Step(modules, start, state, counts, lengths, i);
            return counts.Product();
        }

        private static long Part2(Module[] modules, int start, BitArray state, int[] counts, long[] lengths)
        {
            for (int i = 1001; !lengths.All(v => v > 0); i++)
                Step(modules, start, state, counts, lengths, i);
            return lengths.Lcm();
        }

        private static void Step(Module[] modules, int start, BitArray state, int[] counts, long[] lengths, int step)
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
                state[curr] = pulse = module.Transform(state, pulse);
                if (pulse && lengths[curr] == 0)
                    lengths[curr] = step;
                foreach (var next in module.Dests)
                    queue.Enqueue((pulse, next));
            }
        }

        private static (Module[], Dictionary<string, int>) GetModules(string path)
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
                    conjunction.Sources = GetSources(i, modules);
            return (modules, indices);
        }

        private static long[] CreateLengths(Module[] modules) =>
            modules
                .Select(m => m is ConjunctionModule ? 0L : 1L)
                .ToArray();

        private static Module CreateModule(int index, string key, string[] dests, Dictionary<string, int> indices) =>
            CreateModule(index, key, GetDestinations(dests, indices));

        private static Module CreateModule(int index, string key, int[] dests) => key[0] switch
        {
            '%' => new FlipFlopModule(index, dests),
            '&' => new ConjunctionModule(dests),
            _ => new RelayModule(dests),
        };

        private static string GetKey((string key, string[] dests) tuple) =>
            tuple.key.TrimStart('%', '&');

        private static int[] GetDestinations(string[] dests, Dictionary<string, int> indices) =>
            dests.Select(d => indices[d]).ToArray();

        private static int[] GetSources(int index, Module[] modules) =>
            modules.Select()
                .Where(t => t.Value.Dests.Contains(index))
                .Select(t => t.Index)
                .ToArray();

        private static List<(string key, string[] dests)> Parse(string path) =>
            File.ReadAllLines(path)
                .Select(s => s.Split(" -> "))
                .Select(ParseOne)
                .ToList();

        private static (string key, string[] dests) ParseOne(string[] tt) =>
            (tt[0], tt[1].Split(',', StringSplitOptions.TrimEntries));
    }
}
