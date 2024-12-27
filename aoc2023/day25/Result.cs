namespace aoc.aoc2023.day25
{
    public readonly struct Result
    {
        public Result(int cutSize, int size1, int size2)
        {
            CutSize = cutSize;
            Size1 = size1;
            Size2 = size2;
        }

        public readonly int CutSize { get; }
        public readonly int Size1 { get; }
        public readonly int Size2 { get; }
    }
}
