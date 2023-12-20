using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day20
{
    sealed class LongState
    {
        private const int CountsShift = 32;

        private long counts;

        public LongState(long flipMask, long conjMask)
        {
            FlipMask = flipMask;
            LcmMask = ConjMask = conjMask;
            Lcm = 1;
        }

        public long FlipMask { get; }
        public long ConjMask { get; }
        public long State { get; set; }

        public long Low
        {
            get => (uint)counts;
            set => counts = counts >> CountsShift << CountsShift | value;
        }

        public long High
        {
            get => counts >> CountsShift;
            set => counts = value << CountsShift | (uint)counts;
        }

        public long Product => Low * High;

        public long Lcm     { get; set; }
        public long LcmMask { get; set; }
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

        private static long Part1((long, long)[] modules, int start, LongState state)
        {
            for (int i = 1; i <= 1000; i++)
                Step(modules, start, state, i);
            return state.Product;
        }

        private static long Part2((long, long)[] modules, int start, LongState state)
        {
            for (int i = 1001; state.LcmMask > 0; i++)
                Step(modules, start, state, i);
            return state.Lcm;
        }

        private static void Step((long, long)[] modules, int start, LongState state, int step)
        {
            Queue<(bool, int, long)> queue = new();
            queue.Enqueue((false, start, 1L << start));
            while (queue.TryDequeue(out var tuple))
            {
                var (pulse, index, mask) = tuple;
                if (!pulse)
                    ++state.Low;
                else
                    ++state.High;
                var (destMask, srcMask) = modules[index];
                if ((mask & state.FlipMask) != 0)
                {
                    if (pulse)
                        continue;
                    pulse = (state.State & mask) == 0;
                }
                else if ((mask & state.ConjMask) != 0)
                {
                    pulse = (state.State & srcMask) != srcMask;
                }
                state.State = pulse
                    ? state.State | mask
                    : state.State & ~mask;
                if ((mask & state.LcmMask) != 0 && pulse)
                {
                    state.Lcm = MathEx.Lcm(state.Lcm, step);
                    state.LcmMask &= ~mask;
                }
                for (mask = 1L, index = 0; mask <= destMask; mask <<= 1, ++index)
                    if ((destMask & mask) != 0)
                        queue.Enqueue((pulse, index, mask));
            }
        }

        private static (long, long)[] GetModules(string path, out int start, out LongState state)
        {
            var tuples = Parse(path);
            tuples.SelectMany(t => t.dests)
                .Except(tuples.Select(GetKey))
                .ToList()
                .ForEach(d => tuples.Add((d, Array.Empty<string>())));
            var keys = tuples.Select(GetKey).ToArray();
            var modules = tuples
                .Select(t => CreateModule(t.key, t.dests, tuples, keys))
                .ToArray();
            start = keys.IndexOf(StartKey);
            var flipMask = GetFlipFlopMask(tuples);
            var conjMask = GetConjunctionMask(tuples);
            state = new(flipMask, conjMask);
            return modules;
        }

        private static (long, long) CreateModule(string key, string[] dests, List<(string key, string[] dests)> tuples, string[] keys) =>
            (GetDestMask(dests, keys), GetSourceMask(GetKey(key), tuples));

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
