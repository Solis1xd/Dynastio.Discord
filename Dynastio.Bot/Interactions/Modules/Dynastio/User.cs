using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Dynastio.Net;
using Discord.WebSocket;

namespace Dynastio.Bot.Interactions.Modules.Dynastio
{

    [RequireContext(ContextType.Guild)]
    [RequireBotPermission(ChannelPermission.SendMessages | ChannelPermission.AttachFiles | ChannelPermission.EmbedLinks)]
    [Group("user", "user")]
    public class UserModule : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        public GraphicService GraphicService { get; set; }
        public DynastioClient Dynastio { get; set; }
        public UserService UserService { get; set; }

        [RateLimit(70, 4, RateLimit.RateLimitType.User)]
        [SlashCommand("profile", "your dynastio profile")]
        public async Task profile(
            IGuildUser user = null,
            ulong userId = 0,
            [Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "",
                DynastioProviderType provider = DynastioProviderType.Main)
        {
            await DeferAsync();

            if (user == null && userId == 0)
            {
                await FollowupAsync("Id or User can't be null.");
                return;
            }

            var dynastioProvider = Dynastio[provider];

            var botUser = await UserService.GetUserAsync(user?.Id ?? userId);

            UserAccount selectedAccount = string.IsNullOrWhiteSpace(account)
                ? botUser.GetAccount()
                : botUser.GetAccount(int.Parse(account));

            var profile = await dynastioProvider.GetUserProfileAsync(selectedAccount.Id).TryGet();
            if (profile == null)
            {
                await FollowupAsync("data not found.");
                return;
            }

           var image = GraphicService.GetProfile(profile);

           await FollowupWithFileAsync(image, "profile.jpeg");
        }
        [RateLimit(70, 3, RateLimit.RateLimitType.User)]
        [SlashCommand("chest", "your dynastio chest")]
        public async Task chest(
            IGuildUser user=null,
            ulong userId = 0,
            bool All = false,
            [Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "",
                DynastioProviderType provider = DynastioProviderType.Main)
        {
            await DeferAsync();

            if (user == null && userId == 0)
            {
                await FollowupAsync("Id or User can't be null.");
                return;
            }

            var botUser = await UserService.GetUserAsync(user?.Id ?? userId);

            var dynastioProvider = Dynastio[provider];

            if (All)
            {
                var chests = await botUser.Accounts.GetPersonalchests(dynastioProvider);
                var image = GraphicService.GetPersonalChests(chests.ToArray());
                await FollowupWithFileAsync(image, "chest.jpeg");
            }
            else
            {
                UserAccount account_ = string.IsNullOrWhiteSpace(account)
                        ? botUser.GetAccount()
                        : botUser.GetAccount(int.Parse(account));

                var chest = await dynastioProvider.GetUserPersonalchestAsync(account_.Id).TryGet();
                if (chest == null)
                {
                    await FollowupAsync("chest not found, join the game and put something to your chest.");
                    return;
                }
                var image = GraphicService.GetPersonalChest(chest);

                await FollowupWithFileAsync(image, "chest.jpeg");
            }

        }

        [SlashCommand("stat", "stat")]
        [RateLimit(70, 2, RateLimit.RateLimitType.User)]
        public async Task stat(
           StatType stat,
           IGuildUser user = null, ulong userId = 0,
           [Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "",
           DynastioProviderType provider = DynastioProviderType.Main)
        {
            await DeferAsync();

            if (user == null && userId == 0)
            {
                await FollowupAsync("Id or User can't be null.");
                return;
            }

            var botUser = await UserService.GetUserAsync(user?.Id ?? userId);

            UserAccount account_ = string.IsNullOrWhiteSpace(account) ? botUser.GetAccount() : botUser.GetAccount(int.Parse(account));
            string type = stat == StatType.Craft || stat == StatType.Gather || stat == StatType.Shop ? "item" : "entity";
            string property = stat.ToString().ToLower();


            var dynastioProvider = Dynastio[provider];
            var stat_ = await dynastioProvider.GetUserStatAsync(account_.Id).TryGet();
            if (stat_ is null)
            {
                await FollowupAsync(botUser.Id.ToUserMention(), embed: this["data.not_found.description"].ToEmbed(this["data.not_found.title"]));
                return;
            }
            var image = type switch
            {
                "item" => GraphicService.GetStat(property switch
                {
                    "craft" => stat_.Craft,
                    "gather" => stat_.Gather,
                    "shop" => stat_.Shop,
                    _ => null
                }),
                "entity" => GraphicService.GetStat(property switch
                {
                    "build" => stat_.Build,
                    "kill" => stat_.Kill,
                    "death" => stat_.Death,
                    _ => null
                }),
                _ => null
            };

            await FollowupWithFileAsync(image, "stat.jpeg", botUser.Id.ToUserMention());
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
