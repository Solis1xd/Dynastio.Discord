using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Dynastio.Bot
{
    public class EventHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly Configuration _configuration;
        public readonly GuildService _guildService;
        private readonly LocaleService _localeService;
        private readonly MongoService _mongoService;
        public EventHandler(IServiceProvider services)
        {
            _client = services.GetRequiredService<DiscordSocketClient>();
            _configuration = services.GetRequiredService<Configuration>();
            _guildService = services.GetRequiredService<GuildService>();
            _localeService = services.GetRequiredService<LocaleService>();
            _mongoService = services.GetRequiredService<MongoService>();
            _services = services;
        }
        public void Initialize()
        {
            _client.Ready += _client_Ready;
            _client.JoinedGuild += _client_JoinedGuild;
            _client.LeftGuild += _client_LeftGuild;
            _client.UserJoined += _client_UserJoined;
        }

        private async Task _client_Ready()
        {
            await _client.SetStatusAsync(UserStatus.Online);

            if (!string.IsNullOrEmpty(_configuration.Status))
                await _client.SetGameAsync(_configuration.Status, "", ActivityType.Playing);


            if (!Program.IsDebug())
            {
                await LeaveExtraGuild().Try();
                ChannelUtilities.CheckImageOnlyChannels(_mongoService, _client, _localeService).RunInBackground(true);
            }
        }


        private async Task _client_LeftGuild(SocketGuild guild)
        {
            if (_configuration.Guilds.Main == 0 || _configuration.Channels.Logger == 0)
                return;

            await _client.GetGuild(_configuration.Guilds.Main).GetTextChannel(_configuration.Channels.Logger).SendMessageAsync($"**-** Left `{guild.Name}` with `{guild.MemberCount}` members.");
        }
        private async Task _client_JoinedGuild(SocketGuild guild)
        {
            if (_configuration.Guilds.Main == 0 || _configuration.Channels.Logger == 0)
                return;

            await _client.GetGuild(_configuration.Guilds.Main).GetTextChannel(_configuration.Channels.Logger).SendMessageAsync($"**+** Joined `{guild.Name}` with `{guild.MemberCount}` members, `<@{guild.OwnerId}>` - Guilds **`{_client.Guilds.Count}`**");

            await LeaveExtraGuild().Try();
        }
        public async Task LeaveExtraGuild(string reason = null)
        {
            if (_client.Guilds.Count > 99)
            {
                var gLeave = _client.Guilds.OrderBy(a => a.MemberCount).Where(a => true).FirstOrDefault();
                try
                {
                    await gLeave.Owner.SendMessageAsync($"we are supporting 100 servers only, i invited to new server with more users, i have to leave from your server {gLeave.Name}.\n" +
                        $"The robot acts like an auction, whichever server has more members, the robot prefers that server.\n" +
                        $"You need more members\nSupport & More {_configuration.Guilds.InviteLinkMain}");
                }
                catch { }
                await gLeave.LeaveAsync();
            }
        }
        private async Task _client_UserJoined(SocketGuildUser arg)
        {
            if (_configuration.Guilds.Main == 0 || _configuration.Guilds.InviteLinkMain == default)
                return;

            string img = "https://cdn.discordapp.com/attachments/916998929609023509/976446157880438845/Untitled.jpg";
            try
            {
                if (arg.Guild.Id == _configuration.Guilds.Main) return;

                await arg.SendMessageAsync(_configuration.Guilds.InviteLinkMain, embed: "".ToEmbed(ImageUrl: img),
                components: new ComponentBuilder()
                            .WithButton("Как взломать монеты ?", null, ButtonStyle.Link, null, _configuration.YoutubeLink)
                            .WithButton("How To Hack Dynast.io ?", null, ButtonStyle.Link, null, _configuration.YoutubeLink)
                            .WithButton("Official Dynast.io Channel", null, ButtonStyle.Link, null, _configuration.YoutubeLink)
                .Build());
            }
            catch { }
        }
    }
}
