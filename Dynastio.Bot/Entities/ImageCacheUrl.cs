using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynastio.Bot
{
    public class ImageCacheUrl
    {
        public ImageCacheUrl(string url = "", int expireAt = 0)
        {
            UploadedAt = DateTime.UtcNow;
            this.Url = url;
            this.ExpireTime = expireAt;
        }
        public int ExpireTime { get; set; }
        public string Url { get; set; }
        public DateTime UploadedAt { get; }
        public bool IsAllowedToUploadNew()
        {
            if ((DateTime.UtcNow - UploadedAt).TotalSeconds < ExpireTime)
            {
                return false;
            }
            return true;
        }
    }
}
