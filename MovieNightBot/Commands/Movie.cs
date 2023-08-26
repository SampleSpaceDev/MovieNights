using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace MovieNightBot.Commands;

[SlashCommandGroup("movie", "Movie commands.")]
public class Movie : ApplicationCommandModule
{

    [SlashCommandGroup("manage", "")]
    [SlashRequireOwner]
    public class Manage : ApplicationCommandModule
    {
        private static DiscordButtonComponent TimesButton = new DiscordButtonComponent(ButtonStyle.Primary, "movie-times", "🕑 Times");
        private static DiscordButtonComponent MoviesButton = new DiscordButtonComponent(ButtonStyle.Primary, "movie-movies", "🎬 Movies");
        private static DiscordButtonComponent SubmitButton = new DiscordButtonComponent(ButtonStyle.Success, "movie-submit", "✅ Submit");
        
        private DiscordMessageBuilder CreateMessage = new DiscordMessageBuilder()
            .AddEmbed(new DiscordEmbedBuilder()
                .WithAuthor("Movie Night Creator")
                .WithDescription("You can create a movie night by using the buttons below. Each one is explained here:")
                .AddField("🕑 Times", @"To make it easier for you, the times do not need to be given as timestamps. Instead, you can simply say something like `Saturday at 9pm` and the bot will do the rest for you. When you click the button, you will be greeted with a pop-up window. This window will contain a text box. For each time you want, have each one on a new line, like so:

```
Saturday at 9pm
Saturday at 11pm
Sunday at 9pm
```
                ")
                .AddField("🎬 Movies", @"Movies work in a similar way to the timestamps. When you click the button, you will be greeted with the same type of pop-up window. Movies should be listed in the same way, one on each line, with the only difference being that the emoji you want the reaction to be should be at the start of each line. For example:

```
🧌 Shrek
👻 Ghostbusters
👽 Alien
```
                ")
                .AddField("✅ Submit", @"Note: This button will only work once both **Times** and **Movies** have been submitted.
This button will send the message to the server and ping @everyone. By default, polls will last for **12 hours**, but this can be shortened or increased if needed using the `/movie manage settime` command. They can also be immediately ended using `/movie manage end`.
                ")
                .Build()
            )
            .AddComponents(TimesButton, MoviesButton, SubmitButton);
        
        [SlashCommand("creator", "Receive the creation message.")]
        public async Task Creator(InteractionContext ctx)
        {
            await ctx.Member.SendMessageAsync(CreateMessage);
        }
    }
    
}