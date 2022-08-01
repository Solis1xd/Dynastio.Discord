using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Dynastio.Net;
using Discord.WebSocket;

namespace Dynastio.Bot.Interactions.Modules
{

    [RequireContext(ContextType.Guild)]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    [RequireOwner]
    public class Test : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        [SlashCommand("test", "test something")]
        public async Task test(IAttachment attachment, IAttachment attachment1, IAttachment attachment2)
        {
            await DeferAsync();
 
        }
    }
}
