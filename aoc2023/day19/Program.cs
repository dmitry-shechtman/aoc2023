using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace aoc.aoc2023.day19
{
    using Part      = Vector4D;
    using PartRange = Vector4DRange;
    using Workflows = Dictionary<string, Rule[]>;

    record Rule(string Dest, PartRange Range);

    class Program
    {
        private const int Cardinality = 4, MinValue  =   1, MaxValue  = 4000;
        private const string InKey = "in", RejectKey = "R", AcceptKey =  "A";

        private static readonly PartRange DefaultRange = CreateRange(-1, (MinValue, MaxValue));

        private static readonly Regex WorkflowRegex = new(
            @"^(?<key>[a-z]+)\{((?<coord>[xmas])(?<op>[<>])(?<value>\d+):(?<dest>R|A|[a-z]+),)+(?<dest>R|A|[a-z]+)\}$");

        static void Main(string[] args)
        {
            Parse(args[0], out var workflows, out var parts);
            Console.WriteLine(Part1(workflows, parts));
        }

        private static int Part1(Workflows workflows, Part[] parts) =>
            parts.Sum(p => Process(InKey, p, workflows));

        private static int Process(string key, Part part, Workflows workflows) =>
            Process(workflows[key].First(r => r.Range.Contains(part)), part, workflows);

        private static int Process(Rule rule, Part part, Workflows workflows) => rule.Dest switch
        {
            RejectKey => 0,
            AcceptKey => part.Abs(),
            _ => Process(rule.Dest, part, workflows),
        };

        private static void Parse(string path, out Workflows workflows, out Part[] parts)
        {
            var ss = File.ReadAllText(path).Trim().Split("\n\n");
            workflows = ParseWorkflows(ss[0]);
            parts = ParseParts(ss[1]);
        }

        private static Workflows ParseWorkflows(string s) =>
            s.Split('\n').Select(ParseWorkflow).ToDictionary(t => t.key, t => t.rules);

        private static (string key, Rule[] rules) ParseWorkflow(string s)
        {
            var values = WorkflowRegex.GetAllValues(s);
            var key = values["key"][0];
            return (key, ParseRules(values));
        }

        private static Rule[] ParseRules(Dictionary<string, string[]> values) =>
            values["dest"].Select((d, i) => ParseRule(d, values, i)).ToArray();

        private static Rule ParseRule(string dest, Dictionary<string, string[]> values, int i) =>
            new(dest, GetRange(values, i));

        private static PartRange GetRange(Dictionary<string, string[]> values, int i) =>
            i < values["op"].Length ? ParseRange(values, i) : DefaultRange;

        private static PartRange ParseRange(Dictionary<string, string[]> values, int i)
        {
            var coord = "xmas".IndexOf(values["coord"][i]);
            var op = values["op"][i];
            var value = int.Parse(values["value"][i]);
            var range = op == ">"
                ? (value + 1, MaxValue)
                : (MinValue, value - 1);
            return CreateRange(coord, range);
        }

        private static PartRange CreateRange(int coord, Range range) =>
            (CreatePart(coord, range.Min, MinValue), CreatePart(coord, range.Max, MaxValue));

        private static Part CreatePart(int coord, int value, int defaultValue) =>
            Enumerable.Range(0, Cardinality).Select(i => i == coord ? value : defaultValue).ToArray();

        private static Part[] ParseParts(string s) =>
            Part.Builder.ParseAll(s, CultureInfo.InvariantCulture);
    }
}
