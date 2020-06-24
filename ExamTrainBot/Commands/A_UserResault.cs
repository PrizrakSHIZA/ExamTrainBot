using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Args;

namespace ExamTrainBot.Commands
{
    class A_UserResault : Command
    {
        public override string Name => "/userresault";

        public override bool forAdmin => true;

        public async override void Execute(MessageEventArgs e)
        {
            User user = Program.users.Find(u => u.id == e.Message.Chat.Id);
            int index = e.Message.Text.IndexOf(" ");
            if (index == -1)
            {
                await Program.bot.SendTextMessageAsync(user.id, "Використовуйте команду наступним чином: /userresault 'user id'");
            }
            else
            {
                if (Program.users.Find(u => u.id == Convert.ToInt64(e.Message.Text.Substring(index + 1))) != null)
                {
                    User Selecteduser = Program.users.Find(u => u.id == Convert.ToInt32(e.Message.Text.Substring(index + 1)));
                    string text = $"Результати тестів {Selecteduser.name}:\n";
                    //create string
                    for (int i = 0; i < user.completedtests.Count; i++)
                    {
                        string mistakes = "Помилки у питання за номерами: ";
                        //looking for mistakes
                        for (int y = 0; y < user.completedtests[i].questions.Count; y++)
                        {
                            if (user.mistakes[i][y])
                                mistakes += $"{y + 1}, ";
                        }
                        /*
                        for (int y = 0; y < user.completedtests[i].questions.Count; y++)
                        {
                            foreach (int id in user.mistakes)
                            {
                                if (user.completedtests[i].questions[y] == Program.questions[id])
                                {
                                    mistakes += $" {y+1},";
                                }
                            }
                        }*/
                        text += $"Текст тесту: {user.completedtests[i].Text}\n=> {mistakes}\n=> {user.points[i]} балів\n";
                    }
                    await Program.bot.SendTextMessageAsync(user.id, text);
                }
                else
                {
                    await Program.bot.SendTextMessageAsync(user.id, $"Користувача з id {e.Message.Text.Substring(index + 1)} не знайдено");
                }

            }
        }
    }
}
