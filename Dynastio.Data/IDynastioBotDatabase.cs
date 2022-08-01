using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynastio.Data
{
    public interface IDynastioBotDatabase
    {
        Task<IDynastioBotDatabase> InitializeAsync();

        //Guilds
        Task<List<Guild>> GetGuildsByMessageOnlyChannelsAsync();
        Task<Guild> GetGuildAsync(ulong Id);
        Task<bool> InsertAsync(Guild guild);
        Task<bool> UpdateAsync(Guild guild);

        //Users
        Task<List<User>> Get10TopHonor(int count = 10);
        List<User> GetAll();
        Task<User> GetUserAsync(ulong Id);
        Task<bool> InsertAsync(User Buser);
        Task<bool> UpdateAsync(User Buser);
        Task<bool> UpdateManyAsync(List<User> users);
        Task<bool> DeleteAsync(User user);
    }
}
