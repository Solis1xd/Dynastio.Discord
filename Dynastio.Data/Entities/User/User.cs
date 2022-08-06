using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;

namespace Dynastio.Data
{
    [BsonIgnoreExtraElements]
    public class User
    {
        private readonly IDynastioBotDatabase db;
        public User() { }

        public User(IDynastioBotDatabase db)
        {
            this.db = db;
        }
        public ulong Id { get; set; }
        public List<UserAccount> Accounts { get; set; } = new();
        public DateTime LastHonorGift { get; set; } = DateTime.MinValue;
        public int Honor { get; set; } = 0;

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
            await db.UpdateAsync(this);
        }
        public UserAccount GetAccount() => Accounts.OrderByDescending(a => a.IsDefault).FirstOrDefault();
        public UserAccount GetAccount(int hashcode) => Accounts.FirstOrDefault(a => a.GetHashCode() == hashcode);
        public UserAccount GetAccount(string Id) => Accounts.Where(a => a.Id == Id).FirstOrDefault();
        public UserAccount GetAccountByNickname(string Name) => Accounts.Where(a => a.Nickname == Name).FirstOrDefault();
        public string GetGameAccountsId() => string.Join(", ", Accounts.Select(a => a.Id));


    }


}
