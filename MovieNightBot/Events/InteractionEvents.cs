using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace MovieNightBot.Events;

public static class InteractionEvents
{
    public static async Task ModalSubmit(ModalSubmitEventArgs args)
    {
        var modalId = args.Interaction.Data.CustomId;
        switch (modalId)
        {
            case "movie-times-response":
            {
                MovieManager.Times(args);
                await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder()
                        .WithContent("Times submitted!").AsEphemeral());
                break;
            }
            case "movie-movies-response":
            {
                MovieManager.Movies(args);
                await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder()
                        .WithContent("Movies submitted!").AsEphemeral());
                break;
            }
        }
    }

    public static async Task ComponentInteract(DiscordClient client, ComponentInteractionCreateEventArgs args)
    {
        switch (args.Id)
        {
            case "movie-times":
            {
                await args.Interaction.CreateResponseAsync(InteractionResponseType.Modal,
                    new DiscordInteractionResponseBuilder()
                        .WithTitle("Movie Night Times")
                        .WithCustomId("movie-times-response")
                        .WithContent("Please do not enter more than **10** time options.")
                        .AddComponents(new TextInputComponent("Times", "movie-times-response-times",
                            value: "Saturday at 10pm\nSunday at 8pm",
                            style: TextInputStyle.Paragraph)));
                break;
            }
            case "movie-movies":
            {
                await args.Interaction.CreateResponseAsync(InteractionResponseType.Modal,
                    new DiscordInteractionResponseBuilder()
                        .WithTitle("Movie Night Movies")
                        .WithCustomId("movie-movies-response")
                        .AddComponents(new TextInputComponent("Movies", "movie-movies-response-movies",
                            value: "🧌 Shrek\n🐝 The Bee Movie",
                            style: TextInputStyle.Paragraph)));
                break;
            }
            case "movie-submit":
            {
                await MovieManager.Submit(client);
                await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("✅ Movie night created!"));
                break;
            }
        }
    }
}