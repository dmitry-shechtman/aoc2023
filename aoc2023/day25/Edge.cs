namespace aoc.aoc2023.day25
{
    public readonly struct Edge
    {
        public Edge(int source, int target)
        {
            Source = source;
            Target = target;
        }

        public readonly int Source { get; }
        public readonly int Target { get; }
    }
}
