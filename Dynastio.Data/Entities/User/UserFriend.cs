using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dynastio.Data
{
    public class UserFriend
    {
        [BsonId]
        public UInt64 Id { get; set; }
        public string Nickname { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
