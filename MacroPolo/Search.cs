using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroPolo
{
    public static class Search
    {
        static int LongestCommonSubsequence(string s1, string s2)
        {
            int[,] dp = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    if (s1[i - 1] == s2[j - 1])
                    {
                        dp[i, j] = dp[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                    }
                }
            }

            return dp[s1.Length, s2.Length];
        }

        private static IEnumerable<KeyValuePair<string, string>> OrderByLongestCommonSubsequence(Dictionary<string, string> macros, string search)
        {
            return macros.OrderByDescending(pair => LongestCommonSubsequence(pair.Value, search));
        }

        public static void Run(Dictionary<string, string> macros) => Run(macros as IEnumerable<KeyValuePair<string, string>>);
        public static void Run(Dictionary<string, string> macros, string search) => Run(OrderByLongestCommonSubsequence(macros, search));

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
                    Macro.PrettyPrint(searchResult);
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
