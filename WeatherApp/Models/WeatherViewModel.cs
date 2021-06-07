using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherApp.Models
{
    public class WeatherViewModel
    {
        
        public string CityName { get; set; }
        public double TempMax { get; set; }
        public double TempMin { get; set; }
        public string Description { get; set; }
        
    }

    
}
