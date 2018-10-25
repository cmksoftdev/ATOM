﻿using CMK;
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
            var asm = GetAssemblyFromUrl("http://cmk.bplaced.net/download/TestAssembly.dll");
            var callableClass = Get<ICallable>("TestAssembly", "Class1", asm, null);
            callableClass.Call();
            Console.ReadLine();
        }
    }
}