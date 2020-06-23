using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace ExamTrainBot.Commands
{
    class AdminCmd : Command
    {
        public override string Name => "/admin";

        public override bool forAdmin => false;

        public async override void Execute(MessageEventArgs e)
        {
            User user = Program.users.Find(u => u.id == e.Message.Chat.Id);
            int index = e.Message.Text.IndexOf(" ");
            if (index == -1)
            {
                await Program.bot.SendTextMessageAsync(user.id, "Використовуйте команду наступним чином: /admin 'пароль'");
            }
            else
            {
                if (e.Message.Text.Substring(index + 1) == Program.password)
                {
                    //Edit in DB
                    if (Program.ExecuteMySql($"UPDATE Users SET Admin = 1 WHERE ID = {user.id}"))
                    {
                        user.isadmin = true;
                        await Program.bot.SendTextMessageAsync(user.id, "Ви увыйшли як адміністратор!");
                    }
                    else
                    {
                        await Program.bot.SendTextMessageAsync(user.id, $"Виникла помилка при вході в адмінку. Будь ласка, зверніться до технічного адміністратора!");
                    }
                    //SaveSystem.Save();
                }
                else
                {
                    await Program.bot.SendTextMessageAsync(user.id, "Невірні дані!");
                    Console.WriteLine($"Помилка входу як Адміністратор у {user.name} з id {user.id}");
                }
            }
        }
    }
}
