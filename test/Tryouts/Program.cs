﻿using System;
using System.Threading.Tasks;
using FastTests.Issues;
using FastTests.Server.Basic;
using Raven.Client.Documents;
using Raven.Client.Server;
using Raven.Client.Server.Operations;

namespace Tryouts
{
    public class Program
    {
        public static void Main(string[] args)
        {
            for (int i = 0; i < 10000; i++)
            {
                Console.WriteLine(i);

                using (var a = new RavenDB_5178())
                {
                    a.CanUsePathLongerThan260Chars();
                }
            }
        }
    }
}