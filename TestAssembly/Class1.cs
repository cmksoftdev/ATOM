using System;
using static CMK.ATOM;

namespace TestAssembly
{
    public class Class1 : ICallable
    {
        public void Call()
        {
            Console.WriteLine("Hallo.");
            Console.WriteLine("It's working.");
            Console.WriteLine("Good.");
        }
    }
}
