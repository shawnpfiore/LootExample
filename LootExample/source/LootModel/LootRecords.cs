using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace LootExample.source.LootModel
{
    public static class LootRecords
    {
        private enum TableTypes
        {
            UniqueRandom
        }

        private const int MaxSkips = 10;
        private static readonly Random Rnd = new Random();
        private static readonly List<string> UniqueRandomDrops = new List<string>();
        private static Dictionary<string, LootTable> _lootTableDict = new Dictionary<string, LootTable>();
        private static readonly Dictionary<TableEntryCollection, int> EntryDropCollection = new Dictionary<TableEntryCollection, int>();
        private static int _uniqueSkipAmount = MaxSkips;
        private static string _path;
        private static string[] _files;
        
        /// <summary>
        /// Load all loot.json files 
        /// </summary>
        public static void LoadLootFiles()
        {
            _path = Path.Combine(
                Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())) ?? string.Empty,
                @"Data\");

            if (_path != null) _files = Directory.GetFiles(_path, "*.json");

            if (_files.Any())
            {
                Console.WriteLine(_files?.Aggregate("Enter loot table file name: ",
                                      (current, file) => current + $"{Path.GetFileName(file)}, ") +
                                  $"Or press RETURN to use default file {Path.GetFileName(_files?[0])}");
            }
            else
            {
                Console.WriteLine("There were no valid loot files found to load, please try again");
            }
        }

        /// <summary>
        /// Parses the loot.json file
        /// </summary>
        /// <param name="jsonPath">The loot.json file to parse</param>
        /// <returns></returns>
        public static bool ParseLootRecord(string jsonPath)
        {
            bool success; 
            try
            {
                var selectedFile = string.IsNullOrEmpty(jsonPath) ? _files[0] : _files.First(f => f.Contains(jsonPath));

                Console.WriteLine(
                    $"Loading file {Path.GetFileName(selectedFile)}");
                
                success = true;
                
                var json = File.ReadAllText(selectedFile);

                _lootTableDict = JsonConvert.DeserializeObject<List<LootTable>>(json)?.ToDictionary(t => t.TableName);
            }
            catch
            {
                success = false; 
                Console.WriteLine(
                    $"File {jsonPath} does not exists, you misspelled the file name, or the file is not valid, please try again");
            }

            return success; 
        }

       /// <summary>
       /// Displays the loot drops to console 
       /// </summary>
       /// <param name="lootTableStr">The loot table to sue</param>
       /// <param name="count">The count of opportunities</param>
        public static void DisplayLootDrops(string lootTableStr, int count)
        {
                    
            if (!_lootTableDict.Keys.Contains(lootTableStr))
            {
                Console.WriteLine($"Could not find TableName: {lootTableStr}, please try again");
                return;
            }
           
            var totalWeight = _lootTableDict[lootTableStr].TableEntryCollection.Sum(c => c.SelectionWeight);

            for (var i = 0; i < count; i++)
            {
                RandomiseLootDrop(SelectWeightedTableEntry(_lootTableDict[lootTableStr], totalWeight));
            }

            foreach (var c in EntryDropCollection)
            {
                Console.WriteLine($"Dropped {c.Value} {c.Key.EntryName}");
            }
            
            EntryDropCollection?.Clear();

        }

       /// <summary>
       /// Randomise the loot drop amount
       /// </summary>
       /// <param name="entry">The TableEntry to use</param>
        private static void RandomiseLootDrop(TableEntryCollection entry)
        {
            if (entry == null) return;
            var count = Rnd.Next(entry.MinDrops, entry.MaxDrops);
            if (!EntryDropCollection.ContainsKey(entry))
            {
                EntryDropCollection.Add(entry, count);
            }
            else
            {
                EntryDropCollection[entry] += count;
            }
        }
        
       /// <summary>
       /// Selects a weighted table entry
       /// </summary>
       /// <param name="lootTable">The loot table to use</param>
       /// <param name="totalWeight">The total weight amount from all loot tables entries</param>
       /// <returns>The selected loot table entry</returns>
        private static TableEntryCollection SelectWeightedTableEntry(LootTable lootTable, double totalWeight)
        {
            
            // totalWeight is the sum of all TableEntries' SelectionWeight
            var randomNumber = Rnd.NextDouble() *  totalWeight;

            TableEntryCollection selectableEntry = null;
            
            foreach (var entry in lootTable.TableEntryCollection)
            {
                if (randomNumber < entry.SelectionWeight)
                {
                    selectableEntry = entry;
                    break;
                }

                randomNumber -= entry.SelectionWeight;
            }

            if (lootTable.TableType != TableTypes.UniqueRandom.ToString()) return selectableEntry;

            if (_uniqueSkipAmount != 0)
            {
                if(selectableEntry != null && !UniqueRandomDrops.Contains(selectableEntry.EntryName))
                {
                    UniqueRandomDrops.Add(selectableEntry.EntryName);
                    _uniqueSkipAmount--;
                    return selectableEntry;
                }
            }
            else
            {
                if(UniqueRandomDrops.Any())
                    UniqueRandomDrops.Clear();
                _uniqueSkipAmount = MaxSkips;
            }
          
            _uniqueSkipAmount--;
            return null;
        }
    }
}