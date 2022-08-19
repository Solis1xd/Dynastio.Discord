﻿using Discord.Interactions;
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
    [EnabledInDm(false)]
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
            bool CountItems = false,
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

                string content = "";

                if (CountItems)
                {
                    Dictionary<string, int> items = new();
                    foreach (var chest in chests)
                    {
                        foreach (var item in chest.Items)
                        {
                            var count = item.Count;
                            if (items.TryGetValue(item.ItemType.ToString(), out int v))
                            {
                                items[item.ItemType.ToString()] = v + count;
                            }
                            else
                            {
                                items.Add(item.ItemType.ToString(), count);
                            }
                        }
                    }
                    content = items.OrderByDescending(a=>a.Value).ToStringTable(new string[] { "Item", "Count" },
                        a => a.Key,
                        a => "x" + a.Value);
                }

                await FollowupWithFileAsync(image, "chest.jpeg", embed: content.ToMarkdown().ToEmbed(imageUrl: "attachment://chest.jpeg"));
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
