using Dynastio.Net;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynastio.Bot
{
    public class Configuration
    {
        public string BotStatus { get; set; }
        public string BotToken { get; set; }
        public string DynastioApi { get; set; }
        public string YoutubeApiKey { get; set; }
        public string DynastioYoutubeChannelId { get; set; }
        public string DatabaseConnectionString { get; set; }
        public ulong OwnerId { get; set; }
        public GuildsConfiguration Guilds { get; set; }
        public ChannelsConfiguration Channels { get; set; }

        public static Configuration Get(string debugPath)
        {
            if (Program.IsDebug())
            {
                var configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(debugPath));
                return configuration;
            }


            string path = Environment.GetEnvironmentVariable("path");
            if (!string.IsNullOrEmpty(path))
                return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(path));

            string config = Environment.GetEnvironmentVariable("config");
            if (!string.IsNullOrEmpty(config))
                return JsonConvert.DeserializeObject<Configuration>(config);

            var configuration_ = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(@"config.json"));
            return configuration_;

            string key = Environment.GetEnvironmentVariable("key");
            string encryptedvalue = Environment.GetEnvironmentVariable("encryptedvalue");

            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(encryptedvalue))
                return JsonConvert.DeserializeObject<Configuration>(Encryption.Decrypt(encryptedvalue, key));

            return null;
        }
    }
    public class GuildsConfiguration
    {
        public ulong MainServer { get; set; }
        public ulong DebugServer { get; set; }
        public string MainGuildInviteLink { get; set; }
    }
    public class ChannelsConfiguration
    {
        public ulong JoinLeftLoggerChannel { get; set; }
        public ulong ErrorLoggerChannel { get; set; }
        public ulong HonorChannel { get; set; }
        public ulong UploadsChannel { get; set; }
    }
}
