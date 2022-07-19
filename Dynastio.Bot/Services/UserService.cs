﻿using Dynastio.Net;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Dynastio.Bot
{
    public class UserService
    {
        private readonly ConcurrentBag<User> users;
        private readonly MongoService mongo;
        private readonly DynastioClient dynastioClient;
        public UserService(MongoService mongo, DynastioClient dynastioClient)
        {
            Program.Log("UserService", "StartAsync");

            users = new();
            this.mongo = mongo;
            this.dynastioClient = dynastioClient;

        }
        public async Task<bool> UpdateAsync(User user)
        {
            await mongo.UpdateAsync(user);
            return true;
        }
        private bool _Is10TopHonorUpdated = false;
        public async Task<List<User>> Get10TopHonor()
        {
            if (_Is10TopHonorUpdated)
                return this.users.OrderByDescending(a => a.Honor).Take(10).ToList();

            var users = await this.mongo.Get10TopHonor(10);
            foreach (var user in users)
            {
                if (IsCached(user.Id) is false) Cache(user);
            }
            _Is10TopHonorUpdated = true;
            return users;
        }
        public async Task<User> GetUserAsync(ulong Id)
        {
            User user = users.FirstOrDefault(x => x.Id == Id);
            if (user is null)
            {
                user = await mongo.GetUserAsync(Id);

                if (user is null)
                {
                    user = await GetNewUserAsync(Id);
                    await mongo.InsertAsync(user);
                }
                Cache(user);
            }
            return user;
        }
        public void Cache(User user)
        {
            users.Add(user);
        }
        public bool IsCached(ulong Id)
        {
            return this.users.FirstOrDefault(x => x.Id == Id) != null;
        }
        async Task<User> GetNewUserAsync(ulong id)
        {
            bool result = await dynastioClient.Database.IsUserAccountExistAsync("discord:" + id);
            var user = new User(this)
            {
                Id = id,
                Honor = 0,
                Accounts = new()
            };
            if (result)
            {
                var account = new UserAccount()
                {
                    Id = "discord:" + id,
                    AddedAt = DateTime.UtcNow,
                    Nickname = "Discord",
                    IsDefault = true
                };
                user.Accounts.Add(account);
            }
            return user;
        }
    }
}
