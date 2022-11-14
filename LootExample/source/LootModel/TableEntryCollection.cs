using System;
using Newtonsoft.Json;

namespace LootExample.source.LootModel
{
    [Serializable]
    public class TableEntryCollection
    {
        [JsonProperty("EntryType")]
        public string EntryType { get; set; }

        [JsonProperty("EntryName")]
        public string EntryName { get; set; }
        
        [JsonProperty("MinDrops")]
        public int MinDrops { get; set; }
        
        [JsonProperty("MaxDrops")]
        public int MaxDrops { get; set; }
        
        [JsonProperty("SelectionWeight")]
        public float SelectionWeight { get; set; }
    }
}