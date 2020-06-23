using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace ExamTrainBot.Commands
{
    class A_UserAdd : Command
    {
        public override string Name => "/useradd";

        public override bool forAdmin => true;

        public async override void Execute(MessageEventArgs e)
        {
            User user = Program.users.Find(u => u.id == e.Message.Chat.Id);
            int index = e.Message.Text.IndexOf(" ");
            if (index == -1)
            {
                await Program.bot.SendTextMessageAsync(user.id, "Використовуйте команду наступним чином: /adduser 'user id'");
            }
            else
            {
                if(Program.users.Find(u => u.id == Convert.ToInt64(e.Message.Text.Substring(index + 1))) != null)
                {
                    User Selecteduser = Program.users.Find(u => u.id == e.Message.Chat.Id);
                    if (!Selecteduser.subscriber)
                    {
                        //Edit in DB
                        if (Program.ExecuteMySql($"UPDATE Users SET Subscriber = 1 WHERE ID = {Selecteduser.id}"))
                        {
                            Selecteduser.subscriber = true;
                            await Program.bot.SendTextMessageAsync(user.id, $"Ви додали {Selecteduser.name} до списку підписчиків!");
                        }
                        else
                        {
                            await Program.bot.SendTextMessageAsync(user.id, $"Виникла помилка при додаванні {Selecteduser.name} до списку підписчиків!\nБудь ласка, зверніться до технічного адміністратора!");
                        }
                    }
                    else
                    {
                        await Program.bot.SendTextMessageAsync(user.id, $"Користувач {Selecteduser.name} вже є підписчиком!");
                    }
                }
                else
                {
                    await Program.bot.SendTextMessageAsync(user.id, $"Користувача з id {e.Message.Text.Substring(index + 1)} не знайдено");
                }
            }
        }
    }
}
