using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynastio.Bot
{
    public class YoutubeService
    {
        private readonly YouTubeService youtube;
        private readonly string mainChannelId;
        public List<SearchResult> Videos { get; set; }
        public YoutubeService(string apiKey, string mainChannelId)
        {
            if (apiKey == null || mainChannelId == null) return;
            Program.IsYoutubeServiceInitialized = true;

            youtube = new YouTubeService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                ApiKey = apiKey
            });
            this.mainChannelId = mainChannelId;
        }
        public async Task InitializeAsync()
        {
            if (!Program.IsYoutubeServiceInitialized)
                return;

                Videos = await GetChannelVideos(mainChannelId);
        }
        public Task<List<SearchResult>> GetChannelVideos(string channelId)
        {
            List<SearchResult> res = new List<SearchResult>();

            string nextpagetoken = " ";

            while (nextpagetoken != null)
            {
                var searchListRequest = youtube.Search.List("snippet");
                searchListRequest.MaxResults = 50;
                searchListRequest.ChannelId = channelId;
                searchListRequest.PageToken = nextpagetoken;
                searchListRequest.Type = "video";

                // Call the search.list method to retrieve results matching the specified query term.
                var searchListResponse = searchListRequest.Execute();

                // Process  the video responses 
                res.AddRange(searchListResponse.Items);

                nextpagetoken = searchListResponse.NextPageToken;

            }
            return Task.FromResult(res);
        }
    }
}
