using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace QL.Models
{
    public class QMap
    {
        public virtual State State { get; set; }
        
        public Direction A { get; set; }

        public double Value { get; set; }
    }
}