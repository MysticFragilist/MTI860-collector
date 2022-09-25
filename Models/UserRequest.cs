using Newtonsoft.Json;

namespace MTI860_collector.Models;

public class UserRequest
{
    [JsonProperty("username")]
    public string Username { get; set; }
}
