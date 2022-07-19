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
    [RequireGuild(RequireGuild.LocalGuildId.Main)]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    [RequireContext(ContextType.Guild)]
    [Group("clan", "clan commands")]
    internal class PromoteClanModule : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        [SlashCommand("promote", "promote your clan")]
        public async Task promote()
        {
            await RespondWithModalAsync<PromoteClanForm>("clan submit");
        }


    }
}
