using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;
using QL.Models;

namespace QL.Managers
{
    public class ScenarioManager
    {
        public List<Scenario> GetInitialScenarios()
        {
            var scenarioList = new List<Scenario>();

            var targetDirectory = HttpContext.Current.Server.MapPath("~/Scenarios");
            var filepaths = Directory.GetFiles(targetDirectory);

            foreach (var filepath in filepaths)
            {
                var scenario = GetScenarioFromFile(filepath);
                scenarioList.Add(scenario);
            }

            return scenarioList;
        }

        private Scenario GetScenarioFromFile(string filepath)
        {
            var lines = File.ReadAllLines(filepath);
            var mapInfo =  lines[0].Split(' ').Select(int.Parse).ToArray();
            var width = mapInfo[0];
            var height = mapInfo[1];
            var startposX = mapInfo[2];
            var startposY = mapInfo[3];
            var numberOfFoods = mapInfo[4];

            int[,] map = new int[height, width];

            for (int i = 1; i < (height + 1); i++)
            {
                var row = lines[i].Split(' ').Select(int.Parse).ToArray();
                for (int j = 0; j < width; j++)
                {
                    map[(i-1), j] = row[j];
                }
            }

            return new Scenario
            {
                Values = map,
                Width = width,
                Height = height,
                StartPosition =
                {
                    Row = startposY,
                    Col = startposX
                },
                NumberOfFoods = numberOfFoods
            };
        }
    }
}