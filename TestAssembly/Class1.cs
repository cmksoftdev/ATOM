using System;
using static CMK.ATOM;

namespace TestAssembly
{
    public class Class1 : IStart
    {
        public void EntryPoint()
        {
            Console.WriteLine("Hallo.");
            Console.WriteLine("It's working.");
            Console.WriteLine("Good.");
        }
    }
}
