﻿using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace ExamTrainBot.Tests.Questions
{
    [Serializable]
    class TestQuestion : Question
    {

        public override string text { get; set; }
        public override int points { get; set; }
        public override string[] variants { get; set; }

        public int columns;
        public override dynamic answer { get; set; }

        public async override void Ask(long id)
        {
            User user = Program.GetCurrentUser(id);
            InlineKeyboardMarkup keyboard = Program.GetInlineKeyboard(variants, columns);
            await Program.bot.SendTextMessageAsync(user.id, text, replyMarkup: keyboard);
        }

        public TestQuestion(string text, int points, string[] variants, int columns, string answer)
        {
            this.text = text;
            this.points = points;
            this.variants = variants;
            this.columns = columns;
            this.answer = answer;
        }
    }
}
