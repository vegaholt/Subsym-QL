using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using QL.Models;
using Microsoft.AspNet.SignalR;
using QL.Helpers;

namespace QL.Managers
{
    public class SimulationManager
    {
        public IHubContext Hub { get; set; }

        public List<QMap> Policy { get; set; }

        public int NumberOfSteps { get; set; }

        public int NumberOfEatenPoisons { get; set; }

        public void Run(Scenario scenario, List<QMap> policy, Random random, Settings settings, IHubContext hub)
        {
            Hub = hub;
            Policy = policy;
            NumberOfSteps = 0;
            NumberOfEatenPoisons = 0;

            //Init agent
            var agent = new Agent();

            //New copy of scenario
            var scenarioCopy = (int[,]) scenario.Values.Clone();

            //Reset agent
            agent.SetStartPosition(scenario.StartPosition);

            //Draw scenario
            VisualizeScenario(agent, scenarioCopy, scenario);

            do
            {
                var currentState = (State) agent.State.Clone();

                var q = policy.Where(x => x.State.Equals(currentState)).OrderByDescending(x => x.Value).FirstOrDefault();
                var a = (q != null) ? q.A : ScenarioHelper.GetRandomDirection(random);

                //update agent state, world, and give reward
                ScenarioHelper.GetNewPosition(a, agent, scenario.Height, scenario.Width);
                NumberOfEatenPoisons += ScenarioHelper.UpdateScenarioForSimulation(scenarioCopy, agent, scenario.NumberOfFoods);

                Thread.Sleep(settings.Interval);
                VisualizeScenarioWithArrows(agent, scenarioCopy, scenario);

            } while (
                !(agent.State.EatenFoods.Count == scenario.NumberOfFoods &&
                  agent.State.Position.Equals(scenario.StartPosition))); //Game is not finished
        }

        private void VisualizeScenarioWithArrows(Agent agent, int[,] scenarioCopy, Scenario scenario)
        {
            var scenarioWithArrows = new int[scenario.Height, scenario.Width];

            for (int i = 0; i < scenario.Height; i++)
            {
                for (int j = 0; j < scenario.Width; j++)
                {
                    if (scenarioCopy[i, j] != 0 && scenarioCopy[i,j] != -2)
                    {
                        scenarioWithArrows[i, j] = scenarioCopy[i, j];
                    }
                    else
                    {
                        var s = new State {Position = new Position{Row = i, Col = j}, EatenFoods = new List<int>(agent.State.EatenFoods)};
                        var q = Policy.Where(x => x.State.Equals(s)).OrderByDescending(x => x.Value).FirstOrDefault();
                        var a = 0;
                        if (q != null)
                        {
                            if (q.A == Direction.Up) a = -3;
                            if (q.A == Direction.Right) a = -4;
                            if (q.A == Direction.Down) a = -5;
                            if (q.A == Direction.Left) a = -6;
                        }
                        scenarioWithArrows[i, j] = a;
                    }
                }
            }

            var viewModel = new ViewModel
            {
                Scenario = scenarioWithArrows,
                Agent = agent,
                StartPos = scenario.StartPosition,
                Width = scenario.Width,
                Height = scenario.Height
            };

            Hub.Clients.All.hubVisualizeScenario(viewModel);
        }

        private void VisualizeScenario(Agent agent, int[,] scenarioCopy, Scenario scenario)
        {
            var viewModel = new ViewModel
            {
                Scenario = scenarioCopy,
                Agent = agent,
                StartPos = agent.State.Position,
                Width = scenario.Width,
                Height = scenario.Height,
                NumberOfSteps = 0,
                NumberOfEatenPoisons = 0
            };

            Hub.Clients.All.hubVisualizeScenario(viewModel);
        }
    }
}