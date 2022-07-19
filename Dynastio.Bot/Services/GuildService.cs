using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynastio.Bot
{
    public class GuildService
    {
        private readonly ConcurrentBag<Guild> guilds;
        private readonly MongoService mongo;
        public GuildService(MongoService mongo)
        {
            Program.Log("GuildService", "StartAsync");

            guilds = new();
            this.mongo = mongo;
        }
        public async Task<Guild> GetGuildAsync(ulong id)
        {
            Guild guild = guilds.FirstOrDefault(a => a.Id == id);
            if (guild == null || guild == default)
            {
                guild = await mongo.GetGuildAsync(id);
                if (guild == null || guild == default)
                {
                    guild = new Guild(this)
                    {
                        Id = id,
                    };
                    await mongo.InsertAsync(guild);
                }
                guilds.Add(guild);
            }
            return guild;
        }
        public async Task<bool> UpdateAsync(Guild guild)
        {
            await mongo.UpdateAsync(guild);
            return true;
        }
    }
}
