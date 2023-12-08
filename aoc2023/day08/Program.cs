using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ціле   = System.Int32;
using довге  = System.Int64;
using рядок  = System.String;
using літера = System.Char;

namespace aoc.aoc2023.day08
{
    using Мапа  = Dictionary<рядок, рядок[]>;
    using Кроки = HashSet<(ціле, ціле)>;

    static class Program
    {
        static void Main(рядок[] арг)
        {
            Розбір(арг[0], out ціле[] напр, out Мапа мапа);
            Console.WriteLine(Частина1(напр, мапа));
            Console.WriteLine(Частина2(напр, мапа));
        }

        private static ціле Частина1(ціле[] напр, Мапа мапа)
        {
            ціле крок = 0;
            for (рядок вузол = "AAA"; вузол != "ZZZ"; ++крок)
                вузол = мапа[вузол][напр[крок % напр.Довжина()]];
            return крок;
        }

        private static довге Частина2(ціле[] напр, Мапа мапа)
        {
            рядок[] вузли = мапа.Ключі().НаМасив();
            ціле[][] знач = мапа.Значення()
                .Вибір(знач => знач.Вибір(вузол => вузли.Індекс(вузол)).НаМасив())
                .НаМасив();
            return вузли
                .Вибір((вузол, ліч) => вузол[^1] == 'A' ? ПошукЦиклу(ліч, напр, знач) : 1)
                .Агрегат(Нск);
        }

        private static довге ПошукЦиклу(ціле початок, ціле[] напр, ціле[][] мапа)
        {
            Кроки кроки = new();
            (ціле вузол, ціле крок) потч = (початок, 0);
            while (кроки.Add(потч))
                потч = (мапа[потч.вузол][напр[потч.крок]], (потч.крок + 1) % напр.Довжина());
            return кроки.Кількість() - потч.крок;
        }

        private static довге Нск(довге а, довге б) =>
            а / Нсд(а, б) * б;

        private static довге Нсд(довге а, довге б) =>
            б == 0 ? а : Нсд(б, а % б);

        private static void Розбір(рядок шлях, out ціле[] напр, out Мапа мапа)
        {
            рядок[] рядки = File.ReadAllText(шлях).Розділ("\n\n");
            напр = рядки[0].Вибір(літ => "LR".Індекс(літ)).НаМасив();
            мапа = рядки[1].Trim().Розділ("\n")
                .Вибір(ряд => ряд.Розділ(" = "))
                .НаСловник(рядки => рядки[0], рядки => рядки[1][1..^1].Розділ(", "));
        }

        private static ТДжерело Агрегат<ТДжерело>
            (this IEnumerable<ТДжерело> джерело, Func<ТДжерело, ТДжерело, ТДжерело> вибір) =>
                джерело.Aggregate(вибір);

        private static IEnumerable<ТВиплід> Вибір<ТДжерело, ТВиплід>
            (this IEnumerable<ТДжерело> джерело, Func<ТДжерело, ТВиплід> вибір) =>
                джерело.Select(вибір);

        private static IEnumerable<ТВиплід> Вибір<ТДжерело, ТВиплід>
            (this IEnumerable<ТДжерело> джерело, Func<ТДжерело, ціле, ТВиплід> вибір) =>
                джерело.Select(вибір);

        private static ТДжерело[] НаМасив<ТДжерело>
            (this IEnumerable<ТДжерело> джерело) =>
                джерело.ToArray();

        private static Dictionary<ТКлюч, ТЗнач> НаСловник<ТДжерело, ТКлюч, ТЗнач>
            (this IEnumerable<ТДжерело> джерело, Func<ТДжерело, ТКлюч> ключ, Func<ТДжерело, ТЗнач> знач) =>
                джерело.ToDictionary(ключ, знач);

        private static ціле Індекс<Т>
            (this Т[] масив, Т знач) =>
                Array.IndexOf(масив, знач);

        private static ціле Довжина
            (this Array масив) =>
                масив.Length;

        private static ціле Кількість<ТДжерело>
            (this ICollection<ТДжерело> джерело) =>
                джерело.Count;

        private static Dictionary<ТКлюч, ТЗнач>.KeyCollection Ключі<ТКлюч, ТЗнач>
            (this Dictionary<ТКлюч, ТЗнач> словник) =>
                словник.Keys;

        private static Dictionary<ТКлюч, ТЗнач>.ValueCollection Значення<ТКлюч, ТЗнач>
            (this Dictionary<ТКлюч, ТЗнач> словник) =>
                словник.Values;

        private static ціле Індекс
            (this рядок ряд, літера літ) =>
                ряд.IndexOf(літ);

        private static рядок[] Розділ
            (this рядок ряд, рядок розд) =>
                ряд.Split(розд);
    }
}
