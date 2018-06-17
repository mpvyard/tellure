using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tellure.Algorithms
{
    public struct Template
    {
        public int Distance1;
        public int Distance2;
        public int Distance3;
        public int Distance4;


        public static Template Parse(string template)
        {
            string name = Path.GetFileName(template);
            int ln = name.IndexOf('.');
            string temp = name.Substring(0, ln);
            string[] parts = temp.Split('-');
            int[] numbers = parts.Where(x =>
                int.TryParse(x, out var _)
                ).Select(x => int.Parse(x)).ToArray();

            return new Template
            {
                Distance1 = numbers[0],
                Distance2 = numbers[1],
                Distance3 = numbers[2],
                Distance4 = numbers[3]
            };
        }
    }
}
