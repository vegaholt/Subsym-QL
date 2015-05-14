using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace QL.Models
{
    public class ViewModel
    {
        [JsonProperty("scenario")]
        public int[,] Scenario { get; set; }

        [JsonProperty("agent")]
        public Agent Agent { get; set; }
    }
}