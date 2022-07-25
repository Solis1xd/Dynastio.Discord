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
    [Group("leaderboard", "dynast.io leaderboard")]
    public class Leaderboard : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        public UserService UserService { get; set; }
        [RateLimit(100)]
        [SlashCommand("me", "leaderboard me")]
        public async Task leaderboard_me(LeaderboardType leaderboard = LeaderboardType.Monthly, [Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account = "")
        {
            await DeferAsync();

            UserAccount selectedAccount = string.IsNullOrWhiteSpace(account)
               ? Context.BotUser.GetAccount()
               : Context.BotUser.GetAccount(int.Parse(account));

            var result = await Context.Dynastio.Database.GetUserSurroundingRankAsync(selectedAccount.Id);
            UserSurroundingRankRow userSurroundingRank = leaderboard switch
            {
                LeaderboardType.Monthly => result.Montly,
                LeaderboardType.Weekly => result.Weekly,
                LeaderboardType.Daily => result.Daily,
                _ => null
            };
            List<UserSurroundingRankRow> usersSurroundingRank = leaderboard switch
            {
                LeaderboardType.Monthly => result.UsersRankMontly,
                LeaderboardType.Weekly => result.UsersRankWeekly,
                LeaderboardType.Daily => result.UsersRankDaily,
                _ => null
            };

            var firstUser = await Context.Dynastio.Database.GetUserRanAsync(usersSurroundingRank.First().Id);
            int index = leaderboard switch
            {
                LeaderboardType.Monthly => firstUser.Monthly,
                LeaderboardType.Weekly => firstUser.Weekly,
                LeaderboardType.Daily => firstUser.Daily,
                _ => 0
            };

            string content = $"**Your rank is: {index + 5}**" +
                              usersSurroundingRank.ToStringTable(new[] { this["index"], this["score"], this["time"], this["nickname"] },
                a => $"{(usersSurroundingRank.IndexOf(a) + index).ToRegularCounter()}",
                a => $"{a.Score.Metric()}",
                a => a.CreatedAt.ToRelative(),
                a => $"{a.Nickname.RemoveLines()}").ToMarkdown();

            await FollowupAsync(Context.User.Id.ToUserMention(), embed: content.ToEmbed(this["leaderboard"] + " Me " + this[leaderboard.ToString().ToLower()]));
        }
        [RateLimit(3)]
        [SlashCommand("score", "leaderboard score")]
        public async Task LeaderboardScore(LeaderboardType leaderboard = LeaderboardType.Monthly)
        {
            await DeferAsync();

            var leaderboardContent = await Context.Dynastio.Database.GetScoreLeaderboardAsync();
            if (leaderboard == LeaderboardType.Monthly)
            {
                leaderboardContent = leaderboardContent.Skip((int)leaderboard * 10).Take(10).ToList();
            }
            else leaderboardContent = leaderboardContent.OrderByDescending(a => a.Score).ToList();

            string content = leaderboardContent.ToStringTable(new[] { "#", this["index"], this["score"], this["time"], this["nickname"] },
                a => leaderboardContent.IndexOf(a) < 5 ? "🏆" : "",
                a => $"{(leaderboardContent.IndexOf(a) + 1).ToRegularCounter()}",
                a => $"{a.Score.Metric()}",
                a => a.CreatedAt.ToRelative(),
                a => $"{a.Nickname.RemoveLines()}");
            await FollowupAsync(Context.User.Id.ToUserMention(), embed: content.ToMarkdown().ToEmbed(this["leaderboard"] + " " + this[leaderboard.ToString().ToLower()]));
        }
        [RateLimit(10)]
        [SlashCommand("coin", "leaderboard coin")]
        public async Task leaderboard_coin()
        {
            await DeferAsync();

            var coinboard = await Context.Dynastio.Database.GetCoinLeaderboardAsync();
            string content = coinboard.ToStringTable(new[] { "#", this["index"], this["coin"], this["nickname"] },
                 a => coinboard.IndexOf(a) < 5 ? "🏆" : "",
                 a => $"{(coinboard.IndexOf(a) + 1).ToRegularCounter()}",
                 a => $"{a.Coin.Metric()}",
                 a => $"{(a.Id.Contains("discord") ? this.Context.Client.GetUser(ulong.Parse(a.Id.Remove("discord:")))?.Username ?? "Discord User" : "Unknown")}");

            await FollowupAsync(Context.User.Id.ToUserMention(), embed: content.ToMarkdown().ToEmbed(this["leaderboard"] + " " + this["coin"]));
        }
        [RateLimit(10)]
        [SlashCommand("honor", "leaderboard honor")]
        public async Task leaderboard_honor()
        {
            await DeferAsync();

            var top10Honor = await UserService.Get10TopHonor();
            string content = top10Honor.ToStringTable(new[] { "#", this["index"], this["honor"], this["nickname"] },
                a => top10Honor.IndexOf(a) < 5 ? "🏆" : "",
                a => $"{(top10Honor.IndexOf(a) + 1).ToRegularCounter()}",
                a => $"{a.Honor.Metric()}",
                a => $"<@{a.Id}>");

            await FollowupAsync(Context.User.Id.ToUserMention(), embed: content.ToEmbed(this["leaderboard"] + " " + this["honor"]));
        }
    }
}
