using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotDiseusTestApp.DTOs;

namespace TelegramBotDiseusTestApp
{
    internal static class Utils
    {
        public static AppSettingData Configure(string jsonDirectoryPath, string jsonName)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(jsonName, optional: false, reloadOnChange: true)
            .Build();

            AppSettingData result = new AppSettingData
            { 
                BotUrl = configuration[""]
            };

        }

    }
}
