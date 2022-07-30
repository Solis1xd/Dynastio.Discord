
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynastio.Data
{
    public class Program
    {
        public static void Log(string service, string text)
        {
            Console.WriteLine(DateTime.UtcNow.ToString("T") + " " + service.PadRight(20) + text);
        }
        public static bool IsDebug()
        {
#if DEBUG
            return true;
#else
                return false;
#endif
        }
    }
}
