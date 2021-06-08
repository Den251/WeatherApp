using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WeatherApp.Models;

namespace WeatherApp.Controllers
{
    public class HomeController : Controller
    {
        private static Dictionary<string, DateTime> alertCitiesList = new Dictionary<string, DateTime>();
        private static bool rain = false;
        private static bool alert = false;
        private const string cookieKey = "lastCity";
        ///WeatherViewModel weatherForecast1 = new WeatherViewModel();

        //Method to get info by city
        private static WeatherViewModel MakeRequest(string city)
        {
            string apikey= "3b977a54a06908e31fc9ff4906d8e27b";
            string content;
            string url = "http://api.openweathermap.org/data/2.5/weather?q=" + city + "&appid=" + apikey +"&units=metric";
            WeatherViewModel weatherForecast = new WeatherViewModel();
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            if (stream != null)
            {


                using (StreamReader streamReader = new StreamReader(stream))
                {
                    content = streamReader.ReadToEnd();

                }
                dynamic weatherResponse = JsonConvert.DeserializeObject<dynamic>(content);



                
                weatherForecast = new WeatherViewModel()
                {
                    CityName = weatherResponse.name,
                    TempMax = weatherResponse.main.temp_max,
                    TempMin = weatherResponse.main.temp_min,
                    Description = weatherResponse.weather[0].main
                };
            }

            //Here, we decide should rain alert be displayed or not
            rain = false;
            if (weatherForecast.Description == "Rain" && !alertCitiesList.ContainsKey(city))
            {
                alertCitiesList.Add(city, DateTime.Now.AddDays(1));
                alert = true;
            }
            
            if (alertCitiesList.ContainsKey(city)&& alertCitiesList[city] > DateTime.Now && alert== true)
            {
                rain = true;
                alert = false;
            }
            return weatherForecast;
            
        }

        
        public IActionResult Index()
        {
            return View();
        }

        //Getting initial weather info using city from cookies
        public IActionResult GetWeather()
        {
            
            string lastCity = Request.Cookies[cookieKey];
            WeatherViewModel weatherForecast;
            try
            {
                weatherForecast = MakeRequest(lastCity);
                Console.WriteLine();
            }
            catch
            {
                weatherForecast = new WeatherViewModel();
            }
            ViewBag.Rain = rain;
            return View(weatherForecast);
        }

        //Getting weather info and saving last city to cookies
        [HttpPost]
        public IActionResult GetWeather(string cityName)
        {
            try
            {
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddDays(7);
                Response.Cookies.Append(cookieKey, cityName, options);
            }
            catch(ArgumentNullException)
            {
                //cookies are empty
            }
            

            WeatherViewModel weatherForecast = new WeatherViewModel();
            
            if (cityName != null)
            {
                
                try
                {
                    weatherForecast = MakeRequest(cityName);
                                       
                }
                catch
                {
                    ModelState.AddModelError(nameof(WeatherViewModel.CityName), "The city you have entered doesn't exist or api call has been colapsed");
                }
                
            }
            
            else
            {
                ModelState.AddModelError(nameof(WeatherViewModel.CityName), "Enter your city");
            }
            ViewBag.Rain = rain;
            return View(weatherForecast);

            

        }
    }
}
