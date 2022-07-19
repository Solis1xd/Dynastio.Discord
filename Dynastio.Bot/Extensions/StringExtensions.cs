using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynastio.Bot
{
    public static class StringExtensions
    {
        public static string Bold(this string value)
        {
            return $"**{value}**";
        }
        public static string Join(this IEnumerable<string> value, string spearator)
        {
            return string.Join(spearator, value);
        }
        public static string Remove(this string value, params string[] txt)
        {
            foreach (var x in txt)
                value = value.Replace(x, "");
            return value;
        }
        public static EmbedBuilder ToEmbedBuilder(this string Value, string Title = null, string ThumbnailUrl = null, string ImageUrl = null, Color color = default)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.Description = Value;
            builder.Title = Title;
            builder.ThumbnailUrl = ThumbnailUrl;
            builder.ImageUrl = ImageUrl;
            builder.Color = color;
            return builder;
        }
        public static Embed ToEmbed(this string Value, string Title = null, string ThumbnailUrl = null, string ImageUrl = null)
        {
            return Value.ToEmbedBuilder(Title, ThumbnailUrl, ImageUrl).Build();
        }
        public static Embed ToSuccessfulEmbed(this string Value, string Title = null, string ThumbnailUrl = null, string ImageUrl = null)
        {
            return Value.ToEmbedBuilder(Title, ThumbnailUrl, ImageUrl, Color.Green).Build();
        }
        public static Embed ToWarnEmbed(this string Value, string Title = null, string ThumbnailUrl = null, string ImageUrl = null)
        {
            return Value.ToEmbedBuilder(Title, ThumbnailUrl, ImageUrl, Color.Orange).Build();
        }
        public static Embed ToDangerEmbed(this string Value, string Title = null, string ThumbnailUrl = null, string ImageUrl = null)
        {
            return Value.ToEmbedBuilder(Title, ThumbnailUrl, ImageUrl, Color.Red).Build();
        }
        public static string ToMarkdown(this string Value) => $"```{Value}```";
        public static string ResourcesPath(this string path)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), $@"Resources/{path}");
        }
    }
}
