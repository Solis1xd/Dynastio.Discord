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
    [Group("players", "online players")]
    public class PlayersModule : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        public GraphicService GraphicService { get; set; }
        public DynastioClient Dynastio { get; set; }
        public UserService UserService { get; set; }

        [RateLimit(120, 4)]
        [SlashCommand("profile", "online player profile")]
        public async Task profile(
            [Autocomplete(typeof(SharedAutocompleteHandler.OnlineServersAutocompleteHandler))] string server = "",
            [Autocomplete(typeof(SharedAutocompleteHandler.OnlinePlayersAutocompleteHandler))] string player = "",
             DynastioProviderType provider = DynastioProviderType.Main)
        {
            await DeferAsync();
            var player_ = Dynastio[provider].OnlinePlayers.FirstOrDefault(a => a.UniqeId == player);
            if (player_ is null)
            {
                await FollowupAsync(embed: "player not found".ToWarnEmbed("Not Found !"));
                return;
            }
            if (!player_.IsAuth)
            {
                await FollowupAsync(embed: "the player is a guest".ToWarnEmbed("guest !"));
                return;
            }
            var profile = await Dynastio[provider].GetUserProfileAsync(player_.Id).TryGet();
            if (profile == null)
            {
                await FollowupAsync("chest not found, join the game and put something to your chest.");
                return;
            }
            var image = GraphicService.GetProfile(profile);
            await FollowupWithFileAsync(image, "profile.jpeg", $"Online player profile ({player_.Nickname})");
        }
        [RateLimit(120, 4)]
        [SlashCommand("chest", "online player chest")]
        public async Task chest(
            [Autocomplete(typeof(SharedAutocompleteHandler.OnlineServersAutocompleteHandler))] string server = "",
            [Autocomplete(typeof(SharedAutocompleteHandler.OnlinePlayersAutocompleteHandler))] string player = "",
             DynastioProviderType provider = DynastioProviderType.Main)
        {
            await DeferAsync();
            var player_ = Dynastio[provider].OnlinePlayers.FirstOrDefault(a => a.UniqeId == player);
            if (player_ is null)
            {
                await FollowupAsync(embed: "player not found".ToWarnEmbed("Not Found !"));
                return;
            }
            if (!player_.IsAuth)
            {
                await FollowupAsync(embed: "the player is a guest".ToWarnEmbed("guest !"));
                return;
            }
            var chest = await Dynastio[provider].GetUserPersonalchestAsync(player_.Id).TryGet();
            if (chest == null)
            {
                await FollowupAsync("chest not found, join the game and put something to your chest.");
                return;
            }
            var image = GraphicService.GetPersonalChest(chest);
            await FollowupWithFileAsync(image, "chest.jpeg", $"Online player chest ({player_.Nickname})");
        }

        [RateLimit(10, 2, RateLimit.RateLimitType.User)]
        [SlashCommand("find", "find discord user")]
        public async Task Find(IUser user,
            DynastioProviderType provider = DynastioProviderType.Main)
        {
            await DeferAsync();
            var botUser = await UserService.GetUserAsync(user.Id, false);
            if (botUser.Accounts.Count == 0)
            {
                await FollowupAsync(embed: "No any account of this user found.".ToWarnEmbed("Not Found !"));
                return;
            }
            var accounts = botUser.Accounts.Select(a => a.Id).ToList();

            var result = Dynastio[provider].OnlinePlayers.FirstOrDefault(a => accounts.Contains(a.Id));
            if (result == null)
            {
                await FollowupAsync(embed: "No any online account of this user found.".ToWarnEmbed($"{user.Username} is offline !"));
                return;
            }

            var team = Dynastio[provider].OnlinePlayers.GroupBy(a => a.Team).FirstOrDefault(a => a.Key == result.Team && !string.IsNullOrEmpty(a.Key));
            var teammates = team is null ? "`none`" : string.Join(", ", team.Select(a => a.Nickname));
            string content =
                $"**Nickname:** {result.Nickname.RemoveString(18)}\n" +
                $"**Level:** {result.Level}\n" +
                $"**Score:** {result.Score.Metric()}\n" +
                $"**Server:** {result.Parent.Label.RemoveString(20)}\n" +
                $"**Team:** {result.Team}\n" +
                $"**Teammates:**: {teammates.ToMarkdown()}";

            await FollowupAsync(embed: content.ToEmbed(user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl()));
        }

        [RateLimit(10, 1, RateLimit.RateLimitType.User)]
        [SlashCommand("discord", "search for a list of discord players")]
        public async Task discord(
           [Autocomplete(typeof(SharedAutocompleteHandler.OnlineServersAutocompleteHandler))] string server = "",
           Map Map = Map.Disable,
           int skip = 0,
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
                a.IsDiscordAuth() &&
                a.Parent.Label.ToLower().Contains(server)
                ).Skip(skip).Take(20).ToList();

            var content = players.ToStringTable(new[] { "#", this["server"], this["score"], this["level"], this["team"], this["nickname"] },
                a => players.IndexOf(a) + skip,
                a => a.Parent.Label.RemoveString(16),
                a => a.Score.Metric(),
                a => a.Level.Metric(),
                a => a.Team.RemoveLines().RemoveString(10),
                a => a.Nickname.RemoveLines().RemoveString(16)).ToMarkdown();

            string content1 = string.Join("\n", players.Select(a => $"**{players.IndexOf(a).ToRegularCounter()}** <@{a.Id.Remove("discord:")}>"));

            var map = Map == Map.Enable ? GraphicService.GetMap(players) : null;


            var embeds = map != null ?
                new Embed[] { content.ToEmbed(), content1.ToEmbed(imageUrl: "attachment://map.jpeg") } :
                new Embed[] { content.ToEmbed(), content1.ToEmbed() };

            var msg = map != null ?
                await FollowupWithFileAsync(map, "map.jpeg", Context.User.Id.ToUserMention(), embeds) :
                await FollowupAsync(Context.User.Id.ToUserMention(), embeds);
        }

        [RateLimit(10, 1, RateLimit.RateLimitType.User)]
        [SlashCommand("search", "search for a list of online players")]
        public async Task players(
            [Autocomplete(typeof(SharedAutocompleteHandler.OnlineServersAutocompleteHandler))] string server = "",
            [Summary("server-filter", "search in which servers")] FilterType filter = FilterType.All,
            SortType sort = SortType.Score,
            Map Map = Map.Disable,
            [MaxValue(50)] int take = 25,
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
                a => a.Parent.Label.RemoveString(16),
                a => a.Score.Metric(),
                a => a.Level.Metric(),
                a => a.Team.RemoveLines().RemoveString(10),
                a => a.Nickname.RemoveLines().RemoveString(16)).ToMarkdown();

            string content1 = this["total_servers:*", dyanstioProvider.OnlineServers.Count] + " | " +
                              this["total_players:*", dyanstioProvider.OnlinePlayers.Count] + "\n" +
                              this["page:*", $"{page}/{players.Count() / take}"].ToBold() + "\n" +
                              this["closes:*", 50.ToDiscordUnixTimeFromat()];

            var map = Map == Map.Enable ? GraphicService.GetMap(players1) : null;


            var embeds = map != null ?
                new Embed[] { content.ToEmbed(), "".ToEmbed(imageUrl: "attachment://map.jpeg") } :
                new Embed[] { content.ToEmbed() };

            var msg = map != null ?
                await FollowupWithFileAsync(map, "map.jpeg", Context.User.Id.ToUserMention(), embeds) :
                await FollowupAsync(Context.User.Id.ToUserMention(), embeds);
        }



    }
}
