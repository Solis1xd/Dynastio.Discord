using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Dynastio.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynastio.Bot.Interactions.Modules.Dynastio
{
    [RequireContext(ContextType.Guild)]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    [Group("accounts", "your dynast.io accounts settings")]
    public class AccountsModule : CustomInteractionModuleBase<CustomSocketInteractionContext>
    {
        public UserService userManager { get; set; }
        public DynastioClient Dynastio { get; set; }

        [RateLimit(10)]
        [SlashCommand("list", "dynastio accounts")]
        public async Task list()
        {
            await DeferAsync();

            var message = await FollowupAsync(Context.User.Id.ToUserMention(),
                embed: new EmbedBuilder()
                {
                    Title = this["accounts.account.title"],
                    Description = this["accounts.account.description"] + "\n" +
                                  ((Context.BotUser.Accounts?.ToStringTable(new string[] { "#", this["account"], this["added_at"] },
                                  a => Context.BotUser.Accounts.IndexOf(a) + 1,
                                  a => a.Nickname,
                                  a => a.AddedAt.ToRelative()) + "                 ")
                                  .ToMarkdown() ?? this["no_account_found"].ToMarkdown()),
                    Color = Color.Orange,
                    ThumbnailUrl = "https://cdn.discordapp.com/attachments/916998929609023509/956252644551323658/logo.jpg",
                    Url = "https://www.youtube.com/channel/UCW0PmC1B8jjhpKLHciFp0xA/?sub_confirmation=1"
                }.Build());
        }
        [RateLimit(10)]
        [RequireUserDynastioAccount]
        [SlashCommand("set-default", "set an account as default account")]
        public async Task setDefault([Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account)
        {
            await DeferAsync();

            UserAccount account_ = string.IsNullOrWhiteSpace(account) ? Context.BotUser.GetAccount() : Context.BotUser.GetAccount(int.Parse(account));
            Context.BotUser.SwitchDefault(account_);
            await Context.BotUser.UpdateAsync();
            await FollowupAsync(Context.User.Id.ToUserMention(), embed: this["account_changed"].ToSuccessfulEmbed(this["account_changed"]));
        }
        [RequireUserDynastioAccount]
        [RateLimit(5)]
        [SlashCommand("edit", "edit your added account")]
        public async Task edit([Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account, string newNickname)
        {
            await DeferAsync();

            UserAccount account_ = Context.BotUser.GetAccount(int.Parse(account));

            account_.Nickname = newNickname;

            Context.BotUser.ReplaceAccount(account_.Id, account_);

            await Context.BotUser.UpdateAsync();

            await FollowupAsync(Context.User.Id.ToUserMention(), embed: this["account_changed"].ToSuccessfulEmbed(this["account_changed"]));
        }
        [RequireUserDynastioAccount]
        [RateLimit(3)]
        [SlashCommand("remove", "remove a connected account")]
        public async Task remove([Autocomplete(typeof(SharedAutocompleteHandler.AccountAutocompleteHandler))] string account)
        {
            await DeferAsync();

            UserAccount account_ = Context.BotUser.GetAccount(int.Parse(account));

            Context.BotUser.RemoveAccount(account_);

            await Context.BotUser.UpdateAsync();

            await FollowupAsync(Context.User.Id.ToUserMention(), embed: this["account_removed"].ToSuccessfulEmbed("account_removed"));
        }
        [RateLimit(10)]
        [SlashCommand("add", "connect new account to the bot")]
        public async Task addaccount()
        {
            var modal = new ModalBuilder(this["add_account"], $"accounts add")
               .AddTextInput(new TextInputBuilder(this["nickname"], "nickname", TextInputStyle.Short, this["custom_nickname"], 1, 16, true, null))
               .AddTextInput(new TextInputBuilder(this["coin"], "coin", TextInputStyle.Short, this["account_coins_number"], 1, 16, true, null))
               .AddTextInput(new TextInputBuilder(this["account_id"], "id", TextInputStyle.Short, "google:00000000000000000000", 1, 150, true, null))
               .Build();

            await RespondWithModalAsync(modal);
        }
        [ModalInteraction("accounts add", true)]
        public async Task add(AddAccountForm form)
        {
            await DeferAsync();

            if (!int.TryParse(form.Coins, out int value))
            {
                await FollowupAsync(Context.UserMention(), embed: this["wrong_coin"].ToDangerEmbed(this["unauthorized"]));
                return;
            }
            int coins = value;
            string id = form.Id.Trim().Remove("id:", "Id:", "ID:"); // don't use tolower
            string nickname = form.Nickname.Trim();

            if (id.Contains("discord"))
            {
                if (!id.Contains(Context.User.Id.ToString()))
                {
                    await FollowupAsync(Context.UserMention(), embed: this["can_add_your_account_only"].ToWarnEmbed(this["unauthorized"]));
                    return;
                }
            }
            if (Context.BotUser.Accounts.Count >= 20)
            {
                await FollowupAsync(Context.UserMention(), embed: this["add_account_limits_20_account.description"].ToWarnEmbed(this["add_account_limits_20_account.title"]));
                return;
            }
            if (Context.BotUser.GetAccount(id) != null)
            {
                await FollowupAsync(Context.UserMention(), embed: this["account_added_already.description"].ToWarnEmbed(this["account_added_already.title"]));
                return;
            }

            Profile acc = await Dynastio.Main.GetUserProfileAsync(id).TryGet();
            if (acc is null)
            {
                await FollowupAsync(Context.UserMention(), embed: this["no_account_found"].ToWarnEmbed(this["no_account_found"]));
                return;
            }
            if (coins != acc.Coins)
            {
                await FollowupAsync(Context.UserMention(), embed: this["wrong_coin"].ToDangerEmbed(this["unauthorized"]));
                return;
            }
            var account = new UserAccount()
            {
                Id = id,
                Nickname = nickname,
                AddedAt = DateTime.UtcNow,
            };
            Context.BotUser.AddAccount(account);

            await Context.BotUser.UpdateAsync();

            await FollowupAsync(Context.UserMention(), embed: this["account_added"].ToSuccessfulEmbed(this["account_added.title"]));

        }
    }

}
