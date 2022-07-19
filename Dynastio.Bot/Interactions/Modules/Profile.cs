using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Dynastio.Net;
using Discord.WebSocket;

namespace Dynastio.Bot.Interactions.SlashCommands
{
    [RequireBotPermission(ChannelPermission.AttachFiles)]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    [RequireBotPermission(ChannelPermission.EmbedLinks)]
    [RequireContext(ContextType.Guild)]
    [RateLimit(70, 2, RateLimit.RateLimitType.User)]
    public class ProfileSlashCommand : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        public GraphicService GraphicService { get; set; }

        [RequireAccount]
        [SlashCommand("profile", "your profile")]
        public async Task profile([Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "",bool Cache = true)
        {
            await DeferAsync();
            UserAccount account_ = string.IsNullOrWhiteSpace(account) ? Context.BotUser.GetAccount() : Context.BotUser.GetAccountByHashCode(int.Parse(account));
            if (!Cache) account_.Profile = new();

            if (account_.Profile.IsAllowedToUploadNew())
            {
                var profile = await Context.Dynastio.Database.GetUserProfileAsync(account_.Id);
                var image = GraphicService.GetProfile(profile);

                var msg = await FollowupWithFileAsync(image, "profile.jpeg");
                account_.Profile = new ImageCacheUrl(msg.Attachments.First().Url, 3600);
            }
            else
            {
                await FollowupAsync(embed: $"This Profile Is For {account_.Profile.UploadedAt.ToDiscordUnixTimestampFormat()}".ToEmbed("", null, account_.Profile.Url));
            }
        }


    }
}
