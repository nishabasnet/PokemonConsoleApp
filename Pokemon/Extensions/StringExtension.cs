using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokemon.Extensions
{
    public static class StringExtension
    {
        public static string ToCSV(this IEnumerable<string> strings)
        {
            return string.Join(", ", strings);
        }
    }
}
