using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace LogRushClient.Core;

public class LogRushLogger
{
    private struct RegisterLogRushStreamRequest
    {
        public RegisterLogRushStreamRequest() { }

        public string alias { get; init; } = "";
        public string id { get; init; } = "";
        public string key { get; init; } = "";
    }
    
    private struct UnregisterLogRushStreamRequest
    {
        public UnregisterLogRushStreamRequest() { }
        public string id { get; init; } = "";
        public string key { get; init; } = "";
    }
        
    private struct LogMessageRequest
    {
        public LogMessageRequest() { }

        public string stream { get; init; } = "";
        public string log { get; init; } = "";
        public long timestamp { get; init; } = 0;
    }

    public LogRushLogger(string alias, string address, string id = "", string key = "")
    {
        using var client = new HttpClient();
            
        var json = new RegisterLogRushStreamRequest { alias = alias, id = id, key = key };

        var httpRes = client.PostAsync($"{address}/stream/register", new StringContent(JsonSerializer.Serialize(json), Encoding.UTF8, "application/json")).Result;

        var str = httpRes.Content.ReadAsStringAsync().Result;

        if (!httpRes.IsSuccessStatusCode)
            throw new InvalidOperationException(str);
            
        var response = JsonNode.Parse(str);
        if ( response!["message"] != null && response["message"]?.ToString() != "")
            throw new InvalidOperationException(response["message"]?.ToString());
            
        _address = address;
        _id = response["id"]!.ToString();
        _key = response["key"]!.ToString();
    }
        
    public void Deconstruct()
    {
        var json = new UnregisterLogRushStreamRequest { id = _id, key = _key};
            
        using var client = new HttpClient();
        var httpRes = client.PostAsync($"{_address}/stream/unregister", new StringContent(JsonSerializer.Serialize(json), Encoding.UTF8, "application/json"));
    }
        
    public async Task Log(string log)
    {
        using var client = new HttpClient();

        var message = new LogMessageRequest
        {
            log = log,
            timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            stream = _id
        };

        var res = await client.PostAsync($"{_address}/log", new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json"));
        var str = await res.Content.ReadAsStringAsync();
            
        var response = JsonNode.Parse(str);

        if ( response!["message"] != null && response["message"]?.ToString() != "")
            throw new InvalidOperationException(response["message"]?.ToString());
    }

    private readonly string _address;
    private readonly string _id;
    private readonly string _key;
}