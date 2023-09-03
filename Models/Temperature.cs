using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rest_api.Models
{
    public class Temperature
    {
        public int Id { get; set; }
        public int Temp { get; set; }
        public DateTime DateTime { get; set; }
    }
}