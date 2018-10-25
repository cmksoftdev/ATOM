using CMK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CMK.ATOM;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ExecuteCodeFromUrl("http://cmk.bplaced.net/download/Debug.zip");
            Console.ReadLine();
        }
    }
}
