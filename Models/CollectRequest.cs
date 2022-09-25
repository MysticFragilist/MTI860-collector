using Newtonsoft.Json;
using System.Collections.Generic;

namespace MTI860_collector.Models;

public class CollectRequest
{

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("circles")]
    public IEnumerable<double> Circles { get; set; }

    [JsonProperty("triangles")]
    public IEnumerable<double> Triangles { get; set; }

    [JsonProperty("stars")]
    public IEnumerable<double> Stars { get; set; }
}
