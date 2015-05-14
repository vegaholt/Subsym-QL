using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL.Models
{
    public class QMap
    {
        public State State { get; set; }
        
        public Direction A { get; set; }

        public double Value { get; set; }
    }
}