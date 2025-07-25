using Newtonsoft.Json;

namespace _20241CBE12B_G2.Models
{
        public partial class Clima
        {
            [JsonProperty("weather")]
            public Weather[] Weather { get; set; } = [];
            [JsonProperty("main")]
            public Main? Main { get; set; }
        }

        public partial class Main
        {
            [JsonProperty("temp")]
            public double Temp { get; set; }
        }

        public partial class Weather
        {
            [JsonProperty("main")]
            public string Main { get; set; } = string.Empty;
        }
    }


