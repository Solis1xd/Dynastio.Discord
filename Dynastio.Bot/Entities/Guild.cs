using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynastio.Bot
{
    [BsonIgnoreExtraElements]
    public class Guild
    {
        private readonly GuildService _guildService;
        public Guild() { }

        public Guild(GuildService guildService)
        {
            _guildService = guildService;
        }
        [BsonId]
        public ulong Id { get; set; }
        public bool IsModerationEnabled { get; set; } = false;
        public List<ulong> OnlyImageChannels { get; set; } = new();

        public async Task UpdateAsync()
        {
            if (_guildService != null)
                await _guildService.UpdateAsync(this);
        }
    }
}
