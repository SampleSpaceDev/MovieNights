using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;

namespace MovieNightBot;

internal static class Program
{
    private static Config? Options { get; set; }

    private static async Task Main()
    {
        Options = (await Config.Load())!;

        var discord = new DiscordClient(new DiscordConfiguration
        {
            Token = Options.Token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged
        });

        var slashCommands = discord.UseSlashCommands();
        discord.Logger.LogInformation("[SLASH COMMAND] Registering slash commands.");
        slashCommands.RegisterCommands(typeof(Program).Assembly, Options.GuildId);
        
        await discord.ConnectAsync();
        await Task.Delay(-1);
    }
}