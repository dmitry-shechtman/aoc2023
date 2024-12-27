using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace aoc.aoc2023.day25
{
    public class Karger
    {
        private readonly int _vertices;
        private readonly Edge[] _edges;

        public Karger(int count, Edge[] edges)
        {
            _vertices = count;
            _edges = edges;
        }

        public Result FindMinCut(int m)
        {
            // Calculate optimal number of iterations and batch size
            int totalIterations = (int)(Math.Log(_vertices) * _vertices / 2);
            int batchSize = Math.Max(1000, totalIterations / (Environment.ProcessorCount * 4));
            int numBatches = (totalIterations + batchSize - 1) / batchSize;

            // Create concurrent bag to store results from all batches
            Result final = default;

            // Process batches in parallel
            Parallel.For(0, numBatches, batchIndex =>
            {
                var batch = Math.Min(batchSize, totalIterations - batchIndex * batchSize);

                Span<int> parent = stackalloc int[_vertices];
                Span<int> rank = stackalloc int[_vertices];
                Span<int> size = stackalloc int[_vertices];

                for (int i = 0; i < batch; i++)
                {
                    var result = RunKarger(parent, rank, size, new());
                    if (result.CutSize > m)
                        continue;
                    lock (this)
                        final = result;
                    return;
                }
            });

            return final;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Result RunKarger(Span<int> parent, Span<int> rank, Span<int> size, Random random)
        {
            // Initialize arrays
            for (int i = 0; i < parent.Length; i++)
                parent[i] = i;

            rank.Fill(0);
            size.Fill(1);

            // Contract until only 2 vertices remain
            Edge edge;
            for (int i = 2, x, y, index, value; i < _vertices; i++)
            {
                // Pick a random valid edge
                do
                    edge = _edges[random.Next(_edges.Length)];
                while ((x = Find(edge.Source, parent)) == (y = Find(edge.Target, parent)));

                // Merge vertices
                (index, value) = rank[x] < rank[y] ? (x, y) : (y, x);
                parent[index] = value;
                size[value] = size[x] + size[y];
                rank[x] += rank[x] == rank[y] ? 1 : 0;
            }

            // Count final cut size
            int cut = 0;
            for (int i = 0; i < _edges.Length; i++)
                cut +=
                    Find(_edges[i].Source, parent) ==
                    Find(_edges[i].Target, parent) ? 0 : 1;

            // Find set sizes
            int size1 = 0, root = -1, root1 = -1;
            for (int i = 0; i < _vertices && root == root1; i++)
            {
                root = Find(i, parent);
                (root1, size1) = root1 < 0
                    ? (root, size[root])
                    : (root1, size1);
            }

            return new(cut, size1, size[root]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Find(int vertex, Span<int> parent)
        {
            while (vertex != parent[vertex])
                vertex = parent[vertex] = parent[parent[vertex]];
            return vertex;
        }
    }
}
