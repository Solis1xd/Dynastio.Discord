using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using Dynastio;
using Dynastio.Bot;

namespace Discord.Interactions
{
    public class RequireAccountAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            if ((context as ICustomInteractionContext).BotUser.GetAccount() is null)
                return Task.FromResult(PreconditionResult.FromError(this.ErrorMessage ?? ((ICustomInteractionContext)context).Locale["require_account"]));

            return Task.FromResult(PreconditionResult.FromSuccess());

        }
    }
}
