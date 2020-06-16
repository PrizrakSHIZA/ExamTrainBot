using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Args;

namespace ExamTrainBot.Commands
{
    class Timercmd : Command
    {
        public override string Name => "/timer";

        public override bool forAdmin => false;

        public async override void Execute(MessageEventArgs e)
        {
            User user = Program.GetCurrentUser(e.Message.Chat.Id);
            await Program.bot.SendTextMessageAsync(user.id, $"Таймер встановлено на {Program.TestTime.Hour}:{Program.TestTime.Minute}");
        }
    }
}
