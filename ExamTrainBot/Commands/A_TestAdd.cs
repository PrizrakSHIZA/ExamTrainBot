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
    class A_TestAdd : Command
    {
        public override string Name => "/testadd";

        public override bool forAdmin => true;

        public Test test = new Test("", new List<Question>());
        public string callback, text, answer;
        public int points, columns, stage = 0;
        public string[] variants;
        public List<int> questions = new List<int>();

        MessageEventArgs y;

        public async override void Execute(MessageEventArgs e)
        {
            User user = Program.GetCurrentUser(e);
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
                                    variants = e.Message.Text.Split(Program.delimiterChars);
                                    for (int i = 0; i < variants.Length; i++)
                                    {
                                        if (variants[i].IndexOf(' ') == 0)
                                        {
                                            variants[i] = variants[i].Remove(0, 1);
                                        }
                                    }
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
                                    if (Program.ExecuteMySql($"INSERT INTO Questions (Type, Text, Points, Variants, Answer, Columns) VALUES (1, '{text}', {points}, '{string.Join(";", variants)}', '{answer}', {columns})"))
                                    {
                                        TestQuestion q = new TestQuestion(text, points, variants, columns, answer);
                                        test.questions.Add(q);
                                        Program.questions.Add(q);
                                        questions.Add(Program.questions.IndexOf(q) + 1);
                                        await Program.bot.SendTextMessageAsync(user.id, "Питання створено.");
                                        stage = 0;
                                        callback = null;
                                        Execute(e); break;
                                    }
                                    else
                                    {
                                        await Program.bot.SendTextMessageAsync(user.id, $"Виникла помилка при внесенні змін у БД. Будь ласка, зверніться до технічного адміністратора!");
                                        break;
                                    }
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
                                    await Program.bot.SendTextMessageAsync(user.id, "Введіть правильну відповідь");
                                    stage++; break;
                                case 3:
                                    answer = e.Message.Text;
                                    await Program.bot.SendTextMessageAsync(user.id, "Введіть кількість балів за це запитання");
                                    stage++; break;
                                case 4:
                                    points = Int32.Parse(e.Message.Text);
                                    if (Program.ExecuteMySql($"INSERT INTO Questions (Type, Text, Points, Answer) VALUES (3, '{text}', {points}, '{answer}')"))
                                    {
                                        ConformityQuestion q = new ConformityQuestion(text, points, answer);
                                        test.questions.Add(q);
                                        Program.questions.Add(q);
                                        questions.Add(Program.questions.IndexOf(q) + 1);
                                        await Program.bot.SendTextMessageAsync(user.id, "Питання створено.");
                                        stage = 0;
                                        callback = null;
                                        Execute(e); break;
                                    }
                                    else
                                    {
                                        await Program.bot.SendTextMessageAsync(user.id, $"Виникла помилка при внесенні змін у БД. Будь ласка, зверніться до технічного адміністратора!");
                                        break;
                                    }
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
                                    if (Program.ExecuteMySql($"INSERT INTO Questions (Type, Text, Points, Answer) VALUES (2, '{text}', {points}, '{answer}')"))
                                    {
                                        FreeQuestion q = new FreeQuestion(text, points, answer);
                                        test.questions.Add(q);
                                        Program.questions.Add(q);
                                        questions.Add(Program.questions.IndexOf(q) + 1);
                                        await Program.bot.SendTextMessageAsync(user.id, "Питання створено.");
                                        stage = 0;
                                        callback = null;
                                        Execute(e); break;
                                    }
                                    else
                                    {
                                        await Program.bot.SendTextMessageAsync(user.id, $"Виникла помилка при внесенні змін у БД. Будь ласка, зверніться до технічного адміністратора!");
                                        break;
                                    }
                                default: break;
                            }
                            break;
                        case "Закінчити створення":
                            user.testcreation = false;
                            stage = 0;
                            if (Program.ExecuteMySql($"INSERT INTO Tests (Rule, Questions) VALUES ('{test.Text}', '{string.Join(';',questions)}')"))
                            {
                                Program.testlist.Add(new Test(test));
                                test = null;
                                SaveSystem.SaveTests();
                                await Program.bot.SendTextMessageAsync(user.id, "Ви успішно закінчили створення нового тесту");
                                break;
                            }
                            else
                            {
                                await Program.bot.SendTextMessageAsync(user.id, $"Виникла помилка при внесенні змін у БД. Будь ласка, зверніться до технічного адміністратора!");
                                break;
                            }
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
