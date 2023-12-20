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

    sealed class LongState
    {
        public LongState(long flipMask, long conjMask)
        {
            FlipMask = flipMask;
            LcmMask = conjMask;
            Lcm = 1L;
            Counts = new int[2];
        }

        public long FlipMask { get; }
        public long State    { get; set; }
        public long Lcm      { get; set; }
        public long LcmMask  { get; set; }
        public int[] Counts  { get; }
    }

    class Program
    {
        private const string StartKey          = "broadcaster";
        private const char   FlipFlopPrefix    = '%';
        private const char   ConjunctionPrefix = '&';

        static void Main(string[] args)
        {
            var modules = GetModules(args[0], out int start, out var state);
            Console.WriteLine(Part1(modules, start, state));
            Console.WriteLine(Part2(modules, start, state));
        }

        private static int Part1(Module[] modules, int start, LongState state)
        {
            for (int i = 1; i <= 1000; i++)
                Step(modules, start, state, i);
            return state.Counts.Product();
        }

        private static long Part2(Module[] modules, int start, LongState state)
        {
            for (int i = 1001; state.LcmMask > 0; i++)
                Step(modules, start, state, i);
            return state.Lcm;
        }

        private static void Step(Module[] modules, int start, LongState state, int step)
        {
            Queue<(bool, int)> queue = new();
            queue.Enqueue((false, start));
            while (queue.TryDequeue(out var tuple))
            {
                var (pulse, index) = tuple;
                ++state.Counts[pulse ? 1 : 0];
                var module = modules[index];
                var mask = module.Mask;
                if (pulse && (mask & state.FlipMask) != 0)
                    continue;
                pulse = module.Transform(state.State, pulse);
                state.State = pulse
                    ? state.State | mask
                    : state.State & ~mask;
                if (pulse && (mask & state.LcmMask) != 0)
                {
                    state.Lcm = MathEx.Lcm(state.Lcm, step);
                    state.LcmMask &= ~mask;
                }
                for (mask = 1L, index = 0; index < modules.Length; mask <<= 1, ++index)
                    if ((module.DestMask & mask) != 0)
                        queue.Enqueue((pulse, index));
            }
        }

        private static Module[] GetModules(string path, out int start, out LongState state)
        {
            var tuples = Parse(path);
            tuples.SelectMany(t => t.dests)
                .Except(tuples.Select(GetKey))
                .ToList()
                .ForEach(d => tuples.Add((d, Array.Empty<string>())));
            var keys = tuples
                .Select(GetKey)
                .ToArray();
            var modules = tuples
                .Select((t, i) => CreateModule(i, t.key, t.dests, tuples, keys))
                .ToArray();
            start = keys.IndexOf(StartKey);
            var flipMask = GetFlipFlopMask(tuples);
            var conjMask = GetConjunctionMask(tuples);
            state = new(flipMask, conjMask);
            return modules;
        }

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
            dests.Select(keys.IndexOf)
                .Sum(index => 1L << index);

        private static long GetSourceMask(string key, List<(string key, string[] dests)> tuples) =>
            tuples.Select()
                .Where(t => t.Value.dests.Contains(key))
                .Sum(t => 1L << t.Index);

        private static long GetFlipFlopMask(List<(string key, string[] dests)> tuples) =>
            GetMask(FlipFlopPrefix, tuples);

        private static long GetConjunctionMask(List<(string key, string[] dests)> tuples) =>
            GetMask(ConjunctionPrefix, tuples);

        private static long GetMask(char c, List<(string key, string[] dests)> tuples) =>
            tuples.Select()
                .Where(t => t.Value.key[0] == c)
                .Sum(t => 1L << t.Index);

        private static List<(string key, string[] dests)> Parse(string path) =>
            File.ReadAllLines(path)
                .Select(s => s.Split(" -> "))
                .Select(ParseOne)
                .ToList();

        private static (string key, string[] dests) ParseOne(string[] tt) =>
            (tt[0], tt[1].Split(',', StringSplitOptions.TrimEntries));
    }
}
