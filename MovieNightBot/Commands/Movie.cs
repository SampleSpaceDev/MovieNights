using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace MovieNightBot.Commands;

[SlashCommandGroup("movie", "Movie commands.")]
public class Movie : ApplicationCommandModule
{

    [SlashCommandGroup("manage", "Manage the movie nights.")]
    [SlashRequireOwner]
    public class Manage : ApplicationCommandModule
    {
        private static readonly DiscordButtonComponent TimesButton = new(ButtonStyle.Secondary, "movie-times", "🕑 Times");
        private static readonly DiscordButtonComponent MoviesButton = new(ButtonStyle.Secondary, "movie-movies", "🎬 Movies");
        private static readonly DiscordButtonComponent SubmitButton = new(ButtonStyle.Success, "movie-submit", "✅ Submit");
        
        private static readonly DiscordMessageBuilder CreateMessage = new DiscordMessageBuilder()
            .AddEmbed(new DiscordEmbedBuilder()
                .WithAuthor("Movie Night Creator")
                .WithDescription("You can create a movie night by using the buttons below. Each one is explained here:")
                .AddField("🕑 Times", @"To make it easier for you, the times do not need to be given as timestamps. Instead, you can simply say something like `Saturday at 9pm` and the bot will do the rest for you. When you click the button, you will be greeted with a pop-up window. This window will contain a text box. For each time you want, have each one on a new line, like so:

```
Saturday at 9pm
Saturday at 11pm
Sunday at 9pm
```
⠀
                ")
                .AddField("🎬 Movies", @"Movies work in a similar way to the timestamps. When you click the button, you will be greeted with the same type of pop-up window. Movies should be listed in the same way, one on each line, with the only difference being that the emoji you want the reaction to be should be at the start of each line. To easily access emojis, you can press the Windows key and the `.` key on your keyboard at the same time. For example:

```
🧌 Shrek
👻 Ghostbusters
👽 Alien
```
⠀
                ")
                .AddField("✅ Submit", @"*Note: This button will only work once both **Times** and **Movies** have been submitted.*
This button will send the message to the server and ping @everyone. By default, polls will last for **12 hours**, but this can be shortened or increased if needed using the `/movie manage settime` command. They can also be immediately ended using `/movie manage end`.
                ")
                .WithColor(new DiscordColor("#e74c3c"))
                .Build()
            )
            .AddComponents(TimesButton, MoviesButton, SubmitButton);
        
        [SlashCommand("creator", "Receive the creation message.")]
        public async Task Creator(InteractionContext ctx)
        {
            var message = await ctx.Member.SendMessageAsync(CreateMessage);
            
            if (Program.Options.CreatorMessageId != 0)
            {
                var oldMessage = await message.Channel.GetMessageAsync(Program.Options.CreatorMessageId);
                await oldMessage.DeleteAsync();
            }

            Program.Options.CreatorMessageId = message.Id;
            await Config.Save(Program.Options);
            
            await ctx.CreateResponseAsync("✅ You were sent the creator message!", true);
        }
    }
    
}