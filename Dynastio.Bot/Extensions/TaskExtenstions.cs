using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynastio.Bot
{
    public static class TaskExtenstions
    {
        public static void RunInBackground(this Task task, bool tryCatch = false)
        {
            _ = Task.Run(async () =>
            {
                if (tryCatch)
                    await task.Try();
                else await task;
            });
        }
        public static async Task<T> TryGet<T>(this Task<T> task)
        {
            try
            {
                return await task;
            }
            catch
            {
                return default(T);
            }
        }
        public static async Task<bool> Try(this Task task)
        {
            try
            {
                await task;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
