using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBotDiseusTestApp.CommandPatternt
{
    internal class CommandManager
    {
        private int _maxQueryNumber;
        private List<CommandQuery> _queries;

        public CommandManager(int maxQueryNumber)
        {
            _maxQueryNumber = maxQueryNumber;
            _queries = new List<CommandQuery>();
        }

        public async void ProceedCommand(Message message)
        {
            CommandQuery currentQuery = _queries.FirstOrDefault(q => q.Chat.Id == message.Chat.Id);
            if (currentQuery == null) currentQuery = AddQuery(message);
            if (currentQuery == null) return; // refactor it later

            // TODO
        }

        private CommandQuery AddQuery(Message message)
        {
            if (_queries.Count < _maxQueryNumber && _queries.Find(q => q.Chat == message.Chat) == null)
            {
                var newQuery = new CommandQuery(message.Chat);
                _queries.Add(newQuery);
                return newQuery;
            }

            return null;
        }
    }
}
