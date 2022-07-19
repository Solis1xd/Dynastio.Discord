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
    [RequireContext(ContextType.Guild)]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    [RateLimit(1200, 2, RateLimit.RateLimitType.User)]
    public class Provider : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {

        [SlashCommand("provider", "dynast.io provider setting")]
        public async Task provider(ProviderType @switch)
        {
            await DeferAsync();

            Context.BotUser.Settings.DynastioProvider = @switch.ToString().ToLower();

            await Context.BotUser.UpdateAsync();

            var embed = new EmbedBuilder()
            {
                Title = this["menu.provider.embed.title"],
                Description =
                this["menu.provider.embed.description:*", Context.BotUser.Settings.DynastioProvider] + "\n\n" +
                this["closes:*", 2.ToDiscordUnixTimeFromat()]
            }.Build();
            await FollowupAsync(Context.BotUser.Id.ToUserMention(), embed: embed);
        }
        public enum ProviderType
        {
            Main,
            Nightly
        }
    }
}
