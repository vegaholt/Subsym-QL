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
                NumberOfIterations = 3000,
                LearningRate = 0.1,
                DiscountRate = 0.5,
                Epsilon = 0.1,
                HistorySize = 2,
                Interval = 250
            };
        }
    }
}