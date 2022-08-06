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
    [RequireBotPermission(ChannelPermission.SendMessages | ChannelPermission.AttachFiles)]
    [RequireContext(ContextType.Guild)]
    [RateLimit(20, 1, RateLimit.RateLimitType.User)]
    [Group("personal", "your personal details")]
    public class PersonalChest : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        public GraphicService GraphicService { get; set; }
        public DynastioClient Dynastio { get; set; }

        [RequireUserDynastioAccount]
        [SlashCommand("chest", "your private chest")]
        public async Task personalChest(
            bool All = false,
            [Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "",
            DynastioProviderType provider = DynastioProviderType.Main
            )
        {
            await DeferAsync();
            var dynastioProvider = Dynastio[provider];

            if (All && Context.BotUser.Accounts.Count > 1)
            {
                var chests = await Context.BotUser.Accounts.GetPersonalchests(dynastioProvider);
                if (chests == null || chests.Count == 0)
                {
                    await FollowupAsync("chest not found.");
                    return;
                }
                var image = GraphicService.GetPersonalChests(chests.ToArray());
                await FollowupWithFileAsync(image, "chest.jpeg");
            }
            else
            {
                UserAccount account_ = string.IsNullOrWhiteSpace(account)
                        ? Context.BotUser.GetAccount()
                        : Context.BotUser.GetAccount(int.Parse(account));
                if (account_ is null)
                {
                    await FollowupAsync("account not found.");
                    return;
                }
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

    }
}
