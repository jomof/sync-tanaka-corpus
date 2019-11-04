using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.Console;

namespace Tanaka
{
    static class Extensions
    {
        internal static string Quote(this string str) => str.Replace("\"", "\"\"");
    }

    class Program
    {
        public static Sentence ConvertTanakaCorpusLine(string line)
        {
            var tab = line.IndexOf('\t');
            var id = line.IndexOf("#ID=", StringComparison.Ordinal);
            var japanese = line.Substring(3, tab - 3);
            ;
            var english = line.Substring(tab + 1, id - tab - 1);
            var identifier = line.Substring(id + 4);
            var result = new Sentence {Id = identifier, Japanese = japanese, English = english};
            return result;
        }
        
        static void Main(string[] args)
        {
            Sentence sentence = null;
            var input = args[0];
            var csv = $"{args[0]}.csv";
            var yaml = $"{args[0]}.yaml";
            WriteLine($"Parsing {input} and writing {csv}");
            File.WriteAllText(csv, "ID,Japanese,English,DictionaryInfo\r");
            var sentences = new List<Sentence>();
            foreach (var line in File.ReadAllLines(args[0]))
            {
                if (line.StartsWith("A:"))
                {
                    sentence = ConvertTanakaCorpusLine(line);
                    sentences.Add(sentence);
                } else if (line.StartsWith("B:"))
                {
                    var dictionaryInfo = line.Substring(3);
                    Debug.Assert(sentence != null, nameof(sentence) + " != null");
                    File.AppendAllText(csv,
                        $"{sentence.Id.Quote()}," +
                        $"{sentence.Japanese.Quote()}," +
                        $"{sentence.English.Quote()}," +
                        $"{dictionaryInfo.Quote()}\r");

                }
            }

            WriteLine($"Sorting");
            sentences = sentences
                .OrderBy(it => it.Japanese.Length)
                .ThenBy(it => it.English.Length)
                .ThenBy(it => it.Id)
                .ToList();
            File.WriteAllText(yaml, "");
            WriteLine($"Writing {yaml}");
            foreach (var sent in sentences)
            {
                File.AppendAllText(yaml, $"- id: {sent.Id}\r");
                File.AppendAllText(yaml, $"  japanese: {sent.Japanese}\r");
                File.AppendAllText(yaml, $"  english: {sent.English}\r");
            }
        }
    }
}
