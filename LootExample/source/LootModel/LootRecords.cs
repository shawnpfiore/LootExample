using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace LootExample.source.LootModel
{
    public class LootRecords
    {
        private enum TableTypes
        {
            UniqueRandom
        }

        private enum EntryTypes
        {
            Item,
            Table
        }

        private int _desiredCount;
        private const int MaxSkips = 5;
        private readonly Random _rnd = new Random();
        private readonly List<string> _uniqueRandomDrops;
        private Dictionary<string, LootTable> _lootTableDict;
        private readonly Dictionary<TableEntryCollection, int> _entryDropCollection;
        private int _uniqueSkipAmount = MaxSkips;
        private string _path;
        private string[] _files;

        protected LootRecords()
        {
            _uniqueRandomDrops = new List<string>();
            _lootTableDict = new Dictionary<string, LootTable>();
            _entryDropCollection = new Dictionary<TableEntryCollection, int>();
        }

        /// <summary>
        /// Load all loot.json files 
        /// </summary>
        protected void LoadLootFiles()
        {
            _path = Path.Combine(
                Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())) ?? string.Empty,
                $"Data{Path.DirectorySeparatorChar}");

            if (_path != null)
            {
                _files = Directory.GetFiles(_path, "*.json");
            }
            else
            {
                Console.WriteLine("Could not locate folder path for files please check your configuration");
            }

            if (_files != null)
            {
                Console.WriteLine(_files?.Aggregate("Enter loot table file name: ",
                                      (current, file) => current + $"{Path.GetFileName(file)}, ") +
                                  $"Or press RETURN to use default file {Path.GetFileName(_files?[0])}");
            }
            else
            {
                Console.WriteLine("There were no valid loot files found to load please try again");
            }
        }

        /// <summary>
        /// Parses the loot.json file
        /// </summary>
        /// <param name="jsonPath">The loot.json file to parse</param>
        /// <returns></returns>
        protected bool ParseLootRecord(string jsonPath)
        {
            bool success;
            try
            {
                var selectedFile = string.IsNullOrEmpty(jsonPath) ? _files[0] : _files.First(f => f.Contains(jsonPath));

                if (selectedFile == null)
                {
                    Console.WriteLine(
                        $"File {jsonPath} does not exists");
                    return false;
                }

                var json = File.ReadAllText(selectedFile);

                _lootTableDict = JsonConvert.DeserializeObject<List<LootTable>>(json)?.ToDictionary(t => t?.TableName);

                if (_lootTableDict != null)
                {
                    Console.WriteLine(
                        $"Loading file {Path.GetFileName(selectedFile)}");
                    success = true;
                }
                else
                {
                    Console.WriteLine(
                        $"There was a problem loading {Path.GetFileName(selectedFile)} please check the file and try again");
                    success = false;
                }
            }
            catch (Exception e)
            {
                success = false;
                Console.WriteLine(
                    $"The file is not in valid format please try again, Error Message: {e.Message}");
            }

            return success;
        }

        /// <summary>
        /// Displays the loot drops to console 
        /// </summary>
        /// <param name="lootTableStr">The loot table to sue</param>
        /// <param name="count">The count of opportunities</param>
        protected void CreateLootDrops(string lootTableStr, int count)
        {
            if (!_lootTableDict.Keys.Contains(lootTableStr))
            {
                Console.WriteLine($"Could not find TableName: {lootTableStr}, please try again");
                return;
            }

            _desiredCount = count;

            var totalWeight = _lootTableDict[lootTableStr].TableEntryCollection.Sum(c => c.SelectionWeight);

            for (var i = 0; i < count; i++)
            {
                RandomiseItemLootDrops(SelectWeightedTableEntry(_lootTableDict[lootTableStr], totalWeight));
            }

            DisplayLootDrops();
        }

        /// <summary>
        /// Displays the loot drops to user
        /// </summary>
        private void DisplayLootDrops()
        {
            foreach (var c in _entryDropCollection)
            {
                Console.WriteLine($"Dropped {c.Value} {c.Key.EntryName}");
            }

            _entryDropCollection?.Clear();
        }

        /// <summary>
        /// Randomise the loot drop amount
        /// </summary>
        /// <param name="entry">The TableEntry to use</param>
        private void RandomiseItemLootDrops(TableEntryCollection entry)
        {
            if (entry == null) return;

            if (entry.EntryType == EntryTypes.Table.ToString())
            {
                RandomiseTableLootDrops(entry);
            }
            else
            {
                var randNumber = _rnd.Next(entry.MinDrops, entry.MaxDrops);
                if (!_entryDropCollection.ContainsKey(entry))
                {
                    _entryDropCollection.Add(entry, randNumber);
                }
                else
                {
                    _entryDropCollection[entry] += randNumber;
                }
            }
        }

        /// <summary>
        /// Randomise table loot drops 
        /// </summary>
        /// <param name="entry">The table entry collection</param>
        private void RandomiseTableLootDrops(TableEntryCollection entry)
        {
            if (entry == null) return;

            try
            {
                var bounds = _rnd.Next(entry.MinDrops, entry.MaxDrops);

                for (var i = 0; i < bounds; i++)
                {
                    var totalWeight = _lootTableDict[entry.EntryName].TableEntryCollection.Sum(c => c.SelectionWeight);

                    for (var j = 0; j < _desiredCount; j++)
                    {
                        var selectedTableEntry = SelectWeightedTableEntry(_lootTableDict[entry.EntryName], totalWeight);

                        RandomiseItemLootDrops(selectedTableEntry);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to find {_lootTableDict[entry.EntryName]}, Error Message: {e.Message}");
            }
        }

        /// <summary>
        /// Uninformatively, Ran out of time trying to finish this. I failed to read the instruction thoroughly before handing in the test
        /// Thanks for the opportunity and your feed back :)
        /// </summary>
        /// <param name="selectableEntry"></param>
        private void UniqueRandom(TableEntryCollection selectableEntry)
        {
            if (_uniqueSkipAmount != 0)
            {
                if (selectableEntry != null && !_uniqueRandomDrops.Contains(selectableEntry.EntryName))
                {
                    _uniqueRandomDrops.Add(selectableEntry.EntryName);
                    _uniqueSkipAmount--;
                }
            }
            else
            {
                if (_uniqueRandomDrops.Any())
                    _uniqueRandomDrops.Clear();
                _uniqueSkipAmount = MaxSkips;
            }

            _uniqueSkipAmount--;
        }

        /// <summary>
        /// Selects a weighted table entry
        /// </summary>
        /// <param name="lootTable">The loot table to use</param>
        /// <param name="totalWeight">The total weight amount from all loot tables entries</param>
        /// <returns>The selected loot table entry</returns>
        private TableEntryCollection SelectWeightedTableEntry(LootTable lootTable, double totalWeight)
        {
            // totalWeight is the sum of all TableEntries' SelectionWeight
            var randomNumber = _rnd.NextDouble() * totalWeight;

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

            return selectableEntry;
        }
    }
}