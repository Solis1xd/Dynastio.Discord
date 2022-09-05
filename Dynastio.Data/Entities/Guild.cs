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
        public Guild() { }
        [BsonId]
        public ulong Id { get; set; }
        public bool IsModerationEnabled { get; set; } = false;
        public List<ulong> OnlyImageChannels { get; set; } = new();
        public async Task UpdateAsync(IDatabaseContext db)
        {
            await db.UpdateAsync(this);
        }
    }
}
