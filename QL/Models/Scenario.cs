using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL.Models
{
    public class Scenario
    {
        public Scenario()
        {
            StartPosition = new Position();
        }

        public int[,] Values { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public Position StartPosition { get; set; }
        
        public int NumberOfFoods { get; set; }
    }
}