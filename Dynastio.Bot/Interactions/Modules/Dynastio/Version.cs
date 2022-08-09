using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Dynastio.Net;
using Discord.WebSocket;

namespace Dynastio.Bot.Interactions.Modules.Dynastio
{

    [RequireContext(ContextType.Guild)]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    [RateLimit(60, 1, RateLimit.RateLimitType.User)]
    public class Version : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        public DynastioClient Dynastio { get; set; }

        [SlashCommand("version", "dynast.io version")]
        public async Task version(DynastioProviderType provider = DynastioProviderType.Main)
        {
            await DeferAsync();
            var version = Dynastio[provider].GetCurrentVersionAsync();
            var message = await FollowupAsync(embed: $"Dynastio Current Version {provider} Is {version}".ToEmbed("Dynastio Version"));
        }
    }
}
