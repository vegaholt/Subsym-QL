using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QL.Models;

namespace QL.Helpers
{
    public static class ScenarioHelper
    {
        public static void GetNewPosition(Direction direction, Agent agent, int height, int width)
        {
            if (direction == Direction.Up)
            {
                agent.State.Position.Row = (((agent.State.Position.Row - 1) % height + height) % height);
            }
            else if (direction == Direction.Right)
            {
                agent.State.Position.Col = (agent.State.Position.Col + 1) % width;
            }
            else if (direction == Direction.Down)
            {
                agent.State.Position.Row = (agent.State.Position.Row + 1) % height;
            }
            else if (direction == Direction.Left)
            {
                agent.State.Position.Col = (((agent.State.Position.Col - 1) % width + width) % width);
            }
        }

        public static double UpdateScenario(int[,] scenarioCopy, Agent agent, int numberOfFoods)
        {
            var cellItem = scenarioCopy[agent.State.Position.Row, agent.State.Position.Col];

            if (cellItem == -1) //If poison
            {
                scenarioCopy[agent.State.Position.Row, agent.State.Position.Col] = 0; //remove poison
                return -3; //return negative reward
            }
            else if (cellItem > 0) //If food
            {
                scenarioCopy[agent.State.Position.Row, agent.State.Position.Col] = 0; //remove food
                agent.State.EatenFoods.Add(cellItem); //update agent food list
                return 2; //return positive reward
            }
            else if (cellItem == -2 && agent.State.EatenFoods.Count == numberOfFoods) //if startPos and all food is eaten
            {
                return 1;
            }
            else 
            { 
                return 0;
            }
        }

        public static Direction GetRandomDirection(Random random)
        {
            var f = random.NextDouble();
            var a = Direction.Up;
            if (f >= 0.25 && f < 0.5) a = Direction.Right;
            else if (f >= 0.5 && f < 0.75) a = Direction.Down;
            else if (f >= 0.75) a = Direction.Left;
            return a;
        }
    }
}