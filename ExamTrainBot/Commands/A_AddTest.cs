using ExamTrainBot.Tests;
using ExamTrainBot.Tests.Questions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace ExamTrainBot.Commands
{
    class A_AddTest : Command
    {
        public override string Name => "/addtest";

        public override bool forAdmin => true;

        public Test test = new Test("1", new List<Question>());
        public string callback, text, answer;
        public int points, columns, stage = 0;
        public string[] variants;

        
        char[] delimiterChars = { ',', '.', '\t', '\n' };
        MessageEventArgs y;

        public async override void Execute(MessageEventArgs e)
        {
            User user = Program.GetCurrentUser(e.Message.Chat.Id);
            if (!user.testcreation)
            {
                int index = e.Message.Text.IndexOf(" ");
                if (index == -1)
                {
                    await Program.bot.SendTextMessageAsync(user.id, "Використовуйте команду наступним чином: /addtest 'Правило нового тесту(текст)'");
                }
                else
                {
                    user.testcreation = true;
                    test.Text = e.Message.Text.Substring(index + 1);
                    y = e;
                    string[] arr = { "Тест", "Відповідність", "Вільна відповідь", "Закінчити створення" };
                    InlineKeyboardMarkup keyboard = Program.GetInlineKeyboard(arr, 2);
                    await Program.bot.SendTextMessageAsync(user.id, "Виберіть тип нового питання", replyMarkup: keyboard);
                }
            }
            else if (callback != null)
            {
                try
                {
                    switch (callback)
                    {
                        case "Тест":
                            switch (stage)
                            {
                                case 1:
                                    await Program.bot.SendTextMessageAsync(user.id, "Введіть текст запитання");
                                    stage++; break;
                                case 2:
                                    text = e.Message.Text;
                                    await Program.bot.SendTextMessageAsync(user.id, "Введіть варіванти відповідей(використовуйте ',', '.', табуляцію або нову стрічку для розділення варіантів)");
                                    stage++; break;
                                case 3:
                                    variants = e.Message.Text.Split(delimiterChars);
                                    await Program.bot.SendTextMessageAsync(user.id, "Введіть кількість стовпчиків у яких будуть відображатися варіанти");
                                    stage++; break;
                                case 4:
                                    columns = Int32.Parse(e.Message.Text);
                                    await Program.bot.SendTextMessageAsync(user.id, "Введіть правильну відповідь(має повністю співпадати з одним із варіантів)");
                                    stage++; break;
                                case 5:
                                    answer = e.Message.Text;
                                    await Program.bot.SendTextMessageAsync(user.id, "Введіть кількість балів за це запитання");
                                    stage++; break;
                                case 6:
                                    points = Int32.Parse(e.Message.Text);
                                    test.questions.Add(new TestQuestion(text, points, variants, columns, answer));
                                    await Program.bot.SendTextMessageAsync(user.id, "Питання створено.");
                                    stage = 0;
                                    callback = null;
                                    Execute(e); break;
                                default: break;
                            }
                            break;
                        case "Відповідність":
                            switch (stage)
                            {
                                case 1:
                                    await Program.bot.SendTextMessageAsync(user.id, "Введіть текст запитання");
                                    stage++; break;
                                case 2:
                                    text = e.Message.Text;
                                    await Program.bot.SendTextMessageAsync(user.id, "Введіть кількість балів за це запитання");
                                    stage++; break;
                                case 3:
                                    points = Int32.Parse(e.Message.Text);
                                    await Program.bot.SendTextMessageAsync(user.id, "Введіть правильну відповідь");
                                    stage++; break;
                                case 4:
                                    answer = e.Message.Text;
                                    test.questions.Add(new ConformityQuestion(text, points, answer));
                                    await Program.bot.SendTextMessageAsync(user.id, "Питання створено.");
                                    stage = 0;
                                    callback = null;
                                    Execute(e); break;
                                default: break;
                            }
                            break;
                        case "Вільна відповідь":
                            switch (stage)
                            {
                                case 1:
                                    await Program.bot.SendTextMessageAsync(user.id, "Введіть текст запитання");
                                    stage++; break;
                                case 2:
                                    text = e.Message.Text;
                                    await Program.bot.SendTextMessageAsync(user.id, "Введіть правильну відповідь");
                                    stage++; break;
                                case 3:
                                    answer = e.Message.Text;
                                    await Program.bot.SendTextMessageAsync(user.id, "Введіть кількість балів за це запитання");
                                    stage++; break;
                                case 4:
                                    points = Int32.Parse(e.Message.Text);
                                    test.questions.Add(new FreeQuestion(text, points, answer));
                                    await Program.bot.SendTextMessageAsync(user.id, "Питання створено.");
                                    stage = 0;
                                    callback = null;
                                    Execute(e); break;
                                default: break;
                            }
                            break;
                        case "Закінчити створення":
                            user.testcreation = false;
                            stage = 0;
                            Program.testlist.Add(new Test(test));
                            test = null;
                            SaveSystem.SaveTests();
                            await Program.bot.SendTextMessageAsync(user.id, "Ви успішно закінчили створення нового тесту");
                            break;
                        default: break;
                    }
                }
                catch (Exception exception)
                {
                    user.testcreation = false;
                    stage = 0;
                    test = null;
                    await Program.bot.SendTextMessageAsync(user.id, $"Виникла помилка при створенні тесту. Вожливо ви ввели невірні дані. Помилка:{exception.Message}");
                }
            }
            else
            {
                string[] arr = { "Тест", "Відповідність", "Вільна відповідь", "Закінчити створення" };
                InlineKeyboardMarkup keyboard = Program.GetInlineKeyboard(arr, 2);
                await Program.bot.SendTextMessageAsync(user.id, "Виберіть тип нового питання", replyMarkup: keyboard);
            }
        }

        public void Execute(string callback)
        {
            this.callback = callback;
            stage++;
            Execute(y);
        }
    }
}
