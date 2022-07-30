using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Dynastio.Data
{
    public class MongoDb : IDynastioBotDatabase
    {
        private MongoClient _db { get; set; }
        public MongoDb(string mongoConnection)
        {
            Program.Log("Mongodb", "InitializeAsync");

            _db = new MongoClient(mongoConnection);
            _dynastio = _db.GetDatabase(Program.IsDebug() ? "Dynastio_Debug" : "Dynastio");

            Program.Log("Mongodb", "Initialized");
        }
        public bool IsConnected { get; set; }
        public async Task InitializeAsync()
        {
            try
            {
                Program.Log("Mongodb", "StartSessionAsync");

                await _db.StartSessionAsync();

                Program.Log("Mongodb", "Session Started");

                IsConnected = true;
            }
            catch
            {
                IsConnected = false;
                Console.WriteLine("Mongodb Not Connected");
            }
        }

        private IMongoDatabase _dynastio;
        private IMongoCollection<User> _users => _dynastio.GetCollection<User>("Users");
        private IMongoCollection<Guild> _guilds => _dynastio.GetCollection<Guild>("Guilds");
        public async Task<List<Guild>> GetGuildsByMessageOnlyChannelsAsync()
        {
            if (IsConnected)
            {
                var result = _guilds.Find(a => a.OnlyImageChannels.Count > 0).ToList();
                return await Task.FromResult(result);
            }
            return new();
        }
        public async Task<Guild> GetGuildAsync(ulong Id)
        {
            if (IsConnected)
            {
                var result = _guilds.Find(a => a.Id == Id).FirstOrDefault();
                return await Task.FromResult(result);
            }
            return await Task.FromResult(new Guild()
            {
                Id = Id
            });
        }
        public async Task<bool> InsertAsync(Guild guild)
        {
            if (IsConnected)
                _guilds.InsertOne(guild);

            return await Task.FromResult(true);
        }
        public async Task<bool> UpdateAsync(Guild guild)
        {
            if (IsConnected)
                _guilds.ReplaceOne(a => a.Id == guild.Id, guild);
            return await Task.FromResult(true);
        }

        public async Task<List<User>> Get10TopHonor(int count = 10)
        {
            if (IsConnected)
            {
                var filter = Builders<User>.Filter.Empty;
                var sort = Builders<User>.Sort.Descending(a => a.Honor);
                var result = await _users.FindAsync(filter, new FindOptions<User, User>()
                {
                    Sort = sort,
                    Limit = count,
                });
                return result.ToList();
            }
            return new();
        }
        public List<User> GetAll() => IsConnected ? _users.Find<User>(_ => true).ToList() : new();
        public async Task<User> GetUserAsync(ulong Id)
        {
            if (IsConnected)
            {
                var result = _users.Find(a => a.Id == Id).FirstOrDefault();
                return await Task.FromResult(result);
            }
            return new User() { Id = Id };
        }
        public async Task<bool> InsertAsync(User Buser)
        {
            if (IsConnected)
                _users.InsertOne(Buser);
            return await Task.FromResult(true);
        }
        public async Task<bool> UpdateAsync(User Buser)
        {
            if (IsConnected)
                _users.ReplaceOne(a => a.Id == Buser.Id, Buser);
            return await Task.FromResult(true);
        }
        //public async Task<User> GetUserByAccountIdAsync(string Id)
        //{
        //    var filter = Builders<User>.Filter.ElemMatch(o => o.GameAccounts, Builders<DynastioAccount>.Filter.Where(a => a.Id == Id));
        //    var result = _users.Find(filter).FirstOrDefault();
        //    return await Task.FromResult(result);
        //}
        //public async Task<List<User>> GetUsersByAccountIdAsync(List<string> toFind)
        //{
        //    var regexList = toFind.Select(x => new BsonRegularExpression(x));

        //    var filterList = new List<FilterDefinition<User>>();
        //    foreach (var bsonRegularExpression in regexList)
        //    {
        //        FilterDefinition<User> fil = Builders<User>.Filter.ElemMatch(o => o.GameAccounts, Builders<DynastioAccount>.Filter.Regex(x => x.Id, bsonRegularExpression));
        //        filterList.Add(fil);
        //    }

        //    var orFilter = Builders<User>.Filter.Or(filterList);
        //    var result = _users.Find(orFilter).ToList();
        //    return await Task.FromResult(result);
        //}

        public async Task<bool> UpdateManyAsync(List<User> users)
        {
            if (IsConnected)
            {
                var updates = new List<WriteModel<User>>();
                foreach (var user in users)
                {
                    var filter = Builders<User>.Filter.Where(u => u.Id == user.Id);
                    updates.Add(new ReplaceOneModel<User>(filter, user));
                }
                await _users.BulkWriteAsync(updates, new BulkWriteOptions() { IsOrdered = false });
            }
            return await Task.FromResult(true);
        }
        public async Task<bool> DeleteAsync(User user)
        {
            if (IsConnected)
                _users.DeleteOne(a => a.Id == user.Id);
            return await Task.FromResult(true);
        }
    }
}
