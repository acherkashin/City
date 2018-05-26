using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CyberCity.Models.WeatherStantionModel;

namespace CyberCity.Controllers
{
    public class TownController : Controller
    {
        //public List<Town> GetCities()
        //{
        //    var cities = new List<Town>();

        //    var fileContent = Properties.Resources.Cities;

        //    var array = fileContent.Replace("\r\n", " ").Split(' ');

        //    foreach (var city in array)
        //    {
        //        cities.Add(new Town
        //        {
        //            Key = city.Substring(0, city.IndexOf(':')),
        //            Name = city.Substring(city.IndexOf(':') + 1)
        //        });
        //    }

        //    return cities;
        //}
    }
}