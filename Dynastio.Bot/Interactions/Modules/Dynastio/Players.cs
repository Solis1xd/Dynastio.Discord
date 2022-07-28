using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Dynastio.Net;

using Discord.WebSocket;

namespace Dynastio.Bot.Interactions.Modules.Dynastio
{

    [RequireContext(ContextType.Guild)]
    [RequireBotPermission(ChannelPermission.AttachFiles)]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    public class PlayersModule : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        public GraphicService GraphicService { get; set; }
        public DynastioClient Dynastio { get; set; }

        [RateLimit(10, 1, RateLimit.RateLimitType.User)]
        [SlashCommand("players", "a list of online players")]
        public async Task players(
            [Autocomplete(typeof(SharedAutocompleteHandler.OnlineServersAutocompleteHandler))] string server = "",
            FilterType filter = FilterType.PublicServer, SortType sort = SortType.Score,
            Map Map = Map.Disable,
            [MaxValue(25)] int take = 25,
            string nickname = "",
            int MinLevel = 0,
            int MaxLevel = int.MaxValue,
            int MinScore = 0,
            int MaxScore = int.MaxValue,
            int page = 1,
            DynastioProviderType provider = DynastioProviderType.Main)
        {
            await DeferAsync();
            var dyanstioProvider = Dynastio[provider];
            var players = dyanstioProvider.OnlinePlayers.ToList() ?? null;
            if (players == null)
            {
                await FollowupAsync(embed: "No any online server found.".ToWarnEmbed("Not Found !"));
                return;
            }
            players = players.Where(
                a =>
                a.Nickname.ToLower().Contains(nickname) &&
                a.Parent.Label.ToLower().Contains(server) &&
                a.Level > MinLevel && a.Level < MaxLevel &&
                a.Score > MinScore && a.Score < MaxScore
                ).ToList();

            if (filter != FilterType.All) players = players.Where(a => filter == FilterType.PrivateServer ? a.Parent.IsPrivate : !a.Parent.IsPrivate).ToList();

            switch (sort)
            {
                case SortType.Score:
                    players = players.OrderByDescending(a => a.Score).ToList();
                    break;
                case SortType.Level:
                    players = players.OrderByDescending(a => a.Level).ToList();
                    break;
                case SortType.Team:
                    players = players.OrderByDescending(a => a.Team).ToList();
                    break;
                case SortType.Server:
                    players = players.OrderByDescending(a => a.Parent.Label).ToList();
                    break;
                case SortType.Nickname:
                    players = players.OrderByDescending(a => a.Nickname).ToList();
                    break;
                case SortType.Region:
                    players = players.OrderByDescending(a => a.Parent.Region).ToList();
                    break;
            }
            var players1 = players.Skip((page - 1) * take).Take(take).ToList();
            var content = players1.ToStringTable(new[] { "#", this["server"], this["score"], this["level"], this["team"], this["nickname"] },
                a => players.IndexOf(a),
                a => a.Parent.Label.Summarizing(16),
                a => a.Score.Metric(),
                a => a.Level.Metric(),
                a => a.Team.RemoveLines().Summarizing(10),
                a => a.Nickname.RemoveLines().Summarizing(16)).ToMarkdown();

            string content1 = this["total_servers:*", dyanstioProvider.OnlineServers.Count] + " | " +
                              this["total_players:*", dyanstioProvider.OnlinePlayers.Count] + "\n" +
                              this["page:*", $"{page}/{players.Count() / take}"].Bold() + "\n" +
                              this["closes:*", 50.ToDiscordUnixTimeFromat()];

            var map = Map == Map.Enable ? GraphicService.GetMap(players1) : null;


            var embeds = map != null ?
                new Embed[] { content.ToEmbed(), "".ToEmbed(ImageUrl: "attachment://map.jpeg") } :
                new Embed[] { content.ToEmbed() };

            var msg = map != null ?
                await FollowupWithFileAsync(map, "map.jpeg", Context.User.Id.ToUserMention(), embeds) :
                await FollowupAsync(Context.User.Id.ToUserMention(), embeds);
        }

        public enum SortType
        {
            Score,
            Level,
            Team,
            Server,
            Nickname,
            Region,
        }
        public enum FilterType
        {
            PrivateServer,
            PublicServer,
            All
        }
        public enum Map
        {
            Enable,
            Disable
        }
    }
}
