using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace QL.Models
{
    public class Settings
    {
        [JsonProperty("scenarioIndex")]
        public int ScenarioIndex { get; set; }

        [JsonProperty("numberOfIterations")]
        public int NumberOfIterations { get; set; }

        [JsonProperty("learningRate")]
        public double LearningRate { get; set; }

        [JsonProperty("discountRate")]
        public double DiscountRate { get; set; }

        [JsonProperty("epsilon")]
        public double Epsilon { get; set; }

        [JsonProperty("historySize")]
        public int HistorySize { get; set; }

        [JsonProperty("interval")]
        public int Interval { get; set; }

    }
}