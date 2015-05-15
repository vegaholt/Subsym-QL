using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QL.Models;

namespace QL.Managers
{
    public class SettingsManager
    {
        public Settings GetInitialSettings()
        {
            return new Settings
            {
                ScenarioIndex = 1,
                NumberOfIterations = 1000,
                LearningRate = 0.1,
                DiscountRate = 0.5,
                Epsilon = 0.3,
                HistorySize = 3,
                Interval = 250
            };
        }
    }
}