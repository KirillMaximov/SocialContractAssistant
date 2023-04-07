using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace SocialContractAssistant.Models
{
    internal class SenderModel
    {
        public String QuestionText { get; set; }
        public InlineKeyboardMarkup ReplyMarkup { get; set; }

        public SenderModel() 
        {
            QuestionText = String.Empty;
            ReplyMarkup = new InlineKeyboardMarkup(new InlineKeyboardButton(String.Empty));
        }
        public SenderModel(String questionText, InlineKeyboardMarkup replyMarkup) 
        {
            QuestionText = questionText;
            ReplyMarkup = replyMarkup;
        }
    }
}
