using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Dynastio.Net;
using Discord.WebSocket;
using Dynastio.Bot.Interactions.Modules.Shard;

namespace Dynastio.Bot.Interactions.Modules.Guild
{

    [EnabledInDm(false)]
    [RequireContext(ContextType.Guild)]
    [RequireBotPermission(ChannelPermission.AttachFiles | ChannelPermission.SendMessages)]
    [Group("player", "online players")]
    public class PlayerModule : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        public GraphicService GraphicService { get; set; }
        public DynastioClient Dynastio { get; set; }
        public UserService UserService { get; set; }

        [RateLimit(100, 6)]
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
        [RateLimit(100, 6)]
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
            await FollowupWithFileAsync(image, "chest.jpeg", $"player chest server: `{player_.Parent.Label}` player nickname: `{player_.Nickname}`");
        }

        [RateLimit(10, 2, RateLimit.RateLimitType.User)]
        [SlashCommand("find", "find a discord user in the game")]
        public async Task Find(IUser user,
            DynastioProviderType provider = DynastioProviderType.Main)
        {
            await DeferAsync();
            var botUser = await UserService.GetUserAsync(user.Id, false);
            if (botUser is null || botUser.Accounts == null || botUser.Accounts.Count == 0)
            {
                await FollowupAsync(embed: "No any account of this user found.".ToWarnEmbed("Not Found !", user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl()));
                return;
            }
            var accounts = botUser.Accounts.Select(a => a.Id).ToList();

            var result = Dynastio[provider].OnlinePlayers.FirstOrDefault(a => accounts.Contains(a.Id));
            if (result == null)
            {
                await FollowupAsync(embed: "No any online account of this user found.".ToWarnEmbed($"{user.Username} is offline !", user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl()));
                return;
            }

            var team = Dynastio[provider].OnlinePlayers.GroupBy(a => a.Team).FirstOrDefault(a => a.Key == result.Team && !string.IsNullOrEmpty(a.Key));

            var teammates = team is null ? "`none`" : string.Join(", ", team.Select(a => a.Nickname));
            string content =
                $"**Nickname:** {result.Nickname.TrySubstring(18)}\n" +
                $"**Level:** {result.Level}\n" +
                $"**Score:** {result.Score.Metric()}\n" +
                $"**Server:** {result.Parent.Label.TrySubstring(20)}\n" +
                $"**Team:** {result.Team}\n" +
                $"**Teammates:**: {teammates.ToMarkdown()}";

            await FollowupAsync(embed: content.ToEmbed(user.Username + "(Online Player)", user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl()));
        }

    }
}
