using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Dynastio.Bot
{
    public static class UserMessageExtensions
    {
        public static async Task<IUserMessage> ModifyToEmbed(this IUserMessage Message,string text, Embed Embed, MessageComponent Component = null, Embed[] Embeds = null)
        {
            await Message.ModifyAsync(x =>
            {
                x.Content = Message.MentionedUserIds.FirstOrDefault().ToUserMention() + "\n" + text;
                x.Embed = Embed;
                x.Embeds = Embeds ?? new Embed[] { };
                x.Attachments = new Optional<IEnumerable<FileAttachment>>();
                x.Components = Component ?? new ComponentBuilder().Build();
            });
            return Message;
        }
        public static async Task<IUserMessage> ModifyToEmbed(this IUserMessage Message, Embed Embed, MessageComponent Component = null, Embed[] Embeds = null)
        {
            await Message.ModifyAsync(x =>
            {
                x.Content = Message.MentionedUserIds.FirstOrDefault().ToUserMention();
                x.Embed = Embed;
                x.Embeds = Embeds ?? new Embed[] { };
                x.Attachments = new Optional<IEnumerable<FileAttachment>>();
                x.Components = Component ?? new ComponentBuilder().Build();
            });
            return Message;
        }
        public static async Task WhenNoResponse(this IUserMessage message, CustomSocketInteractionContext context, TimeSpan timeout, Action<IUserMessage> action)
        {
            var userResponse = await context.ReadMessageComponentFromMessageAsync(message, timeout);
            if (userResponse is null)
            {
                action.Invoke(message);
            }
        }
    }
}
