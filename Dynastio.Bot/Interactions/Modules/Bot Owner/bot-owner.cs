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
    [RequireBotPermission(ChannelPermission.SendMessages)]
    [Group("bot-owner", "bot owner commands")]
    public class BotOwnerModule : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        YoutubeService youtubeService { get; set; }

        [SlashCommand("send-all-youtube-videos", "send all youtube videos")]
        public async Task sendAllYoutubeVideos()
        {
            await DeferAsync();
            foreach (var v in youtubeService.Videos)
            {
                await FollowupAsync($"{v.Id.ToYoutubeVideoUrl()}");
                await Task.Delay(1000);
            }
        }

        [Group("bot-users", "users commands")]
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
