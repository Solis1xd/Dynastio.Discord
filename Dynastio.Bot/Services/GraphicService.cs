using Dynastio.Net;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;


namespace Dynastio.Bot
{
    public class GraphicService
    {
        public GraphicService()
        {

        }
        public GraphicService Initialize()
        {
            Program.Log("GraphicService", "Initializing");

            FontCollection collection = new FontCollection();
            fontFamily = collection.Add("Fonts/FiraSans-Bold.ttf".ResourcesPath());
            font = fontFamily.CreateFont(12, FontStyle.Bold);

            Program.Log("GraphicService", "Initializied");

            return this;
        }
        public Font font { get; set; }
        public FontFamily fontFamily { get; set; }
        public Image GetStat<T>(Dictionary<T, int> value)
        {
            Image image = Image.Load($@"Images/stat.png".ResourcesPath());

            int x = 0;
            int y = 12;
            int index = 0;
            foreach (var v in value.OrderByDescending(a => a.Value))
            {
                var item = (T)(object)v.Key;

                int X = (x * 180) + 10;
                image.Mutate(x => x.DrawText(item.ToString() + " x" + v.Value, new Font(fontFamily, 14, FontStyle.Regular), Color.White, new Point(X + 30, y + 5)));

                var imgPath = $"Images/{(item is ItemType ? "Inventory" : "Entities")}/{item.ToString().ToLower()}.png".ResourcesPath();
                if (!File.Exists(imgPath)) imgPath = $"Images/unknown.png".ResourcesPath();

                using (Image img = Image.Load(path: imgPath))
                {
                    img.Mutate(x => x.Resize(25, 25));
                    var point = new Point(X, y);
                    image.Mutate(x => x.DrawImage(img, point, 1f));
                }
                y += 23;
                index++;
                if (index >= 28)
                {
                    x++;
                    index = 0;
                    y = 12;
                }
            }

            return image;
        }
        public Image GetMap(List<Player> players)
        {
            Image image = Image.Load($@"Images/map.png".ResourcesPath());
            List<PointF> points = new List<PointF>();
            foreach (var p in players.ToList())
            {
                try
                {
                    p.X = p.X < 0 ? p.X = 0 : (p.X > 500 ? p.X = 500 : p.X);
                    p.Y = p.Y < 0 ? p.Y = 0 : (p.Y > 500 ? p.Y = 500 : p.Y);
                    var lpoint = new PointF() { X = p.X, Y = p.Y };
                    image.Mutate(x => x.DrawText("*", new Font(fontFamily, 15, FontStyle.Bold), Color.White, lpoint));

                    //name
                    p.Y -= 10;

                    if (p.Y > 450) p.Y = 450;
                    if (p.Y < 30) p.Y = 30;

                    if (p.X > 450) p.X = 450;
                    if (p.X < 30) p.X = 30;

                    var point = new PointF() { X = p.X, Y = p.Y };
                    points.Add(point);
                    image.Mutate(x => x.DrawText(p.Nickname.Summarizing(16).RemoveLines(), new Font(fontFamily, 11, FontStyle.Regular), Color.Orange, point));
                }
                catch { }
            }

            return image;
        }
        public Image GetLeaderboard(Leaderboardscore[] list, LeaderboardType type)
        {
            Image image = Image.Load($@"Images/Leaderboard/{type.ToString().ToLower()}/default.png".ResourcesPath());
            for (int i = 0; i < list.Length; i++)
            {
                //info
                var point = new PointF() { X = 106, Y = 106 + (i * 33) };
                image.Mutate(
                    x =>
                    x.DrawText(list[i].Nickname.RemoveLines(),
                    new Font(fontFamily, 23, FontStyle.Bold),
                    Color.White, point));

                //score
                var pointT = new PointF() { X = 340, Y = 106 + (i * 33) };
                image.Mutate(x => x.DrawText(list[i].Score.Metric(), new Font(fontFamily, 25, FontStyle.Regular), Color.White, pointT));
            }
            return image;
        }
        public Image GetProfile(Profile profile)
        {
            Image image = Image.Load($@"Images/Profile/default.png".ResourcesPath());

            var pointCoinSection = new PointF() { X = (405 - (16 * profile.Coins.ToString().Length)), Y = 28 };
            image.Mutate(x => x.DrawText(profile.Coins.ToString(), new Font(fontFamily, 30, FontStyle.Regular), Color.White, pointCoinSection));

            var TextLevelPoint = new PointF() { X = (72 - (10 * profile.Details.Level.ToString().Length)), Y = 46 };
            image.Mutate(x => x.DrawText(profile.Details.Level.ToString(), new Font(fontFamily, 40, FontStyle.Bold), Color.WhiteSmoke, TextLevelPoint));

            var pointDetailsSection = new PointF() { X = 14, Y = 163 };
            string TextDetails = string.Format("{0} was playing in {1}", profile.LastActiveAt.ToRelative(), profile.LatestServer.Summarizing(16));
            image.Mutate(x => x.DrawText(TextDetails, new Font(fontFamily, 16, FontStyle.Bold), Color.WhiteSmoke, pointDetailsSection));

            var ExperienceLine = new Pen(Color.WhiteSmoke, 10);
            image.Mutate(x => x.DrawLines(ExperienceLine, new Point(165, 144), new Point((int)(profile.GetExperience(321) + 162), 144)));

            for (int x = 0; x < profile.Badges.Count; x++)
            {
                var imgPath = $"Images/Badges/{profile.Badges[x].ToString().ToLower()}.png".ResourcesPath();
                if (!File.Exists(imgPath)) imgPath = $"Images/unknown.png".ResourcesPath();

                using (Image img = Image.Load(path: imgPath))
                {
                    img.Mutate(x => x.Resize(32, 32));
                    var point = new Point((int)(177 + (x * 44)), 94);
                    image.Mutate(x => x.DrawImage(img, point, 1f));
                }
            }
            return image;

        }
        public Image GetRank(Image avatar, UserRank rank)
        {
            Image image = Image.Load($@"Images/rank.png".ResourcesPath());

            avatar.Mutate(x => x.Resize(160, 160));
            image.Mutate(x => x.DrawImage(avatar, new Point(14, 20), 1f));

            var pointRank = new PointF() { X = 200, Y = 22 };
            image.Mutate(x => x.DrawText("# " + rank.Monthly.ToString(), new Font(fontFamily, 50, FontStyle.Bold), Color.Orange, pointRank));

            var pointDailyRank = new PointF() { X = 190, Y = 90 };
            image.Mutate(x => x.DrawText($"weekly #{rank.Weekly} | daily#{rank.Daily} ", new Font(fontFamily, 17, FontStyle.Regular), Color.White, pointDailyRank));


            return image;

        }
        public Image GetPersonalChests(params Personalchest[] personalchests)
        {
            var chestCount = personalchests.Count();

            int width = chestCount > 4 ? 1250 : chestCount * 250;
            int high = (int)Math.Round((decimal)chestCount / 5, MidpointRounding.ToPositiveInfinity) * 250;

            var image = new Image<Rgba32>(width, high);

            image.Mutate(x => x.BackgroundColor(Color.ParseHex("#36393f")));
            for (int y = 1; y <= 5; y++)
            {
                if (y - 1 * 4 > personalchests.Length) break;

                for (int x = 1; x <= 5; x++)
                {
                    var index = ((y - 1) * 5) + (x - 1);
                    if (index > personalchests.Length - 1) break;

                    using (Image img = GetPersonalChest(personalchests[index]))
                    {
                        img.Mutate(x => x.Resize(250, 250));
                        var point = new Point((x - 1) * 250, (y - 1) * 250);
                        image.Mutate(x => x.DrawImage(img, point, 1f));
                    }
                }
            }
            return image;
        }
        public Image GetPersonalChest(Personalchest personalchest)
        {

            Image image = Image.Load($@"Images/PersonalChest/default.png".ResourcesPath());
            if (personalchest == null) return image;

            var items = personalchest.GetAsDictionary();

            const int size = 5;
            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    var slotIndex = y * size + x;
                    if (items.ContainsKey(slotIndex))
                    {
                        var imgPath = $"Images/Inventory/{items[slotIndex].ItemType.ToString().ToLower()}.png".ResourcesPath();
                        if (!File.Exists(imgPath)) imgPath = $"Images/unknown.png".ResourcesPath();

                        using (Image img = Image.Load(path: imgPath))
                        {
                            img.Mutate(x => x.Resize(100, 100));
                            var point = new Point((28 + (x * 117)), (28 + (y * 117)));
                            image.Mutate(x => x.DrawImage(img, point, 1f));
                        }
                        var pointCount = new Point((28 + (x * 117)), (28 + (y * 117)));

                        var color = Color.White;

                        image.Mutate(x => x.DrawText(items[slotIndex].Count.ToString(), new Font(fontFamily, 24, FontStyle.Bold), color, pointCount));
                    }
                }
            }
            return image;

        }



    }
}
