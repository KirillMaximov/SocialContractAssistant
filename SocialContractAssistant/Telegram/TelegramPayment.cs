using SocialContractAssistant.DataAccess;
using SocialContractAssistant.Enums;
using SocialContractAssistant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Payments;

namespace SocialContractAssistant.Telegram
{
    internal class TelegramPayment : TelegramSender
    {
        //enum QuestionType
        //{
        //    simple = 1,
        //    start = 2,
        //    before = 3,
        //    after = 4,
        //    finish = 5
        //}

        /// <summary>
        /// Собираем ответ для оплаты услуг;
        /// </summary>
        /// <param name = "botClient" > Передаем из HandleUpdateAsync</param>
        /// <param name = "update" > Передаем из HandleUpdateAsync</param>
        /// <param name = "cancellationToken" > Передаем из HandleUpdateAsync</param>
        public async Task ResponsCallbackPayment(TelegramModel telegramModel, SettingsModel botSettings)
        {
            List<LabeledPrice> prices = new List<LabeledPrice>() { };
            prices.Add(new LabeledPrice(botSettings.PriceLable!, botSettings.PriceAmount * 100));

            await SendInvoice(telegramModel, botSettings.PriceTitle!, botSettings.PriceDescription!, botSettings.PaymentTokenLife!, prices);
        }

        /// <summary>
        /// Собираем ответ для сообщения после оплаты услуг;
        /// Фиксируем оплату конкретного пользователя;
        /// </summary>
        /// <param name = "botClient" > Передаем из HandleUpdateAsync</param>
        /// <param name = "update" > Передаем из HandleUpdateAsync</param>
        /// <param name = "cancellationToken" > Передаем из HandleUpdateAsync</param>
        public async Task SuccessfulPayment(TelegramModel telegramModel, SettingsModel botSettings)
        {
            //Добавляем токен после оплаты
            //var userController = new Users.UserController();
            //userController.InserPayment(message.Username, message.Payment!);

            using (var db = new DataBaseContext())
            {
                //Находим данные по вопросу после оплаты
                var question = db.Questions.FirstOrDefault(q => q.TypeId == (int)QuestionType.after && q.ApplicationId == botSettings.ApplicationId);
                var questionId = question!.Id;
                var questionText = question!.Text;

                //Создаем меню для вопроса
                var replyMarkup = new TelegramToolbars().CreateToolbar(questionId);

                await SendTextMessage(telegramModel, questionText!, replyMarkup);
            }
        }
    }
}
