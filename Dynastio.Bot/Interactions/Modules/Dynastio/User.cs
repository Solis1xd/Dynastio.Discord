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
            IGuildUser user ,
            [Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "",
                DynastioProviderType provider = DynastioProviderType.Main)
        {
            await DeferAsync();

            var dynastioProvider = Dynastio[provider];

            UserAccount selectedAccount;
           
                var botUser = await UserService.GetUserAsync(user.Id);
                selectedAccount = string.IsNullOrWhiteSpace(account)
                ? botUser.GetAccount()
                : botUser.GetAccount(int.Parse(account));
         

            var profile = await dynastioProvider.GetUserProfileAsync(selectedAccount.Id).TryGet();
            if (profile == null)
            {
                await FollowupAsync("data not found.");
                return;
            }

            // for making it harder to add the account of this user
            profile.Coins += Program.Random.Next(-150, 50);
            if (profile.Coins < 0)
                profile.Coins = 0;

            var image = GraphicService.GetProfile(profile);

            await FollowupWithFileAsync(image, "profile.jpeg");
        }
        [RateLimit(70, 3, RateLimit.RateLimitType.User)]
        [SlashCommand("chest", "your dynastio chest")]
        public async Task chest(
            IGuildUser user,
            bool All = false,
            [Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "",
                DynastioProviderType provider = DynastioProviderType.Main)
        {
            await DeferAsync();

           

            var dynastioProvider = Dynastio[provider];

            if (All)
            {
                var botUser = await UserService.GetUserAsync(user.Id, false);
                if(botUser == null)
                {
                    await FollowupAsync("user not found.");
                    return;
                }
                var chests = await botUser.Accounts.GetPersonalchests(dynastioProvider);
                if (chests == null || chests.Count < 1)
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
              
                    var botUser = await UserService.GetUserAsync(user.Id);

                    selectedAccount = string.IsNullOrWhiteSpace(account)
                    ? botUser.GetAccount()
                    : botUser.GetAccount(int.Parse(account));
               

                if (selectedAccount == null)
                {
                    await FollowupAsync("user not found.");
                    return;
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
    }
}
