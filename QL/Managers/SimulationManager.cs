using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Timers;
using QL.Models;
using Microsoft.AspNet.SignalR;
using QL.Helpers;

namespace QL.Managers
{
    public class SimulationManager
    {
        public InitializerManager InitializerManager { get; set; }

        public IHubContext Hub { get; set; }

        public List<QMap> Policy { get; set; }

        public Scenario Scenario { get; set; }

        public int[,] ScenarioCopy { get; set; }

        public Agent Agent { get; set; }
        
        public int NumberOfSteps { get; set; }

        public int NumberOfEatenPoisons { get; set; }
        
        public Settings Settings { get; set; }

        public Random Random { get; set; }

        public void Run(InitializerManager initManager, Scenario scenario, List<QMap> policy, Random random, Settings settings, IHubContext hub)
        {
            InitializerManager = initManager;
            Hub = hub;
            Policy = policy;
            Scenario = scenario;
            Agent = new Agent();
            ScenarioCopy = (int[,]) scenario.Values.Clone();
            Agent.SetStartPosition(scenario.StartPosition);
            NumberOfSteps = 0;
            NumberOfEatenPoisons = 0;
            VisualizeScenario(Agent, ScenarioCopy, scenario);
        }

        public void OnTimedEvent()
        {
            var currentState = (State) Agent.State.Clone();

            var q = Policy.Where(x => x.State.Equals(currentState)).OrderByDescending(x => x.Value).FirstOrDefault();
            var a = (q != null) ? q.A : ScenarioHelper.GetRandomDirection(Random);

            //update agent state, world, and give reward
            ScenarioHelper.GetNewPosition(a, Agent, Scenario.Height, Scenario.Width);
            NumberOfEatenPoisons += ScenarioHelper.UpdateScenarioForSimulation(ScenarioCopy, Agent, Scenario.NumberOfFoods);
            NumberOfSteps++;

            VisualizeScenarioWithArrows(Agent, ScenarioCopy, Scenario);

            if (Agent.State.EatenFoods.Count == Scenario.NumberOfFoods &&
                  Agent.State.Position.Equals(Scenario.StartPosition)) //Game is finished
            {
                InitializerManager.StopSimulation();
            } 
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
                Height = scenario.Height,
                NumberOfSteps = NumberOfSteps,
                NumberOfEatenPoisons = NumberOfEatenPoisons
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