using System;
using LootExample.source;
namespace LootExample
{
    internal static class Program 
    {
      public static void Main()
      {
          var inputManager = new InputManager();
          
            while (true)
            {
                var input = Console.ReadLine();

                inputManager.Process(input);
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}