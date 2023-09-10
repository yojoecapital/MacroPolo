using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroPoloCore.Utilities
{
    internal static class Helpers
    {
        public static Tuple<string, string> SplitStringFromFirstInstance(this string input, string separator)
        {
            int indexOfFirstInstance = input.IndexOf(separator);

            if (indexOfFirstInstance != -1)
            {
                string firstHalf = input[..indexOfFirstInstance];
                string secondHalf = input[(indexOfFirstInstance + separator.Length)..];
                return new Tuple<string, string>(firstHalf, secondHalf);
            }
            else return new Tuple<string, string>(input, string.Empty);
        }

        public static string FormatFirstUpper(this string input) => char.ToUpper(input[0]) + input[1..];
        public static string FormatFirstLower(this string input) => char.ToLower(input[0]) + input[1..];

        public static string ReplaceFirstInstance(this string input, string searchTerm, string replacement)
        {
            int indexOfFirstInstance = input.IndexOf(searchTerm);

            if (indexOfFirstInstance != -1)
            {
                string result = input[..indexOfFirstInstance] + replacement +
                                input[(indexOfFirstInstance + searchTerm.Length)..];
                return result;
            }
            else return input;
        }

        public static string ReplaceLastInstance(this string input, string searchTerm, string replacement)
        {
            int indexOfLastInstance = input.LastIndexOf(searchTerm);

            if (indexOfLastInstance != -1)
            {
                string result = input[..indexOfLastInstance] + replacement +
                                input[(indexOfLastInstance + searchTerm.Length)..];
                return result;
            }
            else return input;
        }
    }
}
