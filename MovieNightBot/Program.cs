using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using MovieNightBot.Events;
using Serilog;

namespace MovieNightBot;

internal static class Program
{
    public static Config Options { get; private set; } = null!;

    private static async Task Main()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
        
        Options = (await Config.Load())!;

        var discord = new DiscordClient(new DiscordConfiguration
        {
            Token = Options.Token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged,
            LoggerFactory = new LoggerFactory().AddSerilog(),
        });

        var slashCommands = discord.UseSlashCommands();
        discord.Logger.LogInformation("[SLASH COMMAND] Registering slash commands.");
        slashCommands.RegisterCommands(typeof(Program).Assembly, Options.GuildId);
        
        // Register slash command events
        slashCommands.SlashCommandInvoked += (_, args) => SlashCommandEvents.SlashCommandInvoke(discord, args);
        slashCommands.SlashCommandExecuted += (_, args) => SlashCommandEvents.SlashCommandExecute(discord, args);
        slashCommands.SlashCommandErrored += (_, args) => SlashCommandEvents.SlashCommandError(discord, args);
        
        // Register client events
        discord.MessageCreated += async (_, args) => await MessageEvents.MessageCreate(discord, args);
        discord.ModalSubmitted += async (_, args) => await InteractionEvents.ModalSubmit(args);
        discord.ComponentInteractionCreated += async (_, args) => await InteractionEvents.ComponentInteract(discord, args);

        discord.SessionCreated += (client, _) =>
        {
            client.Logger.LogInformation("{username} is now online.", client.CurrentUser.Username);
            return Task.CompletedTask;
        };
        
        await discord.ConnectAsync();
        await Task.Delay(-1);
    }
}