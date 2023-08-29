using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroPolo
{
    public static class Search
    {
        private static int ComputeLevenshteinDistance(string s, string t)
        {
            int length = s.Length;
            int length2 = t.Length;
            int[,] array = new int[length + 1, length2 + 1];
            for (int i = 0; i <= length; i++)
            {
                array[i, 0] = i;
            }
            for (int j = 0; j <= length2; j++)
            {
                array[0, j] = j;
            }
            for (int j = 1; j <= length2; j++)
            {
                for (int i = 1; i <= length; i++)
                {
                    int min = ((s[i - 1] != t[j - 1]) ? 1 : 0);
                    array[i, j] = Math.Min(Math.Min(array[i - 1, j] + 1, array[i, j - 1] + 1), array[i - 1, j - 1] + min);
                }
            }
            return array[length, length2];
        }

        private static IEnumerable<KeyValuePair<string, string>> OrderByLevenshteinDistance(Dictionary<string, string> macros, string search)
        {
            return macros.OrderBy(pair => ComputeLevenshteinDistance(pair.Value, search));
        }

        public static void Run(Dictionary<string, string> macros) => Run(macros as IEnumerable<KeyValuePair<string, string>>);
        public static void Run(Dictionary<string, string> macros, string search) => Run(OrderByLevenshteinDistance(macros, search));

        public static void Run(IEnumerable<KeyValuePair<string, string>> macros)
        {
            var list = macros.Select(pair => "  \u2022 " + pair.Key + " \u2192 " + pair.Value);
            int currentPage = 0, total = macros.Count();
            var macrosPerPage = Macro.Settings.macrosPerPage;
            while (currentPage * macrosPerPage < total)
            {
                Console.Clear();
                var currentPageResults = list.Skip(currentPage * macrosPerPage).Take(macrosPerPage);
                int count = 0;
                foreach (var searchResult in currentPageResults)
                {
                    Console.WriteLine(searchResult);
                    count++;
                }
                Console.WriteLine((currentPage * macrosPerPage + count) + " / " + total + " result(s)." +
                    "\nUse the arrow keys to display the next " + macrosPerPage + " results or Enter to stop.");
                while (true)
                {
                    var keyInfo = Console.ReadKey();
                    if (keyInfo.Key == ConsoleKey.Enter)
                        return;
                    else if (keyInfo.Key == ConsoleKey.RightArrow)
                    {
                        currentPage++;
                        break;
                    }
                    else if (keyInfo.Key == ConsoleKey.LeftArrow)
                    {
                        currentPage--;
                        if (currentPage < 0) currentPage = 0;
                        break;
                    }
                }
            }
            Console.WriteLine("Search completed.");
        }
    }
}
