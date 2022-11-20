using Newtonsoft.Json;
using System.Collections.Generic;

namespace MTI860_collector.Models;

public class CollectRequest
{

    [JsonProperty("username")]
    public string Username { get; set; }
    
    [JsonProperty("laser")]
    public MethodCollect Laser { get; set; }
    
    [JsonProperty("virtualPen")]
    public MethodCollect VirtualPen { get; set; }
    
    [JsonProperty("handTracking")]
    public MethodCollect HandTracking { get; set; }
}

public class MethodCollect
{

    [JsonProperty("circles")]
    public IEnumerable<ShapeData> Circles { get; set; }

    [JsonProperty("triangles")]
    public IEnumerable<ShapeData> Triangles { get; set; }

    [JsonProperty("stars")]
    public IEnumerable<ShapeData> Stars { get; set; }
}


public class ShapeData
{
    public double ScoreCircle { get; set; }
    public double ScoreTriangle { get; set; }
    public double ScoreStar { get; set; }
    public double Time { get; set; }
}