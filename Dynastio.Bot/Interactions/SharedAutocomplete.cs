using Discord;
using Discord.Interactions;
using Dynastio.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dynastio.Bot
{
    public class SharedAutocompleteHandler
    {
        public class AccountAutocompleteHandler : AutocompleteHandler
        {
            public UserService UserService { get; set; }
            public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
            {
                List<AutocompleteResult> results;

                string match = autocompleteInteraction.Data.Current.Value.ToString();
                var all = autocompleteInteraction.Data.Options.Where(a => a.Name == "all").FirstOrDefault();
                var userParam = autocompleteInteraction.Data.Options.Where(a => a.Name == "user").FirstOrDefault();

                User user = (userParam == null || string.IsNullOrEmpty((string)userParam.Value))
                    ? (context as ICustomInteractionContext).BotUser
                    : await UserService.GetUserAsync(ulong.Parse((string)userParam.Value));


                results = (all == null || (bool)all.Value == false)
                 ? AutocompleteUtilities.Parse(user.Accounts.Where(a => a.Nickname.Contains(match)).ToList())
                 : new List<AutocompleteResult>() { new AutocompleteResult() { Name = "All", Value = "0" } };

                // max - 25 suggestions at a time (API limit)
                return await Task.FromResult(AutocompletionResult.FromSuccess(results.Take(25)));
            }
        }

        public class OnlineServersAutocompleteHandler : AutocompleteHandler
        {
            public DynastioClient Dynastio { get; set; }
            public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
            {
                List<AutocompleteResult> results = new();

                string match = autocompleteInteraction.Data.Current.Value.ToString();

                var provider_ = autocompleteInteraction.Data.Options.FirstOrDefault(a => a.Name == "provider");
                string provider = provider_ != null ? provider_.Value.ToString() : DynastioProviderType.Main.ToString();

                var serverFilter = autocompleteInteraction.Data.Options.FirstOrDefault(a => a.Name == "server-filter");
                FilterType serverFilter_ = serverFilter != null ? (FilterType)Enum.Parse(typeof(FilterType), (string)serverFilter.Value) : FilterType.All;

                var servers = Dynastio[provider].OnlineServers.Where(
                    a =>
                    serverFilter_ == FilterType.PrivateServer ? a.IsPrivate : !a.IsPrivate &&
                    a.Label.ToLower().Contains(match)).Take(25).ToList();

                foreach (var server in servers)
                {
                    results.Add(new AutocompleteResult()
                    {
                        Name = StringExtensions.RemoveString(server.Label, 98),
                        Value = server.Label.RemoveString(30, false)
                    });
                }
                // max - 25 suggestions at a time (API limit)
                return await Task.FromResult(AutocompletionResult.FromSuccess(results));
            }
        }
        public class OnlinePlayersAutocompleteHandler : AutocompleteHandler
        {
            public DynastioClient Dynastio { get; set; }
            public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
            {
                List<AutocompleteResult> results = new();

                string match = autocompleteInteraction.Data.Current.Value.ToString().ToLower();

                var provider_ = autocompleteInteraction.Data.Options.FirstOrDefault(a => a.Name == "provider");
                string provider = provider_ != null ? provider_.Value.ToString() : DynastioProviderType.Main.ToString();

                var server = autocompleteInteraction.Data.Options.FirstOrDefault(a => a.Name == "server");

                var players = Dynastio[provider].OnlineServers.Where(
                    a => server != null ? a.Label.ToLower().Contains((string)server.Value) : true).ToList()
                    .SelectMany(a => a.Players)
                    .Where(a => a.Nickname.ToLower().Contains(match)).Take(25).ToList();

                foreach (var player in players)
                {
                    results.Add(new AutocompleteResult()
                    {
                        Name = StringExtensions.RemoveString(player.Nickname, 16),
                        Value = player.UniqeId
                    });
                }
                // max - 25 suggestions at a time (API limit)
                return await Task.FromResult(AutocompletionResult.FromSuccess(results));
            }
        }

    }
}