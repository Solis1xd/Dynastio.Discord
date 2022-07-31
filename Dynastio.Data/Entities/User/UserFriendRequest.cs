using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dynastio.Data
{
    public class UserFriendRequest
    {
        public UInt64 Sender { get; set; }
        public string Nickname { get; set; }
        public string AvatarUrl { get; set; }
        public string TargetNickname { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
