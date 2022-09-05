using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynastio.Data
{
    public interface IDatabaseContext
    {
        Task<IDatabaseContext> InitializeAsync();

        //Guilds
        Task<List<Guild>> GetGuildsByMessageOnlyChannelsAsync();
        List<Guild> GetAllGuilds();
        Task<Guild> GetGuildAsync(ulong Id);
        Task<bool> InsertAsync(Guild guild);
        Task<bool> UpdateAsync(Guild guild);

        //Users
        Task<List<User>> Get10TopHonor(int count = 10);
        List<User> GetAllUsers();
        Task<User> GetUserAsync(ulong Id);
        Task<User> GetUserByAccountIdAsync(string Id);
        Task<bool> InsertAsync(User Buser);
        Task<bool> UpdateAsync(User Buser);
        Task<bool> UpdateManyAsync(List<User> users);
        Task<bool> DeleteAsync(User user);
    }
}
