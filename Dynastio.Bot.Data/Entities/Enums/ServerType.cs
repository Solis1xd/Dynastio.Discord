using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynastio.Data
{
    public enum FilterType
    {
        PrivateServer,
        PublicServer,
        All
    }
    public enum SortType
    {
        Score,
        Level,
        Team,
        Server,
        Nickname,
        Region,
    }

    public enum Map
    {
        Enable,
        Disable
    }
}
