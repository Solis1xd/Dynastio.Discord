using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynastio.Bot
{

    public class InteractionUtilities
    {
        public const string Perfix = "$";

        public static bool IsStaticInteractionCommand(SocketInteraction interaction)
        {
            if (interaction is SocketMessageComponent or SocketModal)
            {
                if (object.Equals((interaction as SocketMessageComponent).Data.CustomId.Substring(0, 1), Perfix))
                    return false;
            }
            return true;
        }
    }
}
