using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynastio.Data
{
    [BsonIgnoreExtraElements]
    public class Guild
    {
        private readonly IDatabase db;
        public Guild() { }

        public Guild(IDatabase db)
        {
            this.db = db;
        }
        [BsonId]
        public ulong Id { get; set; }
        public bool IsModerationEnabled { get; set; } = false;
        public List<ulong> OnlyImageChannels { get; set; } = new();
        public async Task UpdateAsync()
        {
            await db.UpdateAsync(this);
        }
    }
}
