using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace MovieNightBot;

public static class MovieManager
{
    private static readonly string[] Numbers = 
    {
        "1️⃣", "2️⃣", "3️⃣", "4️⃣", "5️⃣", 
        "6️⃣", "7️⃣", "8️⃣", "9️⃣", "🔟"
    };
    
    private static readonly List<int> TimeList = new();
    private static readonly Dictionary<string, string> MovieList = new();
    
    public static void Times(ModalSubmitEventArgs args)
    {
        args.Values.TryGetValue("movie-times-response-times", out var times);
        string[] timeArray = times.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        TimeList.AddRange(timeArray.Select(time => (int) ParseDateString(time)));
    }

    public static void Movies(ModalSubmitEventArgs args)
    {
        args.Values.TryGetValue("movie-movies-response-movies", out var movies);
        string[] movieArray = movies.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var movie in movieArray)
        {
            MovieList.Add(movie[..2], movie[2..].Trim());
        }
    }
    
    public static async Task Submit(DiscordClient client)
    {
        var channel = await client.GetChannelAsync(Program.Options.ChannelId);
        var timesContent = Program.Options.TimesMessage + "\n\n" + string.Join(Environment.NewLine, TimeList.Select((time, index) => $"{Numbers[index]} **<t:{time}:F>**"));
        var moviesContent = Program.Options.MoviesMessage + "\n\n" + string.Join(Environment.NewLine, MovieList.Select(pair => $"{pair.Key} **{pair.Value}**"));

        var timesMessage = await client.SendMessageAsync(channel, timesContent);
        var moviesMessage = await client.SendMessageAsync(channel, moviesContent);

        for (int i = 0; i < TimeList.Count; i++)
        {
            await timesMessage.CreateReactionAsync(DiscordEmoji.FromUnicode(client, Numbers[i]));
        }
        
        foreach (var key in MovieList.Keys)
        {
            await moviesMessage.CreateReactionAsync(DiscordEmoji.FromUnicode(client, key));
        }
    }
    
    private static long ParseDateString(string input)
    {
        string[] parts = input.Split(new[] { " at " }, StringSplitOptions.RemoveEmptyEntries);
        string dayOfWeek = parts[0];
        string time = parts[1];

        DayOfWeek targetDayOfWeek = GetDayOfWeekFromString(dayOfWeek);
        DateTime now = DateTime.Now;

        DateTime targetDate = now.Date;
        int daysToAdd = (7 + (targetDayOfWeek - now.DayOfWeek)) % 7;
        targetDate = targetDate.AddDays(daysToAdd);

        int hour = int.Parse(time.Substring(0, time.Length - 2)) - 1;
        int minute = 0;

        if (time.EndsWith("pm", StringComparison.OrdinalIgnoreCase) && hour < 12)
        {
            hour += 12;
        }

        return new DateTime(targetDate.Year, targetDate.Month, targetDate.Day, hour, minute, 0).ToTimestamp();
    }

    private static DayOfWeek GetDayOfWeekFromString(string dayOfWeekString)
    {
        return Enum.GetValues(typeof(DayOfWeek))
            .Cast<DayOfWeek>()
            .FirstOrDefault(dow => dayOfWeekString.StartsWith(dow.ToString(), StringComparison.OrdinalIgnoreCase));
    }
}