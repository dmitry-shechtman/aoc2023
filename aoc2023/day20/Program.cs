using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day20
{
    abstract record Module(long Mask, long DestMask)
    {
        public abstract bool Transform(long state, bool pulse);
    }

    record RelayModule(long Mask, long DestMask) : Module(Mask, DestMask)
    {
        public override bool Transform(long state, bool pulse) =>
            pulse;
    }

    record FlipFlopModule(long Mask, long DestMask) : Module(Mask, DestMask)
    {
        public override bool Transform(long state, bool pulse) =>
            (state & Mask) == 0;
    }

    record ConjunctionModule(long Mask, long DestMask, long SourceMask) : Module(Mask, DestMask)
    {
        public override bool Transform(long state, bool pulse) =>
            (state & SourceMask) != SourceMask;
    }

    class Program
    {
        private const string StartKey          = "broadcaster";
        private const char   FlipFlopPrefix    = '%';
        private const char   ConjunctionPrefix = '&';

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
                    ? state | module.Mask
                    : state & ~module.Mask;
                if (pulse && lengths[curr] == 0)
                    lengths[curr] = step;
                int next = 0;
                for (var mask = 1L; next < modules.Length; mask <<= 1, ++next)
                    if ((module.DestMask & mask) != 0)
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
            var keys = tuples.Select(GetKey).ToArray();
            var modules = tuples
                .Select((t, i) => CreateModule(i, t.key, t.dests, tuples, keys))
                .ToArray();
            start = keys.IndexOf(StartKey);
            return modules;
        }

        private static long[] CreateLengths(Module[] modules) =>
            modules
                .Select(m => m is ConjunctionModule ? 0L : 1L)
                .ToArray();

        private static Module CreateModule(int index, string key, string[] dests, List<(string key, string[] dests)> tuples, string[] keys) =>
            CreateModule(1L << index, key, GetDestMask(dests, keys), tuples);

        private static Module CreateModule(long mask, string key, long destMask, List<(string key, string[] dests)> tuples) => key[0] switch
        {
            FlipFlopPrefix    => new FlipFlopModule(mask, destMask),
            ConjunctionPrefix => new ConjunctionModule(mask, destMask, GetSourceMask(GetKey(key), tuples)),
            _ => new RelayModule(mask, destMask),
        };

        private static string GetKey((string key, string[] dests) tuple) =>
            GetKey(tuple.key);

        private static string GetKey(string key) =>
            key.TrimStart(FlipFlopPrefix, ConjunctionPrefix);

        private static long GetDestMask(string[] dests, string[] keys) =>
            dests.Select(keys.IndexOf).Sum(index => 1L << index);

        private static long GetSourceMask(string key, List<(string key, string[] dests)> tuples) =>
            tuples.Select()
                .Where(t => t.Value.dests.Contains(key))
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
