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
    public class RequireUserAttribute : PreconditionAttribute
    {
        public RequireUserAttribute(RequireUserType requireUserType = RequireUserType.Mention)
        {
            this.RequireType = requireUserType;
        }
        public RequireUserType? RequireType { get; }
        public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            if (RequireType.HasValue && RequireType.Value == RequireUserType.Mention)
            {
                if (context.Interaction is SocketMessageComponent componentContext)
                {
                    if (componentContext.Message.MentionedUsers.Select(a => a.Id).Contains(context.User.Id))
                        return Task.FromResult(PreconditionResult.FromSuccess());
                }
            }

            return Task.FromResult(PreconditionResult.FromError("RequireUser"));
        }
        public enum RequireUserType
        {
            Mention
        }
    }
}
