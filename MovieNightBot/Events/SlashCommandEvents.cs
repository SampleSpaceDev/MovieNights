using System.Diagnostics;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.Extensions.Logging;

namespace MovieNightBot.Events;

public class SlashCommandEvents
{
    private static readonly Dictionary<ulong, Stopwatch> Timers = new();

    public static Task SlashCommandInvoke(BaseDiscordClient discord, SlashCommandInvokedEventArgs args)
    {
        var cmd = args.Context.QualifiedName;
        var member = $"{args.Context.Member.Username}#{args.Context.Member.Discriminator}";
        var channel = args.Context.Channel.Name;

        Timers[args.Context.Interaction.Id] = Stopwatch.StartNew();

        discord.Logger.LogInformation($"[SLASH COMMAND] [#{channel}] {member} invoked command /{cmd}");
        return Task.CompletedTask;
    }

    public static Task SlashCommandExecute(BaseDiscordClient discord, SlashCommandExecutedEventArgs args)
    {
        var cmd = args.Context.QualifiedName;
        var member = $"{args.Context.Member.Username}#{args.Context.Member.Discriminator}";
        var channel = args.Context.Channel.Name;

        var stopwatch = Timers[args.Context.Interaction.Id];
        stopwatch.Stop();
        Timers.Remove(args.Context.Interaction.Id);

        discord.Logger.LogInformation(
            $"[SLASH COMMAND] [#{channel}] {member} executed command /{cmd} ({stopwatch.ElapsedMilliseconds:N0}ms)");

        return Task.CompletedTask;
    }

    public static async Task SlashCommandError(BaseDiscordClient discord, SlashCommandErrorEventArgs args)
    {
        var command = args.Context.QualifiedName;
        var exceptionMessage = args.Exception.Message;
        var stackTrace = args.Exception.StackTrace;
        var member = $"{args.Context.Member.Username}#{args.Context.Member.Discriminator}";
        var channel = args.Context.Channel.Name;

        discord.Logger.LogError(
            $@"[ERROR] [#{channel}] {member} encountered an error with slash command /{command}: {exceptionMessage} 
                        {stackTrace}");

        try
        {
            await args.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent("There was an error! Please try again.").AsEphemeral());
        }
        catch (BadRequestException) { /* ignored */ }
    }
}