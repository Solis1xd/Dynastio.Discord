using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Dynastio.Net;
using Discord.WebSocket;
using System.Net;
using SixLabors.ImageSharp;

namespace Dynastio.Bot.Interactions.SlashCommands
{
    [RequireBotPermission(ChannelPermission.AttachFiles)]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    [RequireContext(ContextType.Guild)]
    [RateLimit(240, 2, RateLimit.RateLimitType.User)]
    public class Rank : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        public GraphicService GraphicService { get; set; }

        [RequireAccount]
        [RateLimit(60, 2, RateLimit.RateLimitType.User)]
        [SlashCommand("rank", "your dynast.io rank")]
        public async Task rank([Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "",bool Cache = true)
        {
            await DeferAsync();
            UserAccount account_ = string.IsNullOrWhiteSpace(account) ? Context.BotUser.GetAccount() : Context.BotUser.GetAccount(int.Parse(account));
            if (!Cache) account_.Rank = new();

            if (account_.Rank.IsAllowedToUploadNew())
            {
                var rank = await Context.Dynastio.Database.GetUserRanAsync(account_.Id).TryGet();
                if (rank == null)
                {
                    await FollowupAsync(embed: this["data.not_found.description"].ToEmbed(this["data.not_found.title"]));
                    return;
                }
                IUser user = Context.User;
                using (HttpClient httpClient = new())
                {
                    byte[] data = await httpClient.GetByteArrayAsync(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl());

                    using (MemoryStream mem = new MemoryStream(data))
                    {
                        using (var Avatar = SixLabors.ImageSharp.Image.Load(mem))
                        {
                            var img = GraphicService.GetRank(Avatar, rank);
                            using (var stream = new System.IO.MemoryStream())
                            {
                                img.SaveAsJpeg(stream);
                                img.Dispose();
                                var msg = await FollowupWithFileAsync(stream, "img.jpeg", Context.BotUser.Id.ToUserMention());
                                account_.Rank = new ImageCacheUrl(msg.Attachments.First().Url, 1600);

                            }
                        }
                    }

                }
            }
            else
            {
                await FollowupAsync(embed: $"This Rank Is For {account_.Rank.UploadedAt.ToDiscordUnixTimestampFormat()}".ToEmbed("", null, account_.Rank.Url));

            }
        }

    }
}
