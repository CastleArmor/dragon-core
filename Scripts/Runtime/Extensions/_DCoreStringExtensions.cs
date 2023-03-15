using System;
using System.Linq;

namespace Dragon.Core
{
    public static class _DCoreStringExtensions
    {
        public static string RemoveUntilString(this string input, string untilString)
        {
            int index = input.LastIndexOf(untilString, StringComparison.Ordinal);
            string returned = input;
            if (index > 0)
            {
                returned = input.Substring(0, index);
                returned = input.Replace(returned, "");
            }

            return returned;
        }
        
        public static string GetFriendlyWord(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1).ToLower();
            }
        }

        public static string CombineWithSeparator(this string[] array, string separator)
        {
            string formed = "";
            for (int i = 0; i<array.Length; i++)
            {
                string str = array[i];
                formed += str;

                if (i != array.Length - 1)
                {
                    formed += separator;
                }
            }

            return formed;
        }
    }
}