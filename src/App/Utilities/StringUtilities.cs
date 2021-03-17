using System;
using System.Collections.Generic;

namespace Ollio.Utilities
{
    public class StringUtilities
    {
        public static string SelectRandomString(List<String> strings = null)
        {
            int index = Program.Random.Next(strings.Count);
            return strings[index];
        }
    }
}