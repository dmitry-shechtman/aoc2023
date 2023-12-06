# Walkthrough

**Ho-ho-ho!**

Sit down, little Eli, and listen to a fun story about growing food.
No, not in my lap, people nowadays get the strangest ideas about such things!

So our [problem](https://adventofcode.com/2023/day/5) could be broken down into four parts:

0. [Defining the types](#Types),
1. [Parsing the almanac](#Parsing),
2. [Solving for a collection of numbers](#PartOne), and
3. [Solving for a collection of ranges](#PartTwo)


## Types


The almanac has two types of records:

1. **seeds**, which may be represented as an array of 64-bit integers, i.e., `long[]`, and
2. **maps**.


Each map record consists of an array of entries:

```C#
    record Map(Entry[] Entries)
    {
    }
```

A map entry, in turn, consists of three numbers:

1. destination range start,
2. source range start, and
3. range length.


Notice anything common between those three? That's right! It's *range*!
So we'll define an entry as a pair of ranges:

```C#
    record Entry(LongRange Source, LongRange Dest)
```


A range could be represented as *start* and *length*,
but *start* and *end* are more convenient.
You'll see why very soon.
Let's call those `Min` and `Max` - we *are* looking for a minimum, after all!

```C#
    struct LongRange
    {
        public long Min { get; }
        public long Max { get; }
    }
```

We need to define a constructor:

```C#
        public LongRange(long min, long max)
        {
            Min = min;
            Max = max;
        }
```

and a method to initialize an instance from *start* and *length*:

```C#
        public static LongRange FromMinLength(params long[] values) =>
            new(values[0], values[0] + values[1] - 1);
```

See that `params` prefix? It allows us to pass `start` and `length` as two parameters
(in addition to an array of `long`s as a single parameter).

Let's leave those types for now, little Eli, and talk about

## Parsing

First, we need to read the almanac into a `string`.
We do that by calling `File.ReadAllText()` with our first argument (`args[0]`) as the parameter.

Next, we need to split our string into *seeds* and *maps*.
Now, think of the string as a very long tape with special markings for line feeds (LF).
We find all the places where two LFs appear together and split the tape along these:

```C#
    var ss = File.ReadAllText(args[0]).Split("\n\n");
```

`ss` now holds eight pieces of tape:

  1. seeds
  2. seed-to-soil map
  3. soil-to-fertilizer map
  4. fertilizer-to-water map
  5. water-to-light map
  6. light-to-temperature map
  7. temperature-to-humidity map
  8. humidity-to-location map

See how all those pieces have numbers separated by space characters?
Let's split those, parse each one into a `long`, and make an array:

```C#
    private static long[] ParseInt64s(string s) =>
        s.Split(' ').Select(long.Parse).ToArray();
```

We parse the *seeds* by splitting the first piece of tape at `:`,
throwing the first part away, and passing the second part to `ParseInt64s()`:

```C#
ParseInt64s(ss[0].Split(": ")[1]
```

In order to parse the *maps*, we feed each of the remaining pieces to the map parser and make an array of the results:

```C#
ss[1..].Select(Map.Parse).ToArray()
```

How does the map parser work, you ask?
Well, remember those line feed markers?
We use those to split each of the map pieces (after throwing away any LFs from the edges).
We end up with many smaller pieces.
Let's throw away the first one (it just contains the name of the map, nothing important!).
Now, let's feed each of the remaining small pieces to the entry parser and make an array:

```C#
    record Map //...
    {
        public static Map Parse(string s) =>
            new(s.Trim().Split('\n')[1..].Select(Entry.Parse).ToArray());
        //...
    }
```

As for the entry parser, it's very simple:

```C#
    record Entry //...
    {
        public static Entry Parse(string s) =>
            new(ParseInt64s(s));
        //...
    }
```

An overloaded constructor
* takes the resulting array,
* creates the *source range* from the second and the third element,
* creates the *destination range* from the first and the third element, and
* passes those to the auto-generated constructor:

```C#
    Entry(long[] v) : this(
        Source: LongRange.FromMinLength(v[1], v[2]),
        Dest: LongRange.FromMinLength(v[0], v[2]))
    {
    }
```

Our top-level `Parse` method returns both *seeds* and *maps* as a tuple:

```C#
    (long[], Map[]) Parse(string[] ss) =>
        (ParseInt64s(ss[0].Split(": ")[1]), ss[1..].Select(Map.Parse).ToArray());
```

We're done with parsing, Little Eli!
Moving on to solving

## Part One


We're looking for the minimum location number for all the seeds:

```C#
    long Part1(long[] seeds, Map[] maps) =>
        seeds.Min(seed => Min(seed, maps));
```

In order to find the minimum for each seed, we aggregate the minima across all the maps:

```C#
    long Min(long seed, Map[] maps) =>
        maps.Aggregate(seed, Min);
```

See how `seed` is passed as `Aggregate()`'s `seed`?
I bet Eric had this exact thing in mind when he created the puzzle!

Now, in order to find the minimum given a *value* and a *map*, we first attempt to transform the value.
If we succeed, we return the smallest among the results.
Otherwise, we simply return the current value.

```C#
    long Min(long value, Map map) =>
        map.Transform(value).Any() ? map.Transform(value).Min() : value;
```

A *map* transforms a *value* by passing it to all entries with matching source ranges:

```C#
    record Map //...
    {
        //...
        public IEnumerable<long> Transform(long value) =>
            Entries.Where(m => m.Source.Match(value)).Select(m => m.Transform(value));
        //...
    }
```

A *range* matches a *value* by checking whether it, well, falls within the range:

```C#
    struct LongRange
    {
        //...
        public bool Match(long value) =>
            value >= Min && value <= Max;
        //...
    }
```

Finally, an *entry* transforms a *value* by
adding to it the destination range start minus
the source range start:

```C#
    record Entry //...
    {
        //...
        public long Transform(long value) =>
            value + Dest.Min - Source.Min;
        //...
    }
```

Simple, isn't, little Eli?
Now, to slightly more compicated stuff...

## Part Two


We now have *ranges* rather than *values*.

In order to group the numbers into pairs, we use `Chunk(2)`.
It's a really simple extension method, so we won't be explaining it.
We get a collection of arrays, which we pass to our good friend `FromMinLength()`.
We then pass the resulting seed ranges as the `seed` *(see? again!)* to an `Aggregate()` of transforms across all the maps.
Finally, we find the minimum across the final ranges' minima:

```C#
    long Part2(long[] seeds, Map[] maps) =>
        maps.Aggregate(
                seeds.Chunk(2).Select(LongRange.FromMinLength), Transform)
            .Min(range => range.Min);
```

In order to transform a *collection* of ranges via a *map*, we collect the results across all the ranges:

```C#
    IEnumerable<LongRange> Transform(IEnumerable<LongRange> ranges, Map map) =>
        ranges.SelectMany(range => Transform(range, map));
```

Transforming a single *range* via a *map* is similar to what we'd done in Part One:

```C#
    IEnumerable<LongRange> Transform(LongRange range, Map map) =>
        map.Transform(range).Any() ? map.Transform(range) : new[] { range };
```

A *map* transforms a *range* by passing it to all entries with matching source ranges:

```C#
    record Map //...
    {
        //...
        public IEnumerable<LongRange> Transform(LongRange range) =>
            Entries.Where(m => m.Source.Match(range)).Select(m => m.Transform(range));
    }
```

A *range* matches another *range* by checking whether there is an overlap between the two:

```C#
    struct LongRange
    {
        //...
        public bool Match(LongRange other) =>
            other.Min <= Max && other.Max >= Min;
        //...
    }
```

An *entry* transforms a *range* by
intersecting the destination range with the range from
the transformed minimum to the transformed maximum:

```C#
    record Entry //...
    {
        //...
        public LongRange Transform(LongRange range) =>
            Dest.Intersect((Transform(range.Min), Transform(range.Max)));
    }
```

An intersection of two ranges is defined as the range from
the maximum of the two minima to the minimum of the two maxima:

```C#
    struct LongRange
    {
        //...
        public LongRange Intersect(LongRange other) =>
            new(Math.Max(Min, other.Min), Math.Min(Max, other.Max));
        //...
    }
```

See how everything falls into place?
Little Eli?
Hello?!

**Chanukah Sameach!!!**