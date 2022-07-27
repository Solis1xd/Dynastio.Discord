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
    [RateLimit(20, 1, RateLimit.RateLimitType.User)]
    [Group("personal", "your personal details")]
    public class PersonalChest : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        public GraphicService GraphicService { get; set; }
        public DynastioClient Dynastio { get; set; }

        [RequireUserDynastioAccount]
        [SlashCommand("chest", "your private chest")]
        public async Task personalChest(
            bool All = false, [Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "",
            bool Cache = true,
            DynastioProviderType provider = DynastioProviderType.Main)
        {
            var dynastioProvider =Dynastio[provider];
            if (!All || Context.BotUser.Accounts.Count < 2)
            {
                await _personalChest(account, Cache, dynastioProvider);
                return;
            }

            await DeferAsync();
            var cache = Context.BotUser.AllChests;
            if (!Cache) cache = new();
            if (cache.IsAllowedToUploadNew())
            {
                var chests = await Context.BotUser.Accounts.GetPersonalchests(dynastioProvider);

                var image = GraphicService.GetPersonalChests(chests.ToArray());

                var msg = await FollowupWithFileAsync(image, "chest.jpeg");
                Context.BotUser.AllChests = new ImageCacheUrl(msg.Attachments.First().Url, 3600);
                return;
            }
           
            await FollowupAsync(embed: $"This Chest Is For {cache.UploadedAt.ToDiscordUnixTimestampFormat()}".ToEmbed("", null, cache.Url));
        }
        private async Task _personalChest(string account = "", bool Cache = true, IDynastioProvider provider=null)
        {
            await DeferAsync();
            UserAccount account_ = string.IsNullOrWhiteSpace(account) 
                ? Context.BotUser.GetAccount() 
                : Context.BotUser.GetAccount(int.Parse(account));

            if (!Cache) account_.Chest = new();

            if (account_.Chest.IsAllowedToUploadNew())
            {
                var chest = await provider.GetUserPersonalchestAsync(account_.Id).TryGet();
                if (chest == null)
                {
                    await FollowupAsync("chest not found, join the game and put something to your chest.");
                    return;
                }
                var image = GraphicService.GetPersonalChest(chest);

                var msg = await FollowupWithFileAsync(image, "chest.jpeg");
                account_.Chest = new ImageCacheUrl(msg.Attachments.First().Url, 3600);
            }
            else
            {
                await FollowupAsync(embed: $"This Chest Is For {account_.Chest.UploadedAt.ToDiscordUnixTimestampFormat()}".ToEmbed("", null, account_.Chest.Url));
            }
        }
    }
}
