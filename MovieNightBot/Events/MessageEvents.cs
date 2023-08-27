using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;

namespace MovieNightBot.Events;

public static class MessageEvents
{
    public static async Task MessageCreate(BaseDiscordClient discord, MessageCreateEventArgs ctx)
    {
        DiscordMessage message = ctx.Message;
            
        var metadata = GetMetadata(message);
        var author = $"{message.Author.Username}#{message.Author.Discriminator}";
        var channel = ctx.Channel.Type switch
        {
            ChannelType.Text or
                ChannelType.Voice or
                ChannelType.Stage or
                ChannelType.News => $"#{message.Channel.Name}",
            ChannelType.Private or
                ChannelType.Group => "DM",
            ChannelType.PublicThread or
                ChannelType.PrivateThread or
                ChannelType.NewsThread => $"THREAD \"{ctx.Channel.Name}\"",
            ChannelType.GuildForum => $"FORUM \"{ctx.Channel.Name}\"",
            _ => "unknown"
        };
                
        var log = $"[MESSAGE] [{channel}] [{author}]: \"{message.Content}\" {(metadata.Length > 2 ? $"({metadata})" : "")}";
        discord.Logger.LogInformation(log);
    }

    private static string GetMetadata(DiscordMessage message) {
        var values = new List<string>();
        var attachments = message.Attachments.Count;
        var embeds = message.Embeds.Count;
        var users = message.MentionedUsers.Count;
        var roles = message.MentionedRoles.Count;
        if (attachments > 0) {
            values.Add("Attachments: " + attachments);
        }
        if (embeds > 0) {
            values.Add("Embeds: " + embeds);
        }
        if (users > 0) {
            values.Add("Mentioned users: " + users);
        }
        if (roles > 0) {
            values.Add("Mentioned roles: " + roles);
        }

        return string.Join(", ", values);
    }
}