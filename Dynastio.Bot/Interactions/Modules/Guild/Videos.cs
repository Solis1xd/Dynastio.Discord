using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Dynastio.Net;
using Discord.WebSocket;

namespace Dynastio.Bot.Interactions.Modules.Guild
{
    [RequireContext(ContextType.Guild)]
    [RequireBotPermission(ChannelPermission.SendMessages | ChannelPermission.EmbedLinks)]
    [Group("videos", "dynastio videos")]
    public class Videos : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        [RequireYoutubeService]
        [Group("dyanstio", "dynastio channel videos")]
        public class DynastioChannelModule : CustomInteractionModuleBase<CustomSocketInteractionContext>
        {
            public YoutubeService Youtube { get; set; }

            [RateLimit(5, 1)]
            [SlashCommand("search", "search for a video")]
            public async Task search([Autocomplete] string video)
            {
                await DeferAsync();
                var result = Youtube.Videos.FirstOrDefault(a => a.Id.VideoId == video);
                if (result == null)
                {
                    await FollowupAsync(embed: "No video found.".ToWarnEmbed("not found"));
                    return;
                }
                await FollowupAsync(result.Id.ToYoutubeVideoUrl());
                await FollowupAsync(
                    embed:
                    ($"**Title:** {result.Snippet.Title}\n" +
                     $"**Description:** {result.Snippet.Description}\n" +
                     $"**Link:** {result.Id.ToYoutubeVideoUrl()}\n" +
                    $"**PublishedAt:** {result.Snippet.PublishedAt.Value.ToDiscordUnixTimestampFormat()}\n" +
                    $"").ToEmbed(result.Snippet.Title, result.Snippet.Thumbnails.Default__.Url ?? ""));
            }

            [AutocompleteCommand("video", "search")]
            public async Task AutoCompleteSearch()
            {
                var value = (string)(Context.Interaction as SocketAutocompleteInteraction).Data.Current.Value;
                var videos = Youtube.Videos.Where(a => a.Snippet.Title.ToLower().Contains(value)).Take(25);
                List<AutocompleteResult> results = new();
                foreach (var video in videos)
                {
                    results.Add(new AutocompleteResult()
                    {
                        Name = video.Snippet.Title.TrySubstring(90),
                        Value = video.Id.VideoId
                    });
                }
                await (Context.Interaction as SocketAutocompleteInteraction).RespondAsync(results);
            }

            [RateLimit(5, 2)]
            [SlashCommand("random", "get a random video")]
            [ComponentInteraction("videos.dynastio.random", true)]
            public async Task random()
            {
                await DeferAsync();

                var videos = Youtube.Videos;
                if (videos == null || videos.Count == 0)
                {
                    await FollowupAsync(embed: "No video found.".ToWarnEmbed("not found"));
                    return;
                }
                var index = Program.Random.Next(videos.Count);
                var video = videos[index];

                var componenets = new ComponentBuilder();
                componenets.WithButton(this["next"], $"videos.dynastio.random", ButtonStyle.Success, new Emoji("⏩"), null, false, 0);

                var message = await FollowupAsync(video.Id.ToYoutubeVideoUrl(), components: componenets.Build());
            }
        }
        [Group("featured", "featured videos")]
        public class FeaturedVideosModule : CustomInteractionModuleBase<CustomSocketInteractionContext>
        {
            public DynastioClient Dynastio { get; set; }

            [RateLimit(5, 2)]
            [SlashCommand("random", "get a random video")]
            [ComponentInteraction("videos.featured.random:*:*", true)]
            public async Task random(LocalType local = LocalType.common, DynastioProviderType provider = DynastioProviderType.Main)
            {
                await DeferAsync();

                var videos = Dynastio[provider].FeaturedVideos.Where(a => a.Group == local.ToString()).ToList();
                if (videos == null || videos.Count == 0)
                {
                    await FollowupAsync(embed: "No video found.".ToWarnEmbed("not found"));
                    return;
                }
                var index = Program.Random.Next(videos.Count);
                var video = videos[index];

                var componenets = new ComponentBuilder();
                componenets.WithButton(this["next"], $"videos.featured.random:{local}:{provider}", ButtonStyle.Success, new Emoji("⏩"), null, false, 0);

                var message = await FollowupAsync(video.Url, components: componenets.Build());
            }
            public enum LocalType { ru, common }

            [RateLimit(5)]
            [SlashCommand("get", "get a list of videos")]
            public async Task get(int skip = 0, [MaxValue(50)] int take = 50, DynastioProviderType provider = DynastioProviderType.Main)
            {
                await DeferAsync();
                var videos = Dynastio[provider].FeaturedVideos.ToList();
                if (videos == null || videos.Count == 0)
                {
                    await FollowupAsync(embed: "No video found.".ToWarnEmbed("not found"));
                    return;
                }
                var content = $"**Totall Featured Videos**: {videos.Count}\n";
                videos = videos.Skip(skip).Take(take).ToList();
                videos.ForEach(a =>
                {
                    content +=
                    $"Url: {a.Url}\n" +
                    $"ExpireAt: {a.ExpireAt.ToDiscordUnixTimestampFormat()}\n\n";
                });
                await FollowupAsync(embed: content.ToEmbed("Videos"));
            }

        }
    }
}
