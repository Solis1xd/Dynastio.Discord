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
            public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
            {
                List<AutocompleteResult> results;

                string match = autocompleteInteraction.Data.Current.Value.ToString();
                var all = autocompleteInteraction.Data.Options.Where(a => a.Name == "all").FirstOrDefault();

                results = (all == null || (bool)all.Value == false) ?
                AutocompleteUtilities.Parse((context as ICustomInteractionContext).BotUser.Accounts.Where(a => a.Nickname.Contains(match)).ToList()) :
                new List<AutocompleteResult>() { new AutocompleteResult() { Name = "All", Value = "0" } };

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

                var servers = Dynastio[provider].OnlineServers.Where(a => a.Label.ToLower().Contains(match)).Take(25).ToList();

                foreach (var server in servers)
                {
                    results.Add(new AutocompleteResult()
                    {
                        Name = server.Label.Summarizing(20),
                        Value = server.Label.Summarizing(60, false)
                    });
                }
                // max - 25 suggestions at a time (API limit)
                return await Task.FromResult(AutocompletionResult.FromSuccess(results));
            }
        }
    }

}
