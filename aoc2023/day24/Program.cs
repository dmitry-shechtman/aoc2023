using MathNet.Numerics.LinearAlgebra;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace aoc.aoc2023.day24
{
    static class Program
    {
        static void Main(string[] args)
        {
            var hail = Parse(args[0]);
            var min = args.Length > 1 ? long.Parse(args[1]) : 200000000000000;
            var max = args.Length > 2 ? long.Parse(args[2]) : 400000000000000;
            Console.WriteLine(Part1(hail, (min, max), null));
            Console.WriteLine(Part2(hail, null));
        }

        private static int Part1(DoubleMatrix3D[] hail, DoubleVectorRange range, StringBuilder sb) =>
            hail.Sum((pv1, i) => hail[(i + 1)..].Count(pv2 =>
                CrossPaths(pv1, pv2, range, sb)));

        private static long Part2(DoubleMatrix3D[] hail, StringBuilder sb)
        {
            var rock = GetRock(hail);
            sb?.AppendCollisions(hail, rock);
            return (long)rock.R1.Sum();
        }

        /// <summary>
        /// Solves for t1 and t2:
        ///
        /// v1.x * t1 - v2.x * t2 == p2.x - p1.x
        /// v1.y * t1 - v2.y * t2 == p2.y - p1.y
        /// </summary>
        /// 
        /// <returns>
        /// true if t1 >= 0 and t2 >= 0
        /// and the intersection is within range.
        /// </returns>
        private static bool CrossPaths(DoubleMatrix3D pv1, DoubleMatrix3D pv2, DoubleVectorRange range, StringBuilder sb)
        {
            sb?.Append("Hailstone A: ")
                .AppendHailstone(pv1).AppendLine()
                .Append("Hailstone B: ")
                .AppendHailstone(pv2).AppendLine();
            if (!Intersect(pv1, pv2, out DoubleVector x, out DoubleVector3D p))
            {
                sb?.AppendLine("Hailstones' paths are parallel; they never intersect.")
                    .AppendLine();
                return false;
            }
            var (t1, t2) = x;
            if (t1 < 0 || t2 < 0)
            {
                sb?.Append("Hailstones' paths crossed in the past for ")
                    .AppendLine(t1 < 0 && t2 < 0 ? "both hailstones." : t1 < 0 ? "hailstone A." : "hailstone B.")
                    .AppendLine();
                return false;
            }
            bool result = range.Contains((DoubleVector)p);
            sb?.Append("Hailstones' paths will cross ")
                .Append(result ? "inside" : "outside")
                .AppendLine($" the test area (at x={p.x:.###}, y={p.y:.###}).")
                .AppendLine();
            return result;
        }

        /// <summary>
        /// Solves for x, y, z, dx, dy and dz:
        /// 
        /// (dy2 - dy1) * x + (dx1 - dx2) * y + (y1 - y2) * dx + (x2 - x1) * dy ==  x2 * dy2 - y2 * dx2 - x1 * dy1 + y1 * dx1
        /// (dy3 - dy1) * x + (dx1 - dx3) * y + (y1 - y3) * dx + (x3 - x1) * dy ==  x3 * dy3 - y3 * dx3 - x1 * dy1 + y1 * dx1
        /// (dz2 - dz1) * x + (dx1 - dx2) * z + (z1 - z2) * dx + (x2 - x1) * dz ==  x2 * dz2 - z2 * dx2 - x1 * dz1 + z1 * dx1
        /// (dz3 - dz1) * x + (dx1 - dx3) * z + (z1 - z3) * dx + (x3 - x1) * dz ==  x3 * dz3 - z3 * dx3 - x1 * dz1 + z1 * dx1
        /// (dz1 - dz2) * y + (dy2 - dy1) * z + (z2 - z1) * dy + (y1 - y2) * dz == -y2 * dz2 + z2 * dy2 + y1 * dz1 - z1 * dy1
        /// (dz1 - dz3) * y + (dy3 - dy1) * z + (z3 - z1) * dy + (y1 - y3) * dz == -y3 * dz3 + z3 * dy3 + y1 * dz1 - z1 * dy1
        /// </summary>
        /// 
        /// <returns>
        /// ((x, y, z), (dx, dy, dz)).
        /// </returns>
        private static DoubleMatrix3D GetRock(DoubleMatrix3D[] hail)
        {
            var ((x1, y1, z1), (dx1, dy1, dz1)) = hail[0];
            var ((x2, y2, z2), (dx2, dy2, dz2)) = hail[1];
            var ((x3, y3, z3), (dx3, dy3, dz3)) = hail[2];

            var a = Matrix<double>.Build.Dense(6, 6);
            a.SetRow(0, new[] { dy2 - dy1, dx1 - dx2,         0, y1 - y2, x2 - x1,       0 });
            a.SetRow(1, new[] { dy3 - dy1, dx1 - dx3,         0, y1 - y3, x3 - x1,       0 });
            a.SetRow(2, new[] { dz2 - dz1,         0, dx1 - dx2, z1 - z2,       0, x2 - x1 });
            a.SetRow(3, new[] { dz3 - dz1,         0, dx1 - dx3, z1 - z3,       0, x3 - x1 });
            a.SetRow(4, new[] {         0, dz1 - dz2, dy2 - dy1,       0, z2 - z1, y1 - y2 });
            a.SetRow(5, new[] {         0, dz1 - dz3, dy3 - dy1,       0, z3 - z1, y1 - y3 });

            var b = Vector<double>.Build.Dense(new[]
            {
                 x2 * dy2 - y2 * dx2 - x1 * dy1 + y1 * dx1,
                 x3 * dy3 - y3 * dx3 - x1 * dy1 + y1 * dx1,
                 x2 * dz2 - z2 * dx2 - x1 * dz1 + z1 * dx1,
                 x3 * dz3 - z3 * dx3 - x1 * dz1 + z1 * dx1,
                -y2 * dz2 + z2 * dy2 + y1 * dz1 - z1 * dy1,
                -y3 * dz3 + z3 * dy3 + y1 * dz1 - z1 * dy1
            });

            var values = a.Solve(b).AsArray();

            return (values[..3], values[3..]);
        }

        private static bool Intersect(DoubleMatrix3D pv1, DoubleMatrix3D pv2, out DoubleVector t, out DoubleVector3D p)
        {
            var (p1, v1) = pv1;
            var (p2, v2) = pv2;
            var a = DoubleMatrix.FromColumns((DoubleVector)v1, (DoubleVector)(-v2), (DoubleVector)(p2 - p1));
            p = default;
            if (!a.Solve(out t))
                return false;
            p = (v1.x * t.x + p1.x, v1.y * t.x + p1.y, v1.z * t.x + p1.z);
            return true;
        }

        private static StringBuilder AppendCollisions(this StringBuilder sb, DoubleMatrix3D[] hail, DoubleMatrix3D rock)
        {
            foreach (var pv in hail)
            {
                Intersect(rock, pv, out var t, out var p);
                sb.Append("Hailstone: ")
                    .AppendHailstone(pv).AppendLine()
                    .AppendLine($"Collision time: {t.x}")
                    .Append("Collision position: ")
                    .AppendVector(p)
                    .AppendLine()
                    .AppendLine();
            }
            return sb;
        }

        private static StringBuilder AppendHailstone(this StringBuilder sb, DoubleMatrix3D pv) =>
            sb.AppendVector(pv.R1)
                .Append(" @ ")
                .AppendVector(pv.R2);

        private static StringBuilder AppendVector(this StringBuilder sb, DoubleVector3D p) =>
            sb.AppendFormat("{0:x, y, z}", p);

        private static DoubleMatrix3D[] Parse(string path) =>
            DoubleMatrix3D.ParseRowsAll(File.ReadAllText(path), CultureInfo.InvariantCulture, 2);
    }
}
