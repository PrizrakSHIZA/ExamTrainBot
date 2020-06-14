using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace ExamTrainBot.Commands
{
    class AdminCommand : Command
    {
        public override string Name => "/admin";

        public override bool forAdmin => false;

        public async override void Execute(MessageEventArgs e)
        {
            User currentuser = Program.users.Find(u => u.id == e.Message.Chat.Id);
            int index = e.Message.Text.IndexOf(" ");
            if (index == -1)
            {
                await Program.bot.SendTextMessageAsync(currentuser.id, "Використовуйте команду наступним чином: /admin 'пароль'");
            }
            else
            {
                if (e.Message.Text.Substring(index + 1) == Program.password)
                {
                    currentuser.isadmin = true;
                    await Program.bot.SendTextMessageAsync(currentuser.id, "Ви увыйшли як адміністратор!");
                    SaveSystem.Save();
                }
                else
                {
                    await Program.bot.SendTextMessageAsync(currentuser.id, "Невірні дані!");
                    Console.WriteLine($"Помилка входу як Адміністратор у {currentuser.name} з id {currentuser.id}");
                }
            }
        }
    }
}
