using Telegram.Bot;
using Telegram.Bot.Types;
using SocialContractAssistant.DataAccess;
using SocialContractAssistant.Models;
using System.Net;
using Yandex.Checkout.V3;
using SocialContractAssistant.Documents;
using SocialContractAssistant.Yookassa;
using Telegram.Bot.Types.ReplyMarkups;
using SocialContractAssistant.Questions;
using SocialContractAssistant.Users;
using Google.Protobuf.WellKnownTypes;
using SocialContractAssistant.Enums;
using SocialContractAssistant.Logs;

namespace SocialContractAssistant.Telegram
{
    internal class TelegramController : TelegramSender
    {
        LogController log = new LogController();

        /// <summary>
        /// Собираем ответ для стартового сообщения;
        /// Добавляем нового пользователя в базу;
        /// Фиксируем ответ на вопрос конкретного пользователя;
        /// </summary>
        /// <param name="botClient">Передаем из HandleUpdateAsync</param>
        /// <param name="update">Передаем из HandleUpdateAsync</param>
        /// <param name="cancellationToken">Передаем из HandleUpdateAsync</param>
        public async Task ResponsMessage(TelegramModel telegramModel, SettingsModel botSettings)
        {
            Console.WriteLine($"Получено сообщение: '{telegramModel.message.messageText}' в чате {telegramModel.message.chatId} от {telegramModel.message.username} ({telegramModel.message.firstName} {telegramModel.message.secondName})");

            if (telegramModel.message.messageText == "/test")
            {
                #region Testing

                try
                {
                    var document = new DocumentController();

                    var fileName = $"Documents/Document_{telegramModel.message.chatId}_{DateTime.Now.ToShortDateString()}";
                    var fileNamePdf = $"{fileName}.pdf";
                    var fileNameTxt = $"{fileName}.txt";

                    document.CreateTest(new BytescountPdf(), fileNamePdf);
                    document.CreateTest(new NotepadeTxt(), fileNameTxt);

                    await SendDocument(telegramModel, fileNamePdf, "Documents.pdf", String.Empty);
                    await SendDocument(telegramModel, fileNameTxt, "Documents.txt", String.Empty);
                }
                catch (Exception exp)
                {
                    log.Error("TelegramController", "ResponsMessage Test", exp.Message, telegramModel.message.chatId);
                }

                #endregion
            }

            if (telegramModel.message.messageText == "/start")
            {
                try
                {
                    await new UserController().DeleteMessages(telegramModel);

                    //Добавляем нового пользователя
                    var userController = new UserController();
                    if (!userController.UserExists(telegramModel.message.chatId))
                    {
                        userController.InserUser(telegramModel);
                    }
                    userController.DeleteAnswers(telegramModel.message.chatId, botSettings.ApplicationId);

                    //Получаем данные по первому вопросу
                    var senderModel = new QuestionController().GetStartQuestion(botSettings.ApplicationId);
                    await SendTextMessage(telegramModel, senderModel.QuestionText, senderModel.ReplyMarkup);
                }
                catch (Exception exp)
                {
                    log.Error("TelegramController", "ResponsMessage Start", exp.Message, telegramModel.message.chatId);
                }
            }
        }

