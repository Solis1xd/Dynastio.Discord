using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Dynastio.Data
{
    public class DirectoryDb : IDatabase, IDisposable
    {
        private string _path = "";
        public DirectoryDb(string path)
        {
            _path = path;
        }
        private string usersPath { get => Path.Combine(_path + "users.json"); }
        private string guildsPath { get => Path.Combine(_path + "guilds.json"); }

        public async Task<IDatabase> InitializeAsync()
        {
            Program.Log("DirectoryDb", "InitializeAsync");

            try
            {
                if (!Directory.Exists(_path))
                {
                    Program.Log("DirectoryDb", "Directory path not found.", ConsoleColor.Red);
                    Program.Log("DirectoryDb", "Set default directory path.", ConsoleColor.Green);

                    _path = Directory.GetCurrentDirectory();
                }

                if (!File.Exists(usersPath))
                    SaveUsersChanges();

                if (!File.Exists(guildsPath))
                    SaveGuildChanges();

                var users = File.ReadAllText(usersPath);
                _users = JsonConvert.DeserializeObject<List<User>>(users);

                var guilds = File.ReadAllText(guildsPath);
                _guilds = JsonConvert.DeserializeObject<List<Guild>>(guilds);

                Program.Log("DirectoryDb", "Initialized");

                return await Task.FromResult(this);
            }
            catch
            {
                Program.Log("DirectoryDb", "Directory path not found.", ConsoleColor.Red);
                return new NoDatabaseDb();
            }
        }

        private List<User> _users { get; set; } = new();
        private List<Guild> _guilds { get; set; } = new();

        void SaveUsersChanges()
        {
            var data = JsonConvert.SerializeObject(_users);
            File.WriteAllText(usersPath, data);
        }
        void SaveGuildChanges()
        {
            var data = JsonConvert.SerializeObject(_guilds);
            File.WriteAllText(guildsPath, data);
        }
        public async Task<List<Guild>> GetGuildsByMessageOnlyChannelsAsync()
        {
            var result = _guilds.Where(a => a.OnlyImageChannels.Count > 0).ToList();
            return await Task.FromResult(result);
        }
        public List<Guild> GetAllGuilds() => _guilds.Where<Guild>(_ => true).ToList();
        public async Task<Guild> GetGuildAsync(ulong Id)
        {
            var result = _guilds.FirstOrDefault(a => a.Id == Id);
            return await Task.FromResult(result);
        }
        public async Task<bool> InsertAsync(Guild guild)
        {
            _guilds.Add(guild);
            SaveGuildChanges();
            return await Task.FromResult(true);
        }
        public async Task<bool> UpdateAsync(Guild guild)
        {
            var old = _guilds.FirstOrDefault(a => a.Id == guild.Id);
            _guilds.Remove(old);
            _guilds.Add(guild);

            SaveGuildChanges();
            return await Task.FromResult(true);
        }

        public async Task<List<User>> Get10TopHonor(int count = 10)
        {
            return await Task.FromResult(_users.OrderByDescending(a => a.Honor).Take(count).ToList());
        }
        public List<User> GetAllUsers() => _users.ToList();
        public async Task<User> GetUserAsync(ulong Id)
        {
            var result = _users.FirstOrDefault(a => a.Id == Id);
            return await Task.FromResult(result);
        }
        public async Task<bool> InsertAsync(User Buser)
        {
            _users.Add(Buser);

            SaveUsersChanges();
            return await Task.FromResult(true);
        }
        public async Task<bool> UpdateAsync(User Buser)
        {
            var old = _users.FirstOrDefault(a => a.Id == Buser.Id);
            _users.Remove(old);
            _users.Add(Buser);

            SaveUsersChanges();
            return await Task.FromResult(true);
        }
        public async Task<User> GetUserByAccountIdAsync(string Id)
        {
            var result = _users.Where(a => a.Accounts.Any(b => b.Id == Id)).FirstOrDefault();
            return await Task.FromResult(result);
        }

        public async Task<bool> UpdateManyAsync(List<User> users)
        {
            foreach (var user in users)
            {
                var old = _users.FirstOrDefault(a => a.Id == user.Id);
                _users.Remove(old);
                _users.Add(user);
            }

            SaveUsersChanges();
            return await Task.FromResult(true);
        }
        public async Task<bool> DeleteAsync(User user)
        {
            _users.Remove(user);

            SaveUsersChanges();
            return await Task.FromResult(true);
        }

        public void Dispose()
        {
            _users.Clear();
            _guilds.Clear();
        }


    }
}
