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

        [JsonProperty("startPos")]
        public Position StartPos { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        public int NumberOfSteps { get; set; }

        public int NumberOfEatenPoisons { get; set; }
    }
}