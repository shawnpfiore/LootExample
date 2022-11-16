using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LootExample.source.LootModel;

namespace LootExample.source
{
    public class InputManager : LootRecords
    {
        private Dictionary<string, Action> HandleAction =>
            new Dictionary<string, Action>
            {
                {"exit", Exit},
                {"help", Help},
                {"reload", Reload},
                {"clear", Clear}
            };

        public InputManager()
        {
            Reload();
        }

        /// <summary>
        /// Processes the users input
        /// </summary>
        /// <param name="input">The users input</param>
        public void Process(string input)
        {
            // try parsing the string as a command
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Nothing was entered please try again");
                return;
            }

            var subs = input.Split(' ');

            if (subs.Any() && subs.Length > 1)
            {
                var lootTableStr = subs[0];
                var countStr = subs[1];
                int.TryParse(countStr, out var count);

                if (count <= 0)
                {
                    Console.WriteLine("Value input must be a numeric value greater than 0");
                    return; 
                }

                CreateLootDrops(lootTableStr, count);
            }
            else if (HandleAction.ContainsKey(input))
            {
                HandleAction[input]();
            }
            else
            {
                Console.WriteLine($"Input {input} was not a valid input format, please try again or type help");
            }
        }

        /// <summary>
        /// Reloads the application and files 
        /// </summary>
        private void Reload()
        {
            LoadLootFiles();
            LoadLootRecord();
            HelpMessage();
        }

        /// <summary>
        /// Loads the selected loot.json
        /// </summary>
        private void LoadLootRecord()
        {
            while (true)
            {
                var input = Console.ReadLine();

                if (input != null && HandleAction.ContainsKey(input))
                {
                    HandleAction[input]();
                }
                else
                {
                    if (ParseLootRecord(input))
                        break;
                }
            }
        }

        /// <summary>
        /// Exits the application 
        /// </summary>
        private static void Exit()
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Displays the help message 
        /// </summary>
        private static void Help()
        {
            HelpMessage();
        }

        /// <summary>
        /// Clear the screen 
        /// </summary>
        private static void Clear()
        {
            Console.Clear();
        }

        /// <summary>
        /// Creates the help message contents 
        /// </summary>
        private static void HelpMessage()
        {
            Console.WriteLine(@"Welcome to LootGenerator!

USAGE
1. Type commands in the format:
		<TableName> <count>
	For example:
		""CurrencyTable 3""
	This will generate 3 drops of loot from the Currency table.
2. Type ""exit"" to close the program.
3. Type ""help"" to see this message again.
4. Type ""reload"" to load a new loot table file.
5. Type ""clear"" to clear screen");
        }
    }
}