﻿using Dynastio.Net;

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
        public string BotToken { get; set; }
        public string Status { get; set; }
        public string YoutubeApi { get; set; }
        public string DynastioApi { get; set; }
        public string MongoConnection { get; set; }
        public string YoutubeLink { get; set; }
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

            string key = Environment.GetEnvironmentVariable("key");
            string encryptedvalue = Environment.GetEnvironmentVariable("encryptedvalue");

            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(encryptedvalue))
                return JsonConvert.DeserializeObject<Configuration>(Encryption.Decrypt(encryptedvalue, key));

            return null;
        }
    }
    public class GuildsConfiguration
    {
        public ulong Main { get; set; }
        public ulong Test { get; set; }
        public string InviteLinkMain { get; set; }

    }
    public class ChannelsConfiguration
    {

        public ulong Logger { get; set; }
        public ulong ErrorLogger { get; set; }
        public ulong Uploads { get; set; }
        public ulong Honor { get; set; }
    }
}
