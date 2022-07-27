using Discord;
using Discord.Interactions;
using Dynastio.Net;
using System;
using System.Threading.Tasks;

using Discord.WebSocket;
using Dynastio.Bot.Extensions.Discord;
using System.Net;
using SixLabors.ImageSharp;
using Color = Discord.Color;
using System.Linq;
using Dynastio.Bot.Extensions;

namespace Dynastio.Bot.Interactions.SlashCommands
{

    [RequireUsageTime(50)]
    [RequireContext(ContextType.Guild)]
    [RequireBotPermission(ChannelPermission.AttachFiles)]
    [RequireBotPermission(ChannelPermission.EmbedLinks)]
    [RequireBotPermission(ChannelPermission.UseExternalEmojis)]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    public class MenuModule : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        public UserService UserService { get; set; }
        public GraphicService GraphicService { get; set; }
        public Configuration Configuration { get; set; }
        public MongoService MongoService { get; set; }

        [SlashCommand("menu", "dynastio menu")]
        public async Task Menu_menu()
        {
            await ReturnToMenu(true);
        }

        [RequireUser]
        [ComponentInteraction("menu.menu:*")]
        public async Task ReturnToMenu(bool isInteraction = false)
        {
            if (isInteraction) await DeferAsync();

            var componenets = new ComponentBuilder();

            componenets.WithButton(this["online_players"], "menu.players:score:default:default:1:24", ButtonStyle.Primary, new Emoji("⚔"), null, false, 0);
            componenets.WithButton(this["player_infomation"], "menu.playerinfomation", ButtonStyle.Primary, new Emoji("🔎"), null, false, 0);

            componenets.WithButton(this["profile"], "menu.profile:user", ButtonStyle.Primary, null, null, false, 1);
            componenets.WithButton(this["personal_chest"], "menu.chest:user", ButtonStyle.Primary, Emote.Parse("<:privatechest:987097864935178270>"), null, false, 1);
            componenets.WithButton(this["stat"], "menu.stat:user", ButtonStyle.Secondary, null, null, false, 1);
            componenets.WithButton(this["rank"], "menu.rank:user", ButtonStyle.Success, new Emoji("🏁"), null, false, 1);

            componenets.WithButton(this["leaderboard"], "menu.leaderboard", ButtonStyle.Success, new Emoji("👑"), null, false, 2);
            componenets.WithButton(this["events"], "menu.events", ButtonStyle.Secondary, null, null, false, 2);
            componenets.WithButton(this["changes"], "menu.changelog:false:1:txt", ButtonStyle.Secondary, new Emoji("📰"), null, false, 2);

            componenets.WithButton(this["youtube_channel"], null, ButtonStyle.Link, new Emoji("📺"), Configuration.YoutubeLink, false, 3);
            componenets.WithButton(this["cancel"], $"shared.close", ButtonStyle.Danger, new Emoji("🛠"), null, false, 3);

            var embed = new EmbedBuilder()
            {
                Title = this["menu_embed_title"],
                Description = this["menu.embed.description"] + "     \n\n\n" +
                              this["closes:*", 50.ToDiscordUnixTimeFromat()],
                Color = Color.Orange,
                ThumbnailUrl = "https://cdn.discordapp.com/attachments/916998929609023509/956252644551323658/logo.jpg",
                Url = "https://www.youtube.com/channel/UCW0PmC1B8jjhpKLHciFp0xA/?sub_confirmation=1"
            }.Build();

            var message = isInteraction ? await FollowupAsync(Context.User.Id.ToUserMention(), embed: embed, components: componenets.Build()) : await (Context.Interaction as SocketMessageComponent).Message.ModifyToEmbed(embed, componenets.Build());

            await message.WhenNoResponse(Context, TimeSpan.FromSeconds(50), async (x) => { await ModifyToClosed(x); });
        }
        [RequireUser]
        [ComponentInteraction("menu.playerinfomation")]
        public async Task PlayerInfor()
        {
            await DeferAsync();
            await (Context.Interaction as SocketMessageComponent).Message.ModifyToEmbed("This command changed to /toplist".ToEmbed("use /toplist"));
        }
       
        [RequireUser]
        [ComponentInteraction("menu.players:*:*:*:*:*")]
        public async Task Players(string sort = "score", string filter = "default", string Query = "default", int Page = 1, int Take = 24)
        {
            await DeferAsync();
            await (Context.Interaction as SocketMessageComponent).Message.ModifyToEmbed("This command changed to /toplist".ToEmbed("use /toplist"));
        }
        [RequireUser]
        [ComponentInteraction("menu.search:*:*:*:*:*")]
        public async Task Search(string sort = "score", string filter = "default", string Query = "default", int Page = 1, int Take = 24)
            => await Players(sort, filter, Query, 0, Take);
        [RequireUser]
        [ComponentInteraction("menu.player:*")]
        public async Task Player(string id = "player")
        {
            await DeferAsync();
            await (Context.Interaction as SocketMessageComponent).Message.ModifyToEmbed("This command changed to /toplist".ToEmbed("use /toplist"));
        }


        [ModalInteraction("$test")]
        public async Task leaderboard(SearchForm from)
        {
            await DeferAsync();
            await (Context.Interaction as SocketMessageComponent).Message.ModifyToEmbed("This command changed to /profile".ToEmbed("use /profile"));
        }
        [RequireUser]
        [RequireUserDynastioAccount]
        [ComponentInteraction("menu.profile:*")]
        public async Task Profile(string value)
        {
            await DeferAsync();
            await (Context.Interaction as SocketMessageComponent).Message.ModifyToEmbed("This command changed to /profile".ToEmbed("use /profile"));
        }
        [RequireUser]
        [RequireUserDynastioAccount()]
        [ComponentInteraction("menu.chest:*")]
        public async Task Chest(string value)
        {
            await DeferAsync();
            await (Context.Interaction as SocketMessageComponent).Message.ModifyToEmbed("This command changed to /personal chest".ToEmbed("use /personal chest"));
        }

        [RequireUser]
        [RequireUserDynastioAccount()]
        [ComponentInteraction("menu.stat:*")]
        public async Task Stat(string value)
        {
            await DeferAsync();
            await (Context.Interaction as SocketMessageComponent).Message.ModifyToEmbed("This command changed to /stat".ToEmbed("use /stat"));
        }
        [RequireUser]
        [RequireUserDynastioAccount] 
        [ComponentInteraction("menu.rank:*")]
        public async Task Rank(string value)
        {
            await DeferAsync();
            await (Context.Interaction as SocketMessageComponent).Message.ModifyToEmbed("This command changed to /rank".ToEmbed("use /rank"));
        }
        [RequireUser]
        [ComponentInteraction("menu.leaderboard")]
        public async Task LeaderboardScore()
        {
            await DeferAsync();
            await (Context.Interaction as SocketMessageComponent).Message.ModifyToEmbed("This command changed to /leaderboard".ToEmbed("use /leaderboard"));
        }
        [RequireUser]
        [ComponentInteraction("menu.events")]
        public async Task Events()
        {
            await DeferAsync();
            await (Context.Interaction as SocketMessageComponent).Message.ModifyToEmbed("This command changed to /events".ToEmbed("use /events"));
        }

        [RequireUser]
        [ComponentInteraction("menu.changelog:*:*:*")]
        public async Task changelog(bool search, int page, string txt = "txt")
        {
            await DeferAsync();
            await (Context.Interaction as SocketMessageComponent).Message.ModifyToEmbed("This command changed to /changelog".ToEmbed("use /changelog"));
        }

    }
}