using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using SocialContractAssistant.Telegram;
using SocialContractAssistant.Models;
using SocialContractAssistant.DataAccess;
using SocialContractAssistant.Users;
using SocialContractAssistant.Logs;

namespace SocialContractAssistant.CreateBot
{
    internal class InterviewBot
    {
        public SettingsModel botSettings;

        public TelegramBotClient botClient;

        public CancellationTokenSource cancellation;

        LogController log = new LogController();

        public InterviewBot(int applicationId)
        {
            using (var db = new DataBaseContext())
            {
                botSettings = db.Settings.FirstOrDefault(p => p.ApplicationId == applicationId)!;
            }

            botClient = new TelegramBotClient(botSettings.TelegramToken!);

            cancellation = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };

            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cancellation.Token);
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                var telegramModel = new TelegramModel(botClient, update, cancellationToken);
                var telegramController = new TelegramController();

                //Тип сообщения написанного боту в чате
                if (update.Type == UpdateType.Message)
                {
                    //Если тип сообщения текстовое
                    if (update.Message!.Type == MessageType.Text)
                    {
                        await telegramController.ResponsMessage(telegramModel, botSettings);
                    }
                    #region Telegram SuccessfulPayment
                    ////Если тип сообщения об успешной оплате
                    //else if (update.Message!.Type == MessageType.SuccessfulPayment)
                    //{
                    //    await telegramController.SuccessfulPayment(botClient, update, cancellationToken, BotSettings);
                    //}
                    #endregion
                }

                //Тип сообщения после выбора кнопки в меню
                if (update.Type == UpdateType.CallbackQuery)
                {
                    await telegramController.ResponsCallbackQuery(telegramModel, botSettings);
                }

                #region Telegram PreCheckoutQuery
                ////Тип сообщения перед оплатой услуг
                //if (update.Type == UpdateType.PreCheckoutQuery)
                //{
                //    await botClient.AnswerPreCheckoutQueryAsync(update.PreCheckoutQuery!.Id);
                //}
                #endregion

                string qwe = "qwe";
                var ewq = Convert.ToInt32(qwe);
            }
            catch (Exception exp)
            {
                log.Error("InterviewBot", "HandleUpdateAsync", exp.Message);
            }
        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