        /// <summary>
        /// Собираем ответ для изменения сообщения;
        /// Фиксируем ответ на вопрос конкретного пользователя;
        /// </summary>
        /// <param name="botClient">Передаем из HandleUpdateAsync</param>
        /// <param name="update">Передаем из HandleUpdateAsync</param>
        /// <param name="cancellationToken">Передаем из HandleUpdateAsync</param>
        public async Task ResponsCallbackQuery(TelegramModel telegramModel, SettingsModel botSettings)
        {
            if (telegramModel.message.buttonData == "check" || telegramModel.message.buttonData == "pay")
            {
                try
                {
                    var userController = new UserController();
                    var yookassaController = new YookassaController();

                    //Проверяем оплату на yoo кассе
                    var paymentId = userController.GetCheckPaymentId(telegramModel.message.chatId, (int)PaymentType.documents);
                    var yooPayment = yookassaController.YooPaymentGet(paymentId!);

                    if (yooPayment.Status == PaymentStatus.Succeeded)
                    {
                        //Обновляем статус в базе
                        userController.UpdatePayment(paymentId!, yooPayment.Status);

                        //Получаем данные по вопросу после оплаты
                        var senderModel = new QuestionController().GetAfterQuestion(botSettings.ApplicationId);
                        await SendTextMessage(telegramModel, senderModel.QuestionText, senderModel.ReplyMarkup);
                    }
                    else
                    {
                        await yookassaController.ResponsPayment(telegramModel, botSettings, yooPayment.Id);
                    }
                }
                catch (Exception exp)
                {
                    log.Error("TelegramController", "ResponsCallbackQuery CheckPay", exp.Message, telegramModel.message.chatId);
                }
            }
            else if (telegramModel.message.buttonData == "checkConsult" || telegramModel.message.buttonData == "payConsult")
            {
                try
                {
                    var userController = new UserController();
                    var yookassaController = new YookassaController();

                    //Проверяем оплату на yoo кассе
                    var paymentId = userController.GetCheckPaymentId(telegramModel.message.chatId, (int)PaymentType.consult);
                    var yooPayment = yookassaController.YooPaymentGet(paymentId!);

                    if (yooPayment.Status == PaymentStatus.Succeeded)
                    {
                        //Обновляем статус в базе
                        userController.UpdatePayment(paymentId!, yooPayment.Status);

                        await SendTextMessage(telegramModel,
                            "Тебе необходимо перейти в этот чат и обученный человек сможет разрешить твою ситуацию и подскажет, что нужно сделать, для получения стипендии.",
                            new TelegramToolbars().CreateToolbarPaymentConsult());

                        //Current chat ID: 406414632 (https://t.me/getmyid_bot) 
                        await SendTextMessage(telegramModel.botClient, 406414632, $"Пользователь {userController.GetUsername(paymentId!)} оплатил консультацию.", telegramModel.cancellationToken);

                        userController.UpdatePayment(paymentId!, false);
                    }
                    else
                    {
                        await yookassaController.ResponsPaymentConsult(telegramModel, botSettings, yooPayment.Id);
                    }
                }
                catch (Exception exp)
                {
                    log.Error("TelegramController", "ResponsCallbackQuery CheckPayConsult", exp.Message, telegramModel.message.chatId);
                }
            }
            else
            {
                //var optionId = Convert.ToInt32(message.ButtonData);
                var option = new QuestionController().GetOption(telegramModel.message.buttonData!);

                //Добавляем ответ пользователя
                var userController = new UserController();
                userController.InserAnswer(telegramModel.message.chatId, option.Id);

                if (option!.Text == "Назад")
                {
                    try
                    {
                        //Получаем данные по предыдущему вопросу
                        var senderModel = new QuestionController().GetBackQuestion(telegramModel.message.chatId, botSettings.ApplicationId);
                        await EditMessageText(telegramModel, senderModel.QuestionText, senderModel.ReplyMarkup);
                    }
                    catch (Exception exp)
                    {
                        log.Error("TelegramController", "ResponsCallbackQuery Back", exp.Message, telegramModel.message.chatId);
                    }
                }
                //else if (option!.Text == "Консультация")
                //{
                    //try
                    //{
                    //    await SendTextMessage(telegramModel,
                    //    "Вам необходимо перейти в этот чат и наш специалист поможет разрешить проблему и подскажет, что нужно сделать, для получения стипендии.",
                    //    new TelegramToolbars().CreateToolbarPaymentConsult());
                    //}
                    //catch (Exception exp)
                    //{
                    //    log.Error("TelegramController", "ResponsCallbackQuery Consult", exp.Message, telegramModel.message.chatId);
                    //}

                    //try
                    //{
                    //    //Проверяем оплату на yoo кассе (в случае если клиент оплатил, но не получил документы)
                    //    var paymentId = new UserController().GetCheckPaymentId(telegramModel.message.chatId, (int)PaymentType.consult);
                    //    var yooPayment = new YookassaController().YooPaymentGet(paymentId!);
                    //    var yooPaymentId = yooPayment != null ? yooPayment.Id : null;

                    //    await new YookassaController().ResponsPaymentConsult(telegramModel, botSettings, yooPaymentId);
                    //}
                    //catch (Exception exp)
                    //{
                    //    log.Error("TelegramController", "ResponsCallbackQuery Consult", exp.Message, telegramModel.message.chatId);
                    //}
                //}
                else if (option.Question!.TypeId == (int)QuestionType.before)
                {
                    try
                    {
                        //Проверяем оплату на yoo кассе (в случае если клиент оплатил, но не получил документы)
                        var paymentId = new UserController().GetCheckPaymentId(telegramModel.message.chatId, (int)PaymentType.documents);
                        var yooPayment = new YookassaController().YooPaymentGet(paymentId!);
                        var yooPaymentId = yooPayment != null ? yooPayment.Id : null;

                        await new YookassaController().ResponsPayment(telegramModel, botSettings, yooPaymentId);
                    }
                    catch (Exception exp)
                    {
                        log.Error("TelegramController", "ResponsCallbackQuery Before", exp.Message, telegramModel.message.chatId);
                    }
                }
                else if (option.Question!.TypeId == (int)QuestionType.finish)
                {
                    try
                    {
                        var document = new DocumentController();

                        var fileName = $"Documents/Document_{telegramModel.message.chatId}_{DateTime.Now.ToShortDateString()}";
                        var fileNamePdf = $"{fileName}.pdf";
                        var fileNameTxt = $"{fileName}.txt";

                        document.Create(new BytescountPdf(), fileNamePdf, telegramModel.message.chatId, botSettings.ApplicationId);
                        document.Create(new NotepadeTxt(), fileNameTxt, telegramModel.message.chatId, botSettings.ApplicationId);

                        await SendDocument(telegramModel, fileNamePdf, "Documents.pdf", String.Empty);
                        await SendDocument(telegramModel, fileNameTxt, "Documents.txt", String.Empty);

                        //После получения документов делаем платеж неактивным
                        var paymentId = new UserController().GetCheckPaymentId(telegramModel.message.chatId, (int)PaymentType.documents);
                        new UserController().UpdatePayment(paymentId!, false);
                    }
                    catch (Exception exp)
                    {
                        log.Error("TelegramController", "ResponsCallbackQuery Finish", exp.Message, telegramModel.message.chatId);
                    }
                }
                else if (option.Question!.TypeId == (int)QuestionType.replace)
                {
                    try
                    {
                        //Получаем данные по следующему вопросу
                        var senderModel = new QuestionController().GetNextQuestion(option!.NextQuestionId);

                        if (userController.isCollegeStudent(telegramModel.message.chatId))
                        {
                            senderModel.QuestionText = senderModel.QuestionText
                                .Replace("{minAmount}", "928")
                                .Replace("{education}", "среднего")
                                .Replace("{avgAmount}", "1000-3000");
                        }
                        else if (userController.isUniversityStudent(telegramModel.message.chatId))
                        {
                            senderModel.QuestionText = senderModel.QuestionText
                                .Replace("{minAmount}", "2227")
                                .Replace("{education}", "высшего")
                                .Replace("{avgAmount}", "2700-3000");
                        }

                        await EditMessageText(telegramModel, senderModel.QuestionText, senderModel.ReplyMarkup);
                    }
                    catch (Exception exp)
                    {
                        log.Error("TelegramController", "ResponsCallbackQuery Replace", exp.Message, telegramModel.message.chatId);
                    }
                }
                else
                {
                    try
                    {
                        //Получаем данные по следующему вопросу
                        var senderModel = new QuestionController().GetNextQuestion(option!.NextQuestionId);
                        await EditMessageText(telegramModel, senderModel.QuestionText, senderModel.ReplyMarkup);
                    }
                    catch (Exception exp)
                    {
                        log.Error("TelegramController", "ResponsCallbackQuery Next", exp.Message, telegramModel.message.chatId);
                    }
                }
            }
        }
    }
}
