using SocialContractAssistant.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using SocialContractAssistant.Logs;

namespace SocialContractAssistant.Telegram
{
    internal class TelegramToolbars
    {
        LogController log = new LogController();
        public InlineKeyboardMarkup CreateToolbar(int questionId)
        {
            List<InlineKeyboardButton[]> keyboardButtons = new List<InlineKeyboardButton[]>();
            try
            {
                using (var db = new DataBaseContext())
                {
                    var options = db.Options.Where(p => p.QuestionId == questionId).ToList();

                    foreach (var item in options)
                    {
                        if (item.Text == "Консультация")
                        {
                            keyboardButtons.Add(new[] { InlineKeyboardButton.WithUrl(text: "Чат для консультации", url: "https://t.me/ConsultantSocial") });
                        }
                        else if (item.Text == "Калькулятор")
                        {
                            keyboardButtons.Add(new[] { InlineKeyboardButton.WithUrl(text: "Калькулятор прож. минимума", url: "https://t.me/CostLivingCalculatorBot") });
                        }
                        else
                        {
                            keyboardButtons.Add(new[] { InlineKeyboardButton.WithCallbackData(text: item.Text!, callbackData: item.Id.ToString()!) });
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("TelegramToolbars", "CreateToolbar", exp.Message);
            }
            return new InlineKeyboardMarkup(keyboardButtons);
        }

        /// <summary>
        /// Create replyMarkup for payment
        /// </summary>
        /// <param name="toolType"> WithUrl / WithWebApp </param>
        /// <param name="paymentUrl"></param>
        /// <returns></returns>
        public InlineKeyboardMarkup CreateToolbarPayment(string toolType, string paymentUrl)
        {
            List<InlineKeyboardButton[]> keyboardButtons = new List<InlineKeyboardButton[]>();
            try
            {
                switch (toolType)
                {
                    case "WithUrl": keyboardButtons.Add(new[] { InlineKeyboardButton.WithUrl(text: "Оплата", url: paymentUrl) }); break;
                    case "WithWebApp": keyboardButtons.Add(new[] { InlineKeyboardButton.WithWebApp(text: "Оплата", webAppInfo: new WebAppInfo() { Url = paymentUrl }) }); break;
                    default: break;
                }
            }
            catch (Exception exp)
            {
                log.Error("TelegramToolbars", "CreateToolbarPayment", exp.Message);
            }
            return new InlineKeyboardMarkup(keyboardButtons);
        }

        public InlineKeyboardMarkup CreateToolbarPaymentTimeout()
        {
            List<InlineKeyboardButton[]> keyboardButtons = new List<InlineKeyboardButton[]>();
            try
            {
                keyboardButtons.Add(new[] { InlineKeyboardButton.WithCallbackData(text: "Далее", callbackData: "check") });
                keyboardButtons.Add(new[] { InlineKeyboardButton.WithCallbackData(text: "Оплатить", callbackData: "pay") });
            }
            catch (Exception exp)
            {
                log.Error("TelegramToolbars", "CreateToolbarPaymentTimeout", exp.Message);
            }
            return new InlineKeyboardMarkup(keyboardButtons);
        }

        public InlineKeyboardMarkup CreateToolbarPaymentTimeoutConsult()
        {
            List<InlineKeyboardButton[]> keyboardButtons = new List<InlineKeyboardButton[]>();
            try
            {
                keyboardButtons.Add(new[] { InlineKeyboardButton.WithCallbackData(text: "Далее", callbackData: "checkConsult") });
                keyboardButtons.Add(new[] { InlineKeyboardButton.WithCallbackData(text: "Оплатить", callbackData: "payConsult") });
            }
            catch (Exception exp)
            {
                log.Error("TelegramToolbars", "CreateToolbarPaymentTimeoutConsult", exp.Message);
            }
            return new InlineKeyboardMarkup(keyboardButtons);
        }

        public InlineKeyboardMarkup CreateToolbarPaymentConsult()
        {
            List<InlineKeyboardButton[]> keyboardButtons = new List<InlineKeyboardButton[]>();
            try
            {
                keyboardButtons.Add(new[] { InlineKeyboardButton.WithUrl(text: "Чат для консультации", url: "https://t.me/ConsultantSocial") });
            }
            catch (Exception exp)
            {
                log.Error("TelegramToolbars", "CreateToolbarPaymentConsult", exp.Message);
            }
            return new InlineKeyboardMarkup(keyboardButtons);
        }
    }
}
