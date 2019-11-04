using System;
using System.IO;
using static System.Console;

namespace Tanaka
{
    class Program
    {
        public static string ConvertTanakaCorpusLine(string line)
        {
            var tab = line.IndexOf('\t');
            var id = line.IndexOf("#ID=", StringComparison.Ordinal);
            var japanese = line.Substring(3, tab - 3).Replace("\"", "\"\"");
            ;
            var english = line.Substring(tab + 1, id - tab - 1).Replace("\"", "\"\"");
            var identifier = line.Substring(id + 4);
            return $"{identifier},\"{japanese}\",\"{english}\"";
        }

        static void Main(string[] args)
        {
            WriteLine("ID,Japanese,English");
            var ainfo = "";
            var outfile = $"{args[0]}.csv";
            File.WriteAllText(outfile, "ID,Japanese,English,DictionaryInfo\r");
            foreach (var line in File.ReadAllLines(args[0]))
            {
                if (line.StartsWith("A:"))
                {
                    ainfo = ConvertTanakaCorpusLine(line);
                } else if (line.StartsWith("B:"))
                {
                    var binfo = line.Substring(3).Replace("\"", "\"\"");
                    File.AppendAllText(outfile, $"{ainfo},\"{binfo}\"\r");
                }
            }
        }
    }
}
