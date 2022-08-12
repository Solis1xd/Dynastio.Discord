﻿using Google.Apis.YouTube.v3;
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
            if (apiKey == null || mainChannelId == null)
            {
                Program.Log("YoutubeService", "api key not found.");
                return;
            }
            Program.IsYoutubeServiceInitialized = true;

            youtube = new YouTubeService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                ApiKey = apiKey
            });
            this.mainChannelId = mainChannelId;
        }
        public async Task InitializeAsync()
        {
            Program.Log("YoutubeService", "Initializing..");

            if (!Program.IsYoutubeServiceInitialized)
            {
                Program.Log("YoutubeService", "Not Initialized.");
                return;
            }

            Program.Log("YoutubeService", "Get channel videos..");
            Videos = await GetAllChannelVideos(mainChannelId);

            Program.Log("YoutubeService", "Initialized.");
        }
        public Task<List<SearchResult>> GetAllChannelVideos(string channelId)
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
        public Task<List<SearchResult>> SearchVideoByKeyword(string keyword)
        {
            var searchListRequest = youtube.Search.List("snippet");
            searchListRequest.MaxResults = 50;
            searchListRequest.Q = keyword;
            searchListRequest.Type = "video";

            var searchListResponse = searchListRequest.Execute();
            return Task.FromResult(searchListResponse.Items.ToList());
        }

    }
}
