﻿using Newtonsoft.Json;

namespace CyberCity.Models.WeatherStantionModel
{
    [JsonObject("wind")]
    public class Wind
    {
        [JsonProperty("speed")]
        public double Speed;

        [JsonProperty("deg")]
        public double Deg;

        [JsonProperty("gust")]
        public double Gust;
    }
}
