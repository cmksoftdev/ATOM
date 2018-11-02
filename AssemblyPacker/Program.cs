using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyPacker
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args != null && args.Length == 1)
            {
                var files = new DirectoryInfo(args[0]).EnumerateFiles();

            }
        }
    }
}
