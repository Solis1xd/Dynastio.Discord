using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynastio.Data
{
    public class NoDatabaseDb : IDynastioBotDatabase
    {
        public NoDatabaseDb()
        {
            Program.Log("NoDatabaseDb", "No Database");
        }

        public Task<IDynastioBotDatabase> InitializeAsync()
        {
            return Task.FromResult((IDynastioBotDatabase)this);
        }

        public Task<bool> UpdateAsync(Guild guild)
        {
            return Task.FromResult(true);
        }
        public Task<bool> InsertAsync(Guild guild)
        {
            return Task.FromResult(true);
        }
        public Task<Guild> GetGuildAsync(ulong Id)
        {
            return Task.FromResult(new Guild()
            {
                Id = Id
            });
        }
        public List<Guild> GetAllGuilds() => new();
        public Task<List<Guild>> GetGuildsByMessageOnlyChannelsAsync()
        {
            return Task.FromResult(new List<Guild>());
        }
        public Task<bool> DeleteAsync(User user)
        {
            return Task.FromResult(true);
        }
        public Task<List<User>> Get10TopHonor(int count = 10)
        {
            return Task.FromResult(new List<User>());
        }
        public List<User> GetAllUsers()
        {
            return new List<User>();
        }
        public Task<User> GetUserAsync(ulong Id)
        {
            return Task.FromResult(new User()
            {
                Id = Id
            });
        }
        public async Task<User> GetUserByAccountIdAsync(string Id)
        {
            return await Task.FromResult(default(User));
        }
        public Task<bool> InsertAsync(User Buser)
        {
            return Task.FromResult(true);
        }
        public Task<bool> UpdateAsync(User Buser)
        {
            return Task.FromResult(true);
        }
        public Task<bool> UpdateManyAsync(List<User> users)
        {
            return Task.FromResult(true);
        }

    }
}
