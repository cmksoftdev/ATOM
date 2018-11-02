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
            ExecuteCodeFromUrl("https://code.msdn.microsoft.com/Execute-assemblies-from-864a0c57/file/216490/1/Debug.zap");
            Console.ReadLine();
        }
    }
}
