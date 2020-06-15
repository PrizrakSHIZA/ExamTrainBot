using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Telegram.Bot.Args;

namespace ExamTrainBot.Commands
{
    class SetTimer : Command
    {
        public override string Name => "/timer";

        public override bool forAdmin => true;

        public override void Execute(MessageEventArgs e)
        {
        }
    }
}
