using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynastio.Net;
using MongoDB.Bson;

namespace Dynastio.Bot
{
    [BsonIgnoreExtraElements]
    public class User
    {
        private readonly UserService _userManager;
        public User() { }

        public User(UserService userManager)
        {
            _userManager = userManager;
        }
        [BsonIgnore] public ImageCacheUrl AllChests { get; set; } = new();
        public ulong Id { get; set; }

        [BsonElement("GameAccounts")]
        public List<UserAccount> Accounts { get; set; } = new();
        public UserSettings Settings { get; set; } = new();
        public UserPermissions Permissions { get; set; } = new();
        public UserFriends Friends { get; set; } = new();
        public string BanReason { get; set; } = "";
        public bool Banned { get; set; } = false;
        public DateTime BannedAt { get; set; } = DateTime.MinValue;
        public TimeSpan BannedTime { get; set; } = TimeSpan.Zero;
        public DateTime LastHonorGift { get; set; } = DateTime.MinValue;
        public int Honor { get; set; } = 0;
        public bool BannedFeedback { get; set; } = false;

        public void AddAccount(UserAccount account)
        {
            UserAccount.AssignAccountName(this, ref account);
            Accounts.Add(account);
        }
        public void RemoveAccount(UserAccount account)
        {
            Accounts.Remove(account);
        }
        public bool ReplaceAccount(string id, UserAccount account)
        {
            var acc = Accounts.Find(x => x.Id == id);
            if (Accounts.Remove(acc))
            {
                AddAccount(account);
                return true;
            }
            return false;
        }
        public void SwitchDefault(UserAccount account)
        {
            foreach (var acc in Accounts)
                acc.IsDefault = false;
            Accounts.Find(a => a.Id == account.Id).IsDefault = true;
        }
        public async Task UpdateAsync()
        {
            if (_userManager != null)
                await _userManager.UpdateAsync(this);
        }
        public UserAccount GetAccount() => Accounts.OrderByDescending(a => a.IsDefault).FirstOrDefault();
        public UserAccount GetAccountByHashCode(int hashcode) => Accounts.Where(a => a.GetHashCode() == hashcode).FirstOrDefault();

        public UserAccount GetAccount(string Id) => Accounts.Where(a => a.Id == Id).FirstOrDefault();
        public UserAccount GetAccountByNickname(string Name) => Accounts.Where(a => a.Nickname == Name).FirstOrDefault();
        public string GetGameAccountsId() => string.Join(", ", Accounts.Select(a => a.Id));


    }


}
