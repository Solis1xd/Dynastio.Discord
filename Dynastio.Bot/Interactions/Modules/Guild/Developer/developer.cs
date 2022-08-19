using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Dynastio.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynastio.Data;

namespace Dynastio.Bot.Interactions.Modules.Dynastio
{
    [RequireUser(RequireUserAttribute.RequireUserType.BotOwner)]
    [RequireContext(ContextType.Guild)]
    [EnabledInDm(false)]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    [Group("developer", "bot developer commands")]
    public class DeveloperModule : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        [Group("youtube", "users commands")]
        public class YoutubeModule : CustomInteractionModuleBase<CustomSocketInteractionContext>
        {
            public YoutubeService youtubeService { get; set; }

            [SlashCommand("videos", "send all youtube videos of a channel")]
            public async Task sendAllYoutubeVideos(string channelId = "", YoutubeVideoOrderByType orderby = YoutubeVideoOrderByType.OrderByDescending, int take = 1, int skip = 0)
            {
                await DeferAsync();

                var videos = channelId.IsNullOrEmpty()
                    ? youtubeService.Videos
                    : await youtubeService.GetAllChannelVideos(channelId).TryGet();

                if (videos == null)
                {
                    await FollowupAsync(embed: $"no video found.".ToEmbed());
                    return;
                }

                videos = orderby == YoutubeVideoOrderByType.OrderBy
                    ? videos.OrderBy(a => a.Snippet.PublishedAt).ToList()
                    : videos.OrderByDescending(a => a.Snippet.PublishedAt).ToList();

                videos = videos.Skip(skip).Take(take).ToList();

                if (videos == null)
                {
                    await FollowupAsync(embed: $"no video found after the filters.".ToEmbed());
                    return;
                }

                foreach (var v in videos)
                {
                    await FollowupAsync(v.Id.ToYoutubeVideoUrl());
                    await Task.Delay(1000);
                }

                var msg = await FollowupAsync(embed: $"{videos.Count} videos uploaded to the channel. `this message will be delete after 5 seconds.`".ToEmbed());
                await Task.Delay(15000);
                await msg.DeleteAsync();

            }
            public enum YoutubeVideoOrderByType
            {
                OrderBy,
                OrderByDescending,
            }
        }


        [Group("users", "users commands")]
        public class UsersModule : CustomInteractionModuleBase<CustomSocketInteractionContext>
        {
            public UserService userManager { get; set; }

            [SlashCommand("clear-cache", "clear cached users")]
            public async Task clearCache()
            {
                await DeferAsync();
                var usersCount = userManager.GetCacheCount();
                userManager.ClearCache();
                await FollowupAsync(embed: $"{usersCount} users removed from the cache".ToEmbed());
            }

            [SlashCommand("remove-account", "remove an account from the user")]
            public async Task removeAccount(IUser user, [Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "")
            {
                await DeferAsync();
                var buser = await userManager.GetUserAsync(user.Id);

                var selectedAccount = buser.GetAccount(int.Parse(account));
                if (selectedAccount is null)
                {
                    await FollowupAsync(embed: $"account not found.".ToEmbed());
                    return;
                }
                buser.RemoveAccount(selectedAccount);
                await buser.UpdateAsync();
                await FollowupAsync(embed: $"account removed from the user.".ToEmbed());
            }

            [SlashCommand("permissions", "ban a user from the bot")]
            public async Task userPermissiond(IUser user, UserBanType section, bool status = true)
            {
                await DeferAsync();
                var buser = await userManager.GetUserAsync(user.Id);

                if (section == UserBanType.Bot)
                    buser.IsBanned = status;
                else
                    buser.IsBannedToAddNewAccount = status;

                await buser.UpdateAsync();

                await FollowupAsync(embed: $"the {user.Id.ToUserMention()} permission to ` {section} ` is ` {(status ? "banned" : "available")} `.".ToEmbed());
            }
            public enum UserBanType { Bot, AddAccount }
        }

    }

}
