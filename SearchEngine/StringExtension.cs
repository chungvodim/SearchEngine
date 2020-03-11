using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SearchEngine
{
    public static class StringExtension
    {
        [DebuggerStepThrough]
        public static bool IsPresent(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        [DebuggerStepThrough]
        public static string Set(this string value, int index, char letter)
        {
            return value.Remove(index, 1).Insert(index, letter.ToString());
        }
    }
}
