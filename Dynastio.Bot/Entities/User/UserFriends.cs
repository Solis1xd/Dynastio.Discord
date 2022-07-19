using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynastio.Net;

namespace Dynastio.Bot
{
    public class UserFriends
    {
        public List<UserFriend> Friends { get; set; } = new();
        public List<UserFriendRequest> Requests { get; set; } = new();
        public UserFriend Get(ulong id)
        {
            return Friends.Where(a => a.Id == id).FirstOrDefault();
        }
        public void RemoveFriendAsync(UserFriend friend)
        {
            Friends.Remove(friend);
        }
      
    }
}
