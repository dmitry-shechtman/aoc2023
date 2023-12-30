using MathNet.Numerics.LinearAlgebra;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace aoc.aoc2023.day24
{
    static class Program
    {
        private const long Min = 200000000000000;
        private const long Max = 400000000000000;

        static void Main(string[] args)
        {
            var hail = Parse(args[0]);
            StringBuilder sb = new();
            Console.WriteLine(Part1(hail, sb));
            Console.WriteLine(Part2(hail, sb));
        }

        private static int Part1(LongMatrix3D[] hail, StringBuilder sb) =>
            hail.Sum((pv1, i) => hail[(i + 1)..].Count(pv2 =>
                CrossPaths(pv1, pv2, new(Min, Max), sb)));

        private static long Part2(LongMatrix3D[] hail, StringBuilder sb)
        {
            var (p1, v1) = hail[0];
            var (p2, v2) = hail[1];
            var (p3, v3) = hail[2];

            // Column-major order
            var a = Matrix<double>.Build.Dense(6, 6, new double[]
            {
                v2.y - v1.y, v3.y - v1.y, v2.z - v1.z, v3.z - v1.z,           0,           0,
                v1.x - v2.x, v1.x - v3.x,           0,           0, v1.z - v2.z, v1.z - v3.z,
                          0,           0, v1.x - v2.x, v1.x - v3.x, v2.y - v1.y, v3.y - v1.y,
                p1.y - p2.y, p1.y - p3.y, p1.z - p2.z, p1.z - p3.z,           0,           0,
                p2.x - p1.x, p3.x - p1.x,           0,           0, p2.z - p1.z, p3.z - p1.z,
                          0,           0, p2.x - p1.x, p3.x - p1.x, p1.y - p2.y, p1.y - p3.y
            });

            var b = Vector<double>.Build.Dense(new double[]
            {
                 p2.x * v2.y - p2.y * v2.x - p1.x * v1.y + p1.y * v1.x,
                 p3.x * v3.y - p3.y * v3.x - p1.x * v1.y + p1.y * v1.x,
                 p2.x * v2.z - p2.z * v2.x - p1.x * v1.z + p1.z * v1.x,
                 p3.x * v3.z - p3.z * v3.x - p1.x * v1.z + p1.z * v1.x,
                -p2.y * v2.z + p2.z * v2.y + p1.y * v1.z - p1.z * v1.y,
                -p3.y * v3.z + p3.z * v3.y + p1.y * v1.z - p1.z * v1.y
            });

            var x = a.Solve(b);
            var values = x.Select(v => (long)Math.Round(v)).ToArray();
            var pv0 = (values[..3], values[3..]);

            foreach (var pv in hail)
            {
                Intersect(pv0, pv, out var t, out var p);
                sb.Append("Hailstone: ").AppendHailstone(pv).AppendLine();
                sb.AppendLine($"Collision time: {t.x}");
                sb.Append("Collision position: ");
                sb.AppendVector((LongVector3D)p);
                sb.AppendLine();
                sb.AppendLine();
            }

            return values[..3].Sum();
        }

        // Solves for t1 and t2:
        //
        // v1.x * t1 - v2.x * t2 == p2.x - p1.x
        // v1.y * t1 - v2.y * t2 == p2.y - p1.y
        //
        // Returns true if t1 >= 0 and t2 >= 0
        // and the intersection is within range.
        private static bool CrossPaths(LongMatrix3D pv1, LongMatrix3D pv2, DoubleVectorRange range, StringBuilder sb)
        {
            sb.Append("Hailstone A: ").AppendHailstone(pv1).AppendLine();
            sb.Append("Hailstone B: ").AppendHailstone(pv2).AppendLine();
            if (!Intersect(pv1, pv2, out DoubleVector x, out DoubleVector3D p))
            {
                sb.AppendLine("Hailstones' paths are parallel; they never intersect.");
                sb.AppendLine();
                return false;
            }
            var (t1, t2) = x;
            if (t1 < 0 || t2 < 0)
            {
                sb.Append("Hailstones' paths crossed in the past for ");
                sb.AppendLine(t1 < 0 && t2 < 0 ? "both hailstones." : t1 < 0 ? "hailstone A." : "hailstone B.");
                sb.AppendLine();
                return false;
            }
            bool result = range.Contains((DoubleVector)p);
            sb.Append("Hailstones' paths will cross ");
            sb.Append(result ? "inside" : "outside");
            sb.AppendLine($" the test area (at x={p.x:.###}, y={p.y:.###}).");
            sb.AppendLine();
            return result;
        }

        private static bool Intersect(LongMatrix3D pv1, LongMatrix3D pv2, out DoubleVector t, out DoubleVector3D p)
        {
            var (p1, v1) = pv1;
            var (p2, v2) = pv2;
            DoubleMatrix a = (v1.x, -v2.x, v1.y, -v2.y);
            DoubleVector b = (p2.x - p1.x, p2.y - p1.y);
            p = default;
            if (!a.Solve(b, out t))
                return false;
            p = (v1.x * t.x + p1.x, v1.y * t.x + p1.y, v1.z * t.x + p1.z);
            return true;
        }

        private static StringBuilder AppendHailstone(this StringBuilder sb, LongMatrix3D pv)
        {
            sb.AppendVector(pv.R1);
            sb.Append(" @ ");
            sb.AppendVector(pv.R2);
            return sb;
        }

        private static StringBuilder AppendVector(this StringBuilder sb, LongVector3D p) =>
            sb.AppendFormat("{0:x, y, z}", p);

        private static LongMatrix3D[] Parse(string path) =>
            File.ReadAllLines(path)
                .Select(s => LongMatrix3D.Parse(s, '@'))
                .ToArray();
    }
}
