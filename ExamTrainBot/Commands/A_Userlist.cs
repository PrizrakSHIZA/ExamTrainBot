using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace ExamTrainBot.Commands
{
    class A_UserList : Command
    {
        public override string Name => "/userlist";

        public override bool forAdmin => true;

        public async override void Execute(MessageEventArgs e)
        {
            string text = "";
            Program.users.Sort((x, y) => string.Compare(x.name, y.name));
            foreach (User user in Program.users)
            {
                //if admin
                if (user.isadmin)
                    text += "<b>A</b>";
                else
                    text += "a";
                //if subscriber
                if (user.subscriber)
                    text += " <b>S</b> ";
                else
                    text += " s ";
                text += user.name +" ";
                text += user.id + " ";
                text += user.date.ToString("dd.MM.yyyy") + "\n";
            }
            await Program.bot.SendTextMessageAsync(e.Message.Chat.Id, text, parseMode: ParseMode.Html);
        }
    }
}
