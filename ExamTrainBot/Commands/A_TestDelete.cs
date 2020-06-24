using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Args;

namespace ExamTrainBot.Commands
{
    class A_TestDelete : Command
    {
        public override string Name => "/testdelete";

        public override bool forAdmin => true;

        public async override void Execute(MessageEventArgs e)
        {
            User currentuser = Program.users.Find(u => u.id == e.Message.Chat.Id);
            int index = e.Message.Text.IndexOf(" ");
            if (index == -1)
            {
                await Program.bot.SendTextMessageAsync(currentuser.id, "Використовуйте команду наступним чином: /testdelete 'user id'");
            }
            else
            {
                try
                {
                    int number = Int32.Parse(e.Message.Text.Substring(index + 1));
                    if (Program.testlist[number] != null)
                    {
                        if (Program.ExecuteMySql($"DELETE FROM Tests WHERE ID = {number + 1}"))
                        {
                            Program.testlist.RemoveAt(number);
                            //SaveSystem.SaveTests();
                            await Program.bot.SendTextMessageAsync(currentuser.id, $"Тест за номером {number} успішно видалено зі списку!");
                        }
                        else 
                        {
                            await Program.bot.SendTextMessageAsync(currentuser.id, $"Виникла помилка при внесенні змін у БД. Будь ласка, зверніться до технічного адміністратора!");
                        }
                    }
                    else
                    {
                        await Program.bot.SendTextMessageAsync(currentuser.id, $"Тесту за номером {number} не існує");
                    }
                }
                catch (Exception exception)
                {
                    await Program.bot.SendTextMessageAsync(currentuser.id, $"Під час виконання команди виникла помилка. Текст помилки: {exception.Message}");
                }
            }
        }
    }
}
