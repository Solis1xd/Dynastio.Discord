using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynastio.Bot
{
    internal class SelectMenuUtilities
    {

        public static async Task<List<UserAccount>> GetUserAccounts(IInteractionContext Context, User user, int min = 1, int max = 1, IUserMessage Message = null, Action<SocketInteraction> action = null)
        {
            List<UserAccount> accounts = new();
            if (user.Accounts.Count > 1)
            {
                var components = new ComponentBuilder();
                components.WithSelectMenu($"{InteractionUtilities.Perfix}account", user.Accounts.ToSelectMenuOptionBuilder(), $"Pick {min} - {max} Accounts", min, max > user.Accounts.Count ? user.Accounts.Count : max);

                var embed = new EmbedBuilder()
                {
                    Title = "Account Selection",
                    Description = "Welcome to Dynast.io Account Selection .. !\n" +
                                             $"Select {min} - {max} account to show the content.\n" +
                                             $"\n**Closes {20.ToDiscordUnixTimeFromat()}**",
                    ThumbnailUrl = "https://cdn.discordapp.com/attachments/916998929609023509/967518502204370954/a.png",
                    Color = Color.LightOrange
                }.Build();

                var msg = await (Message ?? (Context.Interaction as SocketMessageComponent).Message).ModifyToEmbed(embed, components.Build());
                var result = await Context.ReadSelectMenuFromMessageIdAsync(msg.Id, TimeSpan.FromSeconds(20));

                if (action != null && result != null) action.Invoke(result);
                if (result != null && result.Data != null && result.Data.Values != null)
                {
                    foreach (var v in result.Data.Values)
                    {
                        if (v == null) continue;
                        var account = user.GetAccountByNickname(v.ToString());
                        if (account != null) accounts.Add(account);
                    }
                }
                else return null;
            }
            else { if (user.GetAccount() != null) accounts.Add(user.GetAccount()); }

            return accounts;
        }
    }
}
