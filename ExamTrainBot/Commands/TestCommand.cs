using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace ExamTrainBot.Commands
{
    class TestCommand : Command
    {
        public override string Name => "/test";

        public override bool forAdmin => false;
        public async override void Execute(MessageEventArgs e)
        {
            User currentuser = Program.GetCurrentUser(e);
            await Program.bot.SendTextMessageAsync(currentuser.id, Program.useTimer.ToString());
            /*
            currentuser.ontest = true;
            currentuser.currentquestion = 0;
            await Program.bot.SendTextMessageAsync(currentuser.id, Program.testlist[User.currenttest].Text);
            Program.testlist[User.currenttest].questions[0].Ask(currentuser.id);*/
        }
    }
}
