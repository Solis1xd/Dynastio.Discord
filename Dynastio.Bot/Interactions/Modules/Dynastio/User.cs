using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Dynastio.Net;
using Discord.WebSocket;
using Dynastio.Data;
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
            string dynastId = "",
            [Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "",
                DynastioProviderType provider = DynastioProviderType.Main)
        {
            await DeferAsync();

            if (user == null && dynastId == "")
            {
                await FollowupAsync("Id or User can't be null.");
                return;
            }

            var dynastioProvider = Dynastio[provider];

            UserAccount selectedAccount;
            if (dynastId == "")
            {
                var botUser = await UserService.GetUserAsync(user.Id);
                selectedAccount = string.IsNullOrWhiteSpace(account)
                ? botUser.GetAccount()
                : botUser.GetAccount(int.Parse(account));
            }
            else
            {
                selectedAccount = new UserAccount()
                {
                    Id = dynastId,
                };
            }

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
            IGuildUser user = null,
           string dynastId = "",
            bool All = false,
            [Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "",
                DynastioProviderType provider = DynastioProviderType.Main)
        {
            await DeferAsync();

            if (user == null && dynastId == "")
            {
                await FollowupAsync("Id or User can't be null.");
                return;
            }

            var dynastioProvider = Dynastio[provider];

            if (All)
            {
                var botUser = await UserService.GetUserAsync(user.Id);
                var chests = await botUser.Accounts.GetPersonalchests(dynastioProvider);
                if(chests == null || chests.Count < 1)
                {
                    await FollowupAsync("chest not found.");
                    return;
                }
                var image = GraphicService.GetPersonalChests(chests.ToArray());
                await FollowupWithFileAsync(image, "chest.jpeg");
            }
            else
            {
                UserAccount selectedAccount;
                if (dynastId == "")
                {
                    var botUser = await UserService.GetUserAsync(user.Id);
                    selectedAccount = string.IsNullOrWhiteSpace(account)
                    ? botUser.GetAccount()
                    : botUser.GetAccount(int.Parse(account));
                }
                else
                {
                    selectedAccount = new UserAccount()
                    {
                        Id = dynastId,
                    };
                }

                var chest = await dynastioProvider.GetUserPersonalchestAsync(selectedAccount.Id).TryGet();
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
           IGuildUser user = null,
           string dynastId = "",
           [Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "",
           DynastioProviderType provider = DynastioProviderType.Main)
        {
            await DeferAsync();

            if (user == null && dynastId == "")
            {
                await FollowupAsync("Id or User can't be null.");
                return;
            }

            UserAccount selectedAccount;
            if (dynastId == "")
            {
                var botUser = await UserService.GetUserAsync(user.Id);
                selectedAccount = string.IsNullOrWhiteSpace(account)
                ? botUser.GetAccount()
                : botUser.GetAccount(int.Parse(account));
            }
            else
            {
                selectedAccount = new UserAccount()
                {
                    Id = dynastId,
                };
            }

            string type = stat == StatType.Craft || stat == StatType.Gather || stat == StatType.Shop ? "item" : "entity";
            string property = stat.ToString().ToLower();


            var dynastioProvider = Dynastio[provider];
            var stat_ = await dynastioProvider.GetUserStatAsync(selectedAccount.Id).TryGet();
            if (stat_ is null)
            {
                await FollowupAsync(Context.User.Id.ToUserMention(), embed: this["data.not_found.description"].ToEmbed(this["data.not_found.title"]));
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

            await FollowupWithFileAsync(image, "stat.jpeg", Context.User.Id.ToUserMention());
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
