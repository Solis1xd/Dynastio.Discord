using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Dynastio.Net;
using Dynastio.Bot.Extensions.Discord;
using Discord.WebSocket;
using Newtonsoft.Json;


namespace Dynastio.Bot.Interactions.SlashCommands.Administrator
{
    [Group("server", "server commands")]
    [RequireContext(ContextType.Guild)]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    [RequireUserPermission(GuildPermission.Administrator)]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    public class Guild : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        [Group("setting", "server setting")]
        [RateLimit(30, 3, RateLimit.RateLimitType.Guild)]
        public class GuildSettings : CustomInteractionModuleBase<CustomSocketInteractionContext>
        {
            [SlashCommand("moderator", "moderate the server by dynast.io bot")]
            public async Task moderaotr(ModeraotrType status)
            {
                await DeferAsync();
                Context.BotGuild.IsModerationEnabled = status == ModeraotrType.Enable;

                await Context.BotGuild.UpdateAsync();
                await FollowupAsync(embed: $"Moderation is ` {status} ` for this server.".ToSuccessfulEmbed("Operation was successful"));
            }
            public enum ModeraotrType
            {
                Enable,
                Disable
            }
        }



    }
}
