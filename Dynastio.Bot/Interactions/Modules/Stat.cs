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
    [RequireContext(ContextType.Guild)]
    public class Stat : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        public GraphicService GraphicService { get; set; }
        public DynastioClient Dynastio { get; set; }

        [RequireUserDynastioAccount]
        [SlashCommand("stat", "stat")]
        [RateLimit(60, 2, RateLimit.RateLimitType.User)]
        public async Task stat(
            StatType stat, [Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "",
            bool Cache = true,
            DynastioProviderType provider = DynastioProviderType.Main)
        {
            await DeferAsync();

            UserAccount account_ = string.IsNullOrWhiteSpace(account) ? Context.BotUser.GetAccount() : Context.BotUser.GetAccount(int.Parse(account));
            string type = (stat == StatType.Craft || stat == StatType.Gather || stat == StatType.Shop) ? "item" : "entity";
            string property = stat.ToString().ToLower();

            var result = account_.Stat.TryGetValue(stat, out ImageCacheUrl imageCacheUrl);
            if (!Cache) imageCacheUrl = new();
            if (result is false || result && imageCacheUrl.IsAllowedToUploadNew())
            {
                var dynastioProvider =Dynastio[provider];
                var stat_ = await dynastioProvider.GetUserStatAsync(account_.Id).TryGet();
                if (stat_ is null)
                {
                    await FollowupAsync(Context.User.Id.ToUserMention(), embed: this["data.not_found.description"].ToEmbed(this["data.not_found.title"]));
                    return;
                }
                var image = type switch
                {
                    "item" => GraphicService.GetStat<ItemType>(property switch
                    {
                        "craft" => stat_.Craft,
                        "gather" => stat_.Gather,
                        "shop" => stat_.Shop,
                        _ => null
                    }),
                    "entity" => GraphicService.GetStat<EntityType>(property switch
                    {
                        "build" => stat_.Build,
                        "kill" => stat_.Kill,
                        "death" => stat_.Death,
                        _ => null
                    }),
                    _ => null
                };

                var msg = await FollowupWithFileAsync(image, "stat.jpeg", Context.User.Id.ToUserMention());
                var cache = new ImageCacheUrl(msg.Attachments.First().Url, 3600);
                if (!result)
                {
                    account_.Stat.Add(stat, cache);
                }
                else
                {
                    account_.Stat[stat] = cache;
                }
            }
            else
            {
                await FollowupAsync(embed: $"This Stat Is For {account_.Stat[stat].UploadedAt.ToDiscordUnixTimestampFormat()}".ToEmbed("", null, account_.Stat[stat].Url));
            }

        }


        public enum StatType
        {
            Craft,
            Gather,
            Shop,
            Build,
            Kill,
            Death,
        }
    }
}
