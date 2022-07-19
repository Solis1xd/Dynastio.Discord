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
    public class RequireBotUserPermissionAttribute : PreconditionAttribute
    {
        public RequireBotUserPermissionAttribute(UserPermission Permission)
        {
            this.Permission = Permission;
        }
        public UserPermission Permission { get; set; }
        public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            if ((context as ICustomInteractionContext).BotUser.Permissions.HasOnePermission(Permission))
                return Task.FromResult(PreconditionResult.FromError(this.ErrorMessage ?? $"Require `{Permission}` permission"));

            return Task.FromResult(PreconditionResult.FromSuccess());

        }
    }
}
