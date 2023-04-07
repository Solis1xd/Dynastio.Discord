using System;
using Dynastio.Bot;
using Dynastio.Net;
using Newtonsoft.Json;

namespace Dynastio.Test
{
    public class Program
    {

        static void Main(string[] args)
        {
            var configuration = File.ReadAllText(Dynastio.Bot.Program.FilePathConfigurationMain);
            string n = Encryption.Encrypt(configuration, "**");
            Console.WriteLine(n);
            Console.ReadKey();
        }
    }
}