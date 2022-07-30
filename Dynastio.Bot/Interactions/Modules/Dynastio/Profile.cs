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
    [RequireUserDynastioAccount]
    [RequireContext(ContextType.Guild)]
    [RequireBotPermission(ChannelPermission.AttachFiles | ChannelPermission.SendMessages | ChannelPermission.EmbedLinks)]
    [Group("profile", "dynastio profile")]
    public class ProfileSlashCommand : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        public GraphicService GraphicService { get; set; }
        public DynastioClient Dynastio { get; set; }

        [RateLimit(70, 2, RateLimit.RateLimitType.User)]
        [SlashCommand("picture", "your dynastio profile")]
        public async Task profile([Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "",
                bool Cache = true,
                DynastioProviderType provider = DynastioProviderType.Main)
        {
            await DeferAsync();
            var dynastioProvider = Dynastio[provider];

            UserAccount selectedAccount = string.IsNullOrWhiteSpace(account)
                ? Context.BotUser.GetAccount()
                : Context.BotUser.GetAccount(int.Parse(account));

            var profile = await dynastioProvider.GetUserProfileAsync(selectedAccount.Id).TryGet();
            if (profile == null)
            {
                await FollowupAsync("data not found.");
                return;
            }
            var image = GraphicService.GetProfile(profile);
            var msg = await FollowupWithFileAsync(image, "profile.jpeg");
        }

        [RateLimit(150, 2)]
        [SlashCommand("details", "your dynastio profile details")]
        public async Task details([Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "",
           DynastioProviderType provider = DynastioProviderType.Main)
        {
            await DeferAsync();
            var dynastioProvider = Dynastio[provider];

            UserAccount selectedAccount = string.IsNullOrWhiteSpace(account)
                ? Context.BotUser.GetAccount()
                : Context.BotUser.GetAccount(int.Parse(account));

            var profile = await dynastioProvider.GetUserProfileAsync(selectedAccount.Id);
            string content =
                $"Level: {profile.Details.Level}\n" +
                $"Experience: {profile.Details.Experience}\n" +
                $"Required Experience For New Level: {profile.GetRequireExperienceForNewLevel()}\n" +
                $"Latest Server: {profile.LatestServer}\n" +
                $"Latest Activity: {profile.LastActiveAt.ToDiscordUnixTimestampFormat()}\n" +
                $"Coins: {profile.Coins}\n" +
                $"Unlocked Buildings Count: {profile.UnlockedBuildings?.Count ?? 0}\n" +
                $"Unlocked Recipes Count: {profile.UnlockedRecipes?.Count ?? 0}\n" +
                $"Unlocked Skins Count: {profile.UnlockedSkins?.Count ?? 0}\n" +
                $"Badges: {string.Join(", ", profile.Badges ?? new())}\n" +
                $"";
            await FollowupAsync(embed: content.ToEmbed($"Profile {selectedAccount.Nickname}", Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl()));
        }
        [Group("unlocked", "dynastio profile")]
        [RateLimit(150, 4)]
        public class UnlockedModule : CustomInteractionModuleBase<CustomSocketInteractionContext>
        {
            public DynastioClient Dynastio { get; set; }

            [SlashCommand("skins", "unlocked skins")]
            public async Task skins([Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "",
                       DynastioProviderType provider = DynastioProviderType.Main)
            {
                await DeferAsync();
                var dynastioProvider = Dynastio[provider];

                UserAccount selectedAccount = string.IsNullOrWhiteSpace(account)
                    ? Context.BotUser.GetAccount()
                    : Context.BotUser.GetAccount(int.Parse(account));

                var profile = await dynastioProvider.GetUserProfileAsync(selectedAccount.Id);
                var content = profile.UnlockedSkins?.ToStringTable(new string[] { "#", "Unlocked Skins" },
                    a => profile.UnlockedSkins.IndexOf(a).ToRegularCounter(),
                    a => a.ToString()) ?? "no any unlocked skin found.";
                await FollowupAsync(embed: content.ToMarkdown().ToEmbed($"Profile {selectedAccount.Nickname}", Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl()));
            }
            [SlashCommand("buildings", "unlocked buildings")]
            public async Task buildings([Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "",
                       DynastioProviderType provider = DynastioProviderType.Main)
            {
                await DeferAsync();
                var dynastioProvider = Dynastio[provider];

                UserAccount selectedAccount = string.IsNullOrWhiteSpace(account)
                    ? Context.BotUser.GetAccount()
                    : Context.BotUser.GetAccount(int.Parse(account));

                var profile = await dynastioProvider.GetUserProfileAsync(selectedAccount.Id);
                var content = profile.UnlockedBuildings?.ToStringTable(new string[] { "#", "Unlocked Buildings" },
                    a => profile.UnlockedBuildings.IndexOf(a).ToRegularCounter(),
                    a => a.ToString()) ?? "no any unlocked building found.";
                await FollowupAsync(embed: content.ToMarkdown().ToEmbed($"Profile {selectedAccount.Nickname}", Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl()));
            }
            [SlashCommand("recipes", "unlocked recipes")]
            public async Task recipes([Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "",
                       DynastioProviderType provider = DynastioProviderType.Main)
            {
                await DeferAsync();
                var dynastioProvider = Dynastio[provider];

                UserAccount selectedAccount = string.IsNullOrWhiteSpace(account)
                    ? Context.BotUser.GetAccount()
                    : Context.BotUser.GetAccount(int.Parse(account));

                var profile = await dynastioProvider.GetUserProfileAsync(selectedAccount.Id);
                var content = profile.UnlockedRecipes?.ToStringTable(new string[] { "#", "Unlocked Recipes" },
                    a => profile.UnlockedRecipes.IndexOf(a).ToRegularCounter(),
                    a => a.ToString()) ?? "no any unlocked recipes.";
                await FollowupAsync(embed: content.ToMarkdown().ToEmbed($"Profile {selectedAccount.Nickname}", Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl()));
            }
        }

    }
}
