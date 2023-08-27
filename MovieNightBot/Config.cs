using System.Text.Json;

namespace MovieNightBot;

public class Config
{
    public string Token { get; set; }
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
    public string TimesMessage { get; set; }
    public string MoviesMessage { get; set; }
    public ulong CreatorMessageId { get; set; }
    
    internal static async Task<Config?> Load()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var json = await File.ReadAllTextAsync("config.json");
        var config = JsonSerializer.Deserialize<Config>(json, options);

        return config;
    }

    internal static async Task Save(Config config)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(config, options);
        await File.WriteAllTextAsync("config.json", json);
    }
}