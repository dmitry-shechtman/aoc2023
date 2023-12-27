using System;
using System.IO;
using System.Linq;
using System.Text;

namespace aoc.aoc2023.day24
{
    class Program
    {
        private const long Min = 200000000000000;
        private const long Max = 400000000000000;

        static void Main(string[] args)
        {
            var hail = Parse(args[0]);
            Console.WriteLine(Part1(hail));
        }

        private static int Part1(LongParticle3D[] hail) =>
            hail.Sum((pv1, i) => hail[(i + 1)..].Count(pv2 =>
                CrossPaths(pv1, pv2, ((Min, Min), (Max, Max)), new())));

        // Solves for t1 and t2:
        //
        // v1.x * t1 - v2.x * t2 == p2.x - p1.x
        // v1.y * t1 - v2.y * t2 == p2.y - p1.y
        //
        // Returns true if t1 >= 0 and t2 >= 0.
        private static bool CrossPaths(LongParticle3D pv1, LongParticle3D pv2, DoubleVectorRange range, StringBuilder sb)
        {
            var (p1, v1) = pv1;
            var (p2, v2) = pv2;
            bool result;
            sb.AppendLine($"Hailstone A: {p1:x, y, z} @ {v1:x, y, z}");
            sb.AppendLine($"Hailstone B: {p2:x, y, z} @ {v2:x, y, z}");
            DoubleMatrix a = (v1.x, -v2.x, v1.y, -v2.y);
            DoubleVector b = (p2.x - p1.x, p2.y - p1.y);
            if (!a.Solve(b, out DoubleVector x))
            {
                sb.Append("Hailstones' paths are parallel; they never intersect");
                result = false;
            }
            else
            {
                var (t1, t2) = x;
                if (t1 < 0 || t2 < 0)
                {
                    sb.Append("Hailstones' paths crossed in the past for ");
                    sb.Append(t1 < 0 && t2 < 0 ? "both hailstones" : t1 < 0 ? "hailstone A" : "hailstone B");
                    result = false;
                }
                else
                {
                    DoubleVector p = (v1.x * t1 + p1.x, v1.y * t1 + p1.y);
                    result = range.IsMatch(p);
                    sb.Append("Hailstones' paths will cross ");
                    sb.Append(result ? "inside" : "outside");
                    sb.Append($" the test area (at x={p.x:.###}, y={p.y:.###})");
                }
            }
            sb.AppendLine(".");
            sb.AppendLine();
            return result;
        }

        private static LongParticle3D[] Parse(string path) =>
            File.ReadAllLines(path)
                .Select(LongParticle3D.Parse)
                .ToArray();
    }
}
