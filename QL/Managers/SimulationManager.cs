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

        public void Run(Scenario scenario, List<QMap> policy, Random random, IHubContext hub)
        {
            Hub = hub;

            //Init agent
            var agent = new Agent();

            //New copy of scenario
            var scenarioCopy = (int[,]) scenario.Values.Clone();

            //Reset agent
            agent.SetStartPosition(scenario.StartPosition);

            while (!(agent.State.EatenFoods.Count == scenario.NumberOfFoods && agent.State.Position.Equals(scenario.StartPosition)))//Game is not finished
            {
                var currentState = new State { Position = (Position)agent.State.Position.Clone(), EatenFoods = new List<int>(agent.State.EatenFoods) };
                
                var q = policy.Where(x => x.State.Equals(currentState)).OrderByDescending(x => x.Value).FirstOrDefault();
                var a = (q != null) ? q.A : ScenarioHelper.GetRandomDirection(random);

                //update agent state, world, and give reward
                ScenarioHelper.GetNewPosition(a, agent, scenario.Height, scenario.Width);
                ScenarioHelper.UpdateScenario(scenarioCopy, agent, scenario.NumberOfFoods);

                VisualizeScenario(agent, scenarioCopy);

                Thread.Sleep(400);
            }
        }

        private void VisualizeScenario(Agent agent, int[,] scenarioCopy)
        {
            var viewModel = new ViewModel
            {
                Scenario = scenarioCopy,
                Agent = agent
            };

            Hub.Clients.All.hubVisualizeScenario(viewModel);
        }
    }
}