﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace ExamTrainBot.Commands
{
    class RecordsCmd : Command
    {
        public override string Name => "/records";

        public override bool forAdmin => false;

        public async override void Execute(MessageEventArgs e)
        {
            User user = Program.GetCurrentUser(e);
            if (User.currenttest != 0)
            {
                List<User> orderedlist = Program.users.OrderBy(u => u.points[User.currenttest - 1]).ToList();
                string text = "Список лідерів за останнім тестом:\n";
                for (int i = 0; i < orderedlist.Count; i++)
                {
                    if (i >= 49)
                        break;
                    if (orderedlist[i].id == user.id)
                        text += $"<b>{orderedlist[i].name} - {orderedlist[i].points[User.currenttest - 1]}</b>\n";
                    else
                        text += $"{orderedlist[i].name} - {orderedlist[i].points[User.currenttest - 1]}\n";
                }
                await Program.bot.SendTextMessageAsync(user.id, text, ParseMode.Html);
            }
            else
            {
                await Program.bot.SendTextMessageAsync(user.id, "Ще не було проведено жодного тесту!");
            }
        }
    }
}
