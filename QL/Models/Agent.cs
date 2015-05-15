using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace QL.Models
{
    public class Agent
    {
        public Agent()
        {
            State = new State();
        }

        [JsonProperty("state")]
        public State State { get; set; }

        public void SetStartPosition(Position position)
        {
            State.Position = new Position
            {
                Row = position.Row,
                Col= position.Col
            };
        }
    }

    public class State : IEquatable<State>, ICloneable
    {
        public State()
        {
            Position = new Position();
            EatenFoods = new List<int>();
        }

        [JsonProperty("position")]
        public Position Position { get; set; }

        [JsonProperty("eatenFoods")]
        public List<int> EatenFoods { get; set; }

        public bool Equals(State other)
        {
            return (Position.Equals(other.Position) && EatenFoods.All(other.EatenFoods.Contains) && EatenFoods.Count == other.EatenFoods.Count);
        }

        public object Clone()
        {
            return new State{ Position = (Position)Position.Clone(), EatenFoods = new List<int>(EatenFoods)};
        }
    }

    public class Position : IEquatable<Position>, ICloneable
    {
        [JsonProperty("row")]
        public int Row { get; set; }

        [JsonProperty("col")]
        public int Col { get; set; }

        public bool Equals(Position other)
        {
            return (Row == other.Row && Col == other.Col);
        }
        
        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public enum Direction
    {
        Up = 0, Left = 1, Down = 2, Right = 3
    }
}