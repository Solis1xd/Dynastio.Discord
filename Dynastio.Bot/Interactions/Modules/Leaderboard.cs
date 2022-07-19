using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Dynastio.Net;


namespace Dynastio.Bot.Interactions.SlashCommands
{
    [RequireContext(ContextType.Guild)]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    public class Leaderboard : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        public UserService UserService { get; set; }

        [SlashCommand("leaderboard", "dynast.io leaderboard")]
        [RateLimit(120, 6, RateLimit.RateLimitType.User)]

        public async Task LeaderboardScore(LeaderboardParamType leaderboard = LeaderboardParamType.All)
        {
            await DeferAsync();

            string content;
            switch (leaderboard)
            {
                case LeaderboardParamType.Honor:
                    var top10Honor = await UserService.Get10TopHonor();
                    content = top10Honor.ToStringTable(new[] { "#", this["index"], this["honor"], this["nickname"] },
                        a => top10Honor.IndexOf(a) < 5 ? "🏆" : "",
                        a => $"{(top10Honor.IndexOf(a) + 1).ToRegularCounter()}",
                        a => $"{a.Honor.Metric()}",
                        a => $"<@{a.Id}>");
                    break;
                case LeaderboardParamType.Coin:

                    var coinboard = await Context.Dynastio.Database.GetCoinLeaderboardAsync();
                    content = coinboard.ToStringTable(new[] { "#", this["index"], this["coin"], this["nickname"] },
                        a => coinboard.IndexOf(a) < 5 ? "🏆" : "",
                        a => $"{(coinboard.IndexOf(a) + 1).ToRegularCounter()}",
                        a => $"{a.Coin.Metric()}",
                        a => $"{(a.Id.Contains("discord") ? this.Context.Client.GetUser(ulong.Parse(a.Id.Remove("discord:")))?.Username ?? "Discord User" : "Unknown")}");
                    break;
                default:
                    var leaderboardContent = await Context.Dynastio.Database.GetScoreLeaderboardAsync();
                    if (leaderboard != LeaderboardParamType.All)
                    {
                        var leaderboardType = (LeaderboardType)Enum.Parse(typeof(LeaderboardType), leaderboard.ToString());
                        leaderboardContent = leaderboardContent.Skip((int)leaderboardType * 10).Take(10).ToList();
                    }
                    else leaderboardContent = leaderboardContent.OrderByDescending(a => a.Score).ToList();
                    content = leaderboardContent.ToStringTable(new[] { "#", this["index"], this["score"], this["time"], this["nickname"] },
                        a => leaderboardContent.IndexOf(a) < 5 ? "🏆" : "",
                        a => $"{(leaderboardContent.IndexOf(a) + 1).ToRegularCounter()}",
                        a => $"{a.Score.Metric()}",
                        a => a.CreatedAt.ToRelative(),
                        a => $"{a.Nickname.RemoveLines()}");
                    break;
            }

            await FollowupAsync(Context.User.Id.ToUserMention(), embed: content.ToMarkdown().ToEmbed(this["leaderboard"] + " " + this[leaderboard.ToString().ToLower()]));
        }
        public enum LeaderboardParamType
        {
            Coin,
            Honor,
            Daily,
            Monthly,
            Weekly,
            All
        }
    }
}
