using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynastio.Bot;
using Discord.WebSocket;
using Discord;

namespace Discord.Interactions
{
    public class RequireGuildRole : PreconditionAttribute
    {
        public ulong? GuildRole
        {
            get;
        }
        public ulong? GuildId
        {
            get;
        }
        public RequireGuildRole(ulong RoleId, ulong GuildId)
        {
            this.GuildRole = RoleId;
            this.GuildId = GuildId;
        }
        public RequireGuildRole(ulong RoleId)
        {
            this.GuildRole = RoleId;
        }
     
        public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            if(Program.IsDebug())
                return Task.FromResult(PreconditionResult.FromSuccess());

            var guild = context.Guild as SocketGuild;
            if (this.GuildId.HasValue) guild = (context.Client as DiscordSocketClient).Guilds.Where(a => a.Id == this.GuildId).FirstOrDefault();

            if (guild == null) return Task.FromResult(PreconditionResult.FromError(this.ErrorMessage ?? "Guild Not Found."));

            var user = guild.GetUser(context.User.Id);
            if (user == null) return Task.FromResult(PreconditionResult.FromError(this.ErrorMessage ?? "User Not Found."));

            if (user.Roles.Where(r => r.Id == this.GuildRole).Any())
                return Task.FromResult(PreconditionResult.FromSuccess());

            return Task.FromResult(PreconditionResult.FromError(this.ErrorMessage ?? "You don't have permission."));
        }
    }
}
