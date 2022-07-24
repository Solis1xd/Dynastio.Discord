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
using System.Collections.Concurrent;

namespace Discord.Interactions
{
    public class RateLimit : PreconditionAttribute
    {
        private static ConcurrentDictionary<ulong, List<RateLimitItem>> Items = new();
        private readonly RateLimitType? _ratelimitType;
        private readonly RateLimitBaseType _baseType;
        private readonly int _requests;
        private readonly int _seconds;
        public RateLimit(int Seconds = 4, int Requests = 1, RateLimitType Context = RateLimitType.User, RateLimitBaseType BaseType = RateLimitBaseType.BaseOnCommandInfo)
        {
            this._ratelimitType = Context;
            this._requests = Requests;
            this._seconds = Seconds;
            this._baseType = BaseType;
        }
  
        public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            ulong id = _ratelimitType.Value switch
            {
                RateLimitType.User => context.User.Id,
                RateLimitType.Channel => context.Channel.Id,
                RateLimitType.Guild => context.Guild.Id,
                RateLimitType.Global => 0,
                _ => 0
            };

            var command = _baseType switch
            {
                RateLimitBaseType.BaseOnCommandInfo => commandInfo.Module.Name + "//" + commandInfo.MethodName,
                RateLimitBaseType.BaseOnMessageComponentCustomId => (context.Interaction as SocketMessageComponent).Data.CustomId,
                _ => "unknown"
            };
            var dateTime = DateTime.UtcNow;

            var target = Items.GetOrAdd(id, new List<RateLimitItem>());

            var commands = target.Where(
                a =>
                a.command == command
            ).ToList();
            
            foreach (var c in commands)
            {
                if ((dateTime - c.createdAt).TotalSeconds > _seconds)
                    target.Remove(c);
            }

            if (commands.Count > _requests)
                return Task.FromResult(PreconditionResult.FromError($"{_ratelimitType} using this command very fast, you can use this command every {_seconds} seconds, wait a while."));


            target.Add(new RateLimitItem()
            {
                command = command,
                createdAt = DateTime.UtcNow
            });

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
        private class RateLimitItem
        {
            public string command { get; set; }
            public DateTime createdAt { get; set; }
        }
        public enum RateLimitType
        {
            User,
            Channel,
            Guild,
            Global
        }
        public enum RateLimitBaseType
        {
            BaseOnCommandInfo,
            BaseOnMessageComponentCustomId,
        }
    }
}
