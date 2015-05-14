using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using QL.Helpers;
using QL.Models;

namespace QL.Managers
{
    public class TrainingManager
    {
        public IHubContext Hub { get; set; }

        public Random Random { get; set; }

        public Settings Settings { get; set; }

        public List<QMap> QMap { get; set; }

        public List<QMap> Train(Scenario scenario, Settings settings, Random random, IHubContext hub)
        {
            Hub = hub;
            Random = random;
            Settings = settings;

            //Init agent
            var agent = new Agent();

            //Init array Q(state, a, r)
            QMap = new List<QMap>();

            //Init history log Q(state, a, r)
            var qHistory = new Queue<QMap>();

            //Main loop
            for (int i = 1; i <= settings.NumberOfIterations; i++)
            {
                //New copy of scenario
                var scenarioCopy = (int[,]) scenario.Values.Clone();

                //Reset agent
                agent.SetStartPosition(scenario.StartPosition);
                agent.State.EatenFoods.Clear();

                //Reset history
                qHistory.Clear();

                while (!(agent.State.EatenFoods.Count == scenario.NumberOfFoods && agent.State.Position.Equals(scenario.StartPosition)))//Game is not finished
                {
                    //Get current state
                    var state0 = (State)agent.State.Clone();

                    //Select action
                    var a = SelectAction(state0);

                    //Update agent state, world, and give reward
                    ScenarioHelper.GetNewPosition(a, agent, scenario.Height, scenario.Width);
                    var r = ScenarioHelper.UpdateScenario(scenarioCopy, agent, scenario.NumberOfFoods);

                    //Log Q(state0, a, r)
                    var loggedQ = QMap.FirstOrDefault(x => x.State.Equals(state0) && x.A == a);
                    if (loggedQ != null)
                    {
                        qHistory.Enqueue(loggedQ);
                    }
                    else
                    {
                        qHistory.Enqueue(new QMap { State = state0, A = a });
                    }
                    if (qHistory.Count > settings.HistorySize) qHistory.Dequeue();

                    //New state
                    var state1 = (State)agent.State.Clone();

                    //Update array(Q, state, a, r) from log
                    for (int j = qHistory.Count-1; j >= 0; j--)
                    {
                        var q = qHistory.ElementAt(j);
                        var futureState = (j == qHistory.Count - 1) ? state1 : qHistory.ElementAt(j + 1).State;
                        var maxQt1Value = GetBestValue(futureState);

                        q.Value += settings.LearningRate * (r + settings.DiscountRate * maxQt1Value - q.Value);

                        //Add to qMap
                        var policyQ = QMap.FirstOrDefault(x => x.State.Equals(q.State) && x.A == q.A);
                        if (policyQ != null)
                        {
                            policyQ.Value = q.Value;
                        }
                        else
                        {
                            QMap.Add(q);
                        }
                    }
                    //VisualizeScenario(agent, scenarioCopy);
                }
                VisualizeIteration(i);
            }

            return QMap;
        }

        private Direction SelectAction(State state)
        {
            var QInState = QMap.Where(x => x.State.Equals(state)).ToList();
            var bestQ = QInState.OrderByDescending(x=>x.Value).FirstOrDefault();
            var e = Random.NextDouble();

            if (bestQ != null && e > Settings.Epsilon)
            {
                if (bestQ.Value >= 0 || QInState.Count == 4)
                {
                    return bestQ.A;
                }
                else
                {
                    var directions = new List<Direction>();

                    if (!QInState.Where(x => x.A == Direction.Up).Any()) directions.Add(Direction.Up);
                    if (!QInState.Where(x => x.A == Direction.Right).Any()) directions.Add(Direction.Right);
                    if (!QInState.Where(x => x.A == Direction.Down).Any()) directions.Add(Direction.Down);
                    if (!QInState.Where(x => x.A == Direction.Left).Any()) directions.Add(Direction.Left);

                    var f = Random.Next(directions.Count);
                    return directions.ElementAt(f);
                }
            }
            else
            {
                return ScenarioHelper.GetRandomDirection(Random);
            }
        }

        private double GetBestValue(State state)
        {
            var QInState = QMap.Where(x => x.State.Equals(state)).ToList();
            var bestQ = QInState.OrderByDescending(x => x.Value).FirstOrDefault();

            if (bestQ != null && !(bestQ.Value < 0 && QInState.Count < 4))
            {
                return bestQ.Value;
            }
            else
            {
                return 0;
            }
        }

        private void VisualizeIteration(int iterationNumber)
        {
            Hub.Clients.All.hubVisualizeIteration(iterationNumber);
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