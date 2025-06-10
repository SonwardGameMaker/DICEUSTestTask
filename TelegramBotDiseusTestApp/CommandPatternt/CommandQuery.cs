using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotDiseusTestApp.CommandPatternt.Command;

namespace TelegramBotDiseusTestApp.CommandPatternt
{
    internal class CommandQuery
    {
        private Chat _chat;
        private List<ICommand> _commands;

        public CommandQuery(Chat chat)
        { 
            _chat = chat; 
        }
        public CommandQuery(Chat chat, List<ICommand> commands) : this(chat)
        {
            _commands = commands;
        }

        public Chat Chat { get => _chat; }

        public bool InitQuery(List<ICommand> commands)
        {
            if (_commands == null && commands != null)
            {
                _commands = commands;
                return true;
            }

            return false;
        }
    }
}
