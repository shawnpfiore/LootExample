using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LootExample.source.LootModel
{
    [Serializable]
    public class LootTable
    {
        [JsonProperty("TableType")]
        public string TableType { get; set; }
        
        [JsonProperty("TableName")]
        public string TableName { get; set; }

        [JsonProperty("TableEntryCollection")]
        public List<TableEntryCollection> TableEntryCollection { get; set; }
    }
}