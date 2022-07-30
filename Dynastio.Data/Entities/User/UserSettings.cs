
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynastio.Data
{
    [BsonIgnoreExtraElements]
    public class UserSettings
    {
        public bool IsPChestPrivate { get; set; }
        public DateTime PchestUpdatedAt { get; set; } = DateTime.MinValue;
        public UserProfileStyle ProfileStyle { get; set; } = UserProfileStyle.Default;
        public UserPersonalchestStyle PchestStyle { get; set; } = UserPersonalchestStyle.Default;

    }
}
