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
    public class ToplistModule : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        public GraphicService GraphicService { get; set; }
        public DynastioClient Dynastio { get; set; }

        [RateLimit(10, 1, RateLimit.RateLimitType.User)]
        [SlashCommand("toplist", "a list of top players")]
        public async Task toplist(
              [Autocomplete(typeof(SharedAutocompleteHandler.OnlineServersAutocompleteHandler))] string server = "",
              [MaxValue(50)] int take = 25,
              SortType sort = SortType.Score,
              Map Map = Map.Disable,
              int page = 1,
            DynastioProviderType provider = DynastioProviderType.Main)
        {
            await DeferAsync();
            var dynastioProvider = Dynastio[provider];

            var players = dynastioProvider.OnlinePlayers.Where(a => a.Parent.IsPrivate == false).ToList() ?? null;
            if (players == null)
            {
                await FollowupAsync(embed: "No any online server found.".ToWarnEmbed("Not Found !"));
                return;
            }
            players = players.Where(
                a =>
                a.Parent.Label.ToLower().Contains(server)
                ).ToList();

            players = sort == SortType.Score ?
                 players.OrderByDescending(a => a.Score).ToList() :
                 players.OrderByDescending(a => a.Level).ToList();

            var players1 = players.Skip((page - 1) * take).Take(take).ToList();
            var content = players1.ToStringTable(new[] { "#", this["server"], this["score"], this["level"], this["team"], this["nickname"] },
                a => players.IndexOf(a),
                a => a.Parent.Label.Summarizing(16),
                a => a.Score.Metric(),
                a => a.Level.Metric(),
                a => a.Team.RemoveLines().Summarizing(10),
                a => a.Nickname.RemoveLines().Summarizing(16)).ToMarkdown();

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
        }
        public enum Map
        {
            Enable,
            Disable
        }
    }
}
