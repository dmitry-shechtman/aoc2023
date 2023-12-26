using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace aoc.aoc2023.day23
{
    class Program
    {
        private const int Empty         = 0, Forest        = 3;
        private const int StartHeading  = 0, StartDistance = 2;

        private static readonly Vector PreStart = (1, 0), DeltaPostEnd = (2, 1);
        private static readonly Vector Start    = (1, 1), DeltaEnd     = (2, 2);

        static void Main(string[] args)
        {
            var input = Parse(args[0], out var size);
            Console.WriteLine(Part1(input, size));
            Console.WriteLine(Part2(input, size));
        }

        private static int Part1(int[] input, Size size) =>
            Solve(input, size);

        private static int Part2(int[] input, Size size) =>
            Solve(input.Select(v => v == Forest ? Forest : Empty).ToArray(), size);

        private static int Solve(int[] input, Size size)
        {
            var idxStart = size.GetIndex(Start);
            var idxEnd = size.GetIndex((Vector)size - DeltaEnd);
            var output = new int[size.Length];
            var visited = CreateVisited(idxStart, size);
            var deltas = CreateDeltas(size);
            FindLongestPath(idxStart, StartHeading, StartDistance, input, output, visited, deltas);
            return output[idxEnd];
        }

        public static void FindLongestPath(int index, int heading, int dist, int[] input, int[] output, BitArray visited, int[][] deltas)
        {
            visited[index] = true;
            if (output[index] < dist)
                output[index] = dist;
            foreach (var delta in deltas[heading])
            {
                var index2 = index + delta;
                var heading2 = input[index2];
                if (heading2 != Forest && !visited[index2])
                    FindLongestPath(index2, heading2, dist + 1, input, output, visited, deltas);
            }
            visited[index] = false;
        }

        private static BitArray CreateVisited(int idxStart, Size size)
        {
            BitArray visited = new(size.Length);
            visited[idxStart] = true;
            return visited;
        }

        private static int[][] CreateDeltas(Size size) => new[]
        {
            new[] { -size.x, 1, size.x, -1 },
            new[] { 1 },
            new[] { size.x },
            Array.Empty<int>()
        };

        private static int[] Parse(string path, out Size size)
        {
            var s = File.ReadAllText(path);
            size = Size.FromField(s);
            var input = s.Replace("\n", "")
                .Select(c => ".>v#".IndexOf(c))
                .ToArray();
            size.SetValue(input, PreStart, Forest);
            size.SetValue(input, (Vector)size - DeltaPostEnd, Forest);
            return input;
        }
    }
}
