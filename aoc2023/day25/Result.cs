namespace aoc.aoc2023.day25
{
    public readonly struct Result
    {
        public Result(int cutSize, int product)
        {
            CutSize = cutSize;
            Product = product;
        }

        public readonly int CutSize { get; }
        public readonly int Product { get; }
    }
}
