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
            sb.AppendLine($"Hailstone A: {p1.x}, {p1.y}, {p1.z} @ {v1.x}, {v1.y}, {v1.z}");
            sb.AppendLine($"Hailstone B: {p2.x}, {p2.y}, {p2.z} @ {v2.x}, {v2.y}, {v2.z}");
            if (!DoubleMatrix.Invert((v1.x, -v2.x, v1.y, -v2.y), out var inv))
            {
                sb.Append("Hailstones' paths are parallel; they never intersect");
                result = false;
            }
            else
            {
                var (t1, t2) = inv * (p2.x - p1.x, p2.y - p1.y);
                if (t1 < 0 || t2 < 0)
                {
                    sb.Append("Hailstones' paths crossed in the past for ");
                    sb.Append(t1 < 0 && t2 < 0 ? "both hailstones" : t1 < 0 ? "hailstone A" : "hailstone B");
                    result = false;
                }
                else
                {
                    var (x, y) = (v1.x * t1 + p1.x, v1.y * t1 + p1.y);
                    result = range.IsMatch((x, y));
                    sb.Append("Hailstones' paths will cross ");
                    sb.Append(result ? "inside" : "outside");
                    sb.Append($" the test area (at x={x:.###}, y={y:.###})");
                }
            }
            sb.AppendLine(".");
            sb.AppendLine();
            return result;
        }

        private static LongParticle3D[] Parse(string path) =>
            File.ReadAllLines(path)
                .Select(s => LongParticle3D.Parse(s, '@'))
                .ToArray();
    }
}
