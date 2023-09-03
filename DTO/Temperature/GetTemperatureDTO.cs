using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rest_api.DTO.Temperature
{
    public class GetTemperatureDTO
    {
        public int Id { get; set; }
        public int Temp { get; set; }
        public DateTime DateTime { get; set; }
    }
}