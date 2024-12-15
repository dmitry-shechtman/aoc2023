using aoc.Grids;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace aoc.aoc2023.day16
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = args[0];
            var dd = Parse(path, out var r);
            Console.WriteLine(Part1(dd));
            Console.WriteLine(Part2(dd, r));
        }

        private static int Part1(uint[] dd) =>
            Energize(1, dd, new int[dd.Length >> 2]);

        private static int Part2(uint[] dd, VectorRange r)
        {
            var data = new[]
            {
                (r.Max.y * r.Width, r.Length, 1      ),
                (0,                 r.Length, r.Width),
                (0,                 r.Width,  1      ),
                (r.Max.x,           r.Length, r.Width)
            };
            var tasks = new Task<int>[4];
            for (int i = 0; i < 4; i++)
            {
                var (start, end, step) = data[i];
                tasks[i] = Task.Run(() => Max(start << 2 | i, end << 2, step << 2, dd));
            }
            return Task.WhenAll(tasks).Result.Max();
        }

        private static int Max(int start, int end, int step, uint[] dd)
        {
            var nn = new int[(end - start + 1) / step + 1];
            var a = new int[dd.Length >> 2];
            for (int i = 0, p = start; p < end; p += step, ++i)
            {
                a.Clear();
                nn[i] = Energize(p, dd, a);
            }
            return nn.Max();
        }

        private static int Energize(int p, uint[] dd, int[] a)
        {
            int i = p >> 2, v = a[i], n = v == 0 ? 1 : 0;
            uint q;
            if (v != (v |= 1 << (p & 3)))
                for (a[i] = v, q = dd[p]; q > 0; q >>= 16)
                    n += Energize((ushort)q - 1, dd, a);
            return n;
        }

        private static uint[] Parse(string path, out VectorRange r)
        {
            var s = File.ReadAllText(path);
            r = VectorRange.FromField(s);
            var dd = new uint[r.Length << 2];
            Vector p = Vector.Zero;
            for (int i = 0, j = 0, h; i < s.Length; ++i, p += (1, 0))
                if ((h = "|-./\n\\".IndexOf(s[i])) == 4)
                    p = (-1, p.y + 1);
                else
                    for (int k = 0; k < 4; ++k)
                        dd[j++] = (k & 1) != (h ^ 1)
                            ? GetDirection(p, r, k ^ (h > 1 ? h - 2 : 0))
                            : GetDirection(p, r, h + 2, h);
            return dd;
        }

        private static uint GetDirection(Vector p, VectorRange r, params int[] hh)
        {
            uint v = 0;
            foreach (var h in hh)
            {
                Vector q = p + Grid.Headings[h];
                v = r.Contains(q) ? v << 16 | (uint)((q.y * r.Width + q.x) << 2 | h) + 1 : v;
            }
            return v;
        }
    }
}
