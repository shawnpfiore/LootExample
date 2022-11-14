using System;
using LootExample.source;

namespace LootExample
{
    internal static class Program
    {
      public static void Main()
        {
            ProcessInput.Reload();

            while (true)
            {
                var input = Console.ReadLine();

                ProcessInput.Process(input);
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}