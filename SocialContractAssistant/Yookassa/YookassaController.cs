using SocialContractAssistant.DataAccess;
using SocialContractAssistant.Enums;
using SocialContractAssistant.Logs;
using SocialContractAssistant.Models;
using SocialContractAssistant.Telegram;
using SocialContractAssistant.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Yandex.Checkout.V3;

namespace SocialContractAssistant.Yookassa
{
    internal class YookassaController : TelegramSender
    {
        LogController log = new LogController();

        #region Control payment for Documents

        public async Task ResponsPayment(TelegramModel telegramModel, SettingsModel botSettings, String? paymentId)
        {
            try
            {
                if (String.IsNullOrEmpty(paymentId))
                {
                    var userController = new UserController();

                    Payment yooPayment = new Payment();

                    if (userController.isCollegeStudent(telegramModel.message.chatId))
                    {
                        yooPayment = YooPaymentCreate(929, "Оплата услуг для студента колледжа");
                    }
                    if (userController.isUniversityStudent(telegramModel.message.chatId))
                    {
                        yooPayment = YooPaymentCreate(1389, "Оплата услуг для студента университета");
                    }
                    else
                    {
                        yooPayment = YooPaymentCreate(1389, "Оплата услуг");
                    }

                    userController.InserPayment(telegramModel.message.chatId, yooPayment!.Id, (int)PaymentType.documents);

                    await SendTextMessage(telegramModel,
                        "Перейдите по ссылке для оплаты",
                        new TelegramToolbars().CreateToolbarPayment("WithUrl", yooPayment.Confirmation.ConfirmationUrl));

                    YooPaymentThread(yooPayment.Id, telegramModel, botSettings);
                }
                else
                {
                    var yooPayment = YooPaymentGet(paymentId);

                    if (yooPayment.Status == PaymentStatus.Pending && yooPayment.Confirmation.ConfirmationUrl != null)
                    {
                        await SendTextMessage(telegramModel,
                            $"Оплата не была произведена. Для предоставления помощи, требуется ее оплатить.{Environment.NewLine}Перейдите по ссылке для оплаты.",
                            new TelegramToolbars().CreateToolbarPayment("WithUrl", yooPayment.Confirmation.ConfirmationUrl));

                        YooPaymentThread(yooPayment.Id, telegramModel, botSettings);
                    }
                    else
                    {
                        new UserController().UpdatePayment(paymentId!, false);
                        await ResponsPayment(telegramModel, botSettings, null);
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("YookassaController", "ResponsPayment", exp.Message, telegramModel.message.chatId);
            }
        }
       
        public async void YooPaymentThread(String paymentId, TelegramModel telegramModel, SettingsModel botSettings)
        {
            await Task.Run(() => YooPaymentSuccess(paymentId, telegramModel, botSettings));
        }

        public async void YooPaymentSuccess(String paymentId, TelegramModel telegramModel, SettingsModel botSettings)
        {
            try
            {
                Thread.Sleep(1000);
                var yooPayment = YooPaymentGet(paymentId);

                var repeat = true;

                var timeoutMax = 300; //5 минут
                var timeoutCount = 0;

                while (repeat)
                {
                    Thread.Sleep(1000);
                    yooPayment = YooPaymentGet(paymentId);
                    if (yooPayment.Status == PaymentStatus.Succeeded)
                    {
                        repeat = false;
                    }
                    if (timeoutCount > timeoutMax) // timeout > 5 минут, выходим из цикла
                    {
                        repeat = false;
                    }
                    timeoutCount++;
                }

                if (timeoutCount > timeoutMax)
                {
                    await SendTextMessage(telegramModel, "Вы на пути к получению повышенной стипендии. Если Вы оплатили чек-лист, нажмите 'Далее' и я выдам документы!",
                        new TelegramToolbars().CreateToolbarPaymentTimeout());
                }
                else
                {
                    //Добавляем токен после оплаты
                    var userController = new UserController();
                    userController.UpdatePayment(paymentId, PaymentStatus.Succeeded);

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
            catch (Exception exp)
            {
                log.Error("YookassaController", "YooPaymentSuccess", exp.Message, telegramModel.message.chatId);
            }
        }

        #endregion

        #region Control payment for Consult

        public async Task ResponsPaymentConsult(TelegramModel telegramModel, SettingsModel botSettings, String? paymentId)
        {
            try
            {
                if (String.IsNullOrEmpty(paymentId))
                {
                    var userController = new UserController();

                    Payment yooPayment = YooPaymentCreate(2000, "Оплата консультации");

                    userController.InserPayment(telegramModel.message.chatId, yooPayment!.Id, (int)PaymentType.consult);

                    await SendTextMessage(telegramModel,
                        "Перейдите по ссылке для оплаты консультации. Наш специалист постарается помочь Вам с проблемой. В случае, если она окажется нерешаемой, Вам вернется 70% от стоимости консультации.",
                        new TelegramToolbars().CreateToolbarPayment("WithUrl", yooPayment.Confirmation.ConfirmationUrl));

                    YooPaymentThreadConsult(yooPayment.Id, telegramModel, botSettings);
                }
                else
                {
                    var yooPayment = YooPaymentGet(paymentId);

                    if (yooPayment.Status == PaymentStatus.Pending && yooPayment.Confirmation.ConfirmationUrl != null)
                    {
                        await SendTextMessage(telegramModel,
                            $"Оплата не была произведена. Для предоставления помощи, требуется ее оплатить.{Environment.NewLine}Перейдите по ссылке для оплаты.",
                            new TelegramToolbars().CreateToolbarPayment("WithUrl", yooPayment.Confirmation.ConfirmationUrl));

                        YooPaymentThreadConsult(yooPayment.Id, telegramModel, botSettings);
                    }
                    else
                    {
                        new UserController().UpdatePayment(paymentId!, false);
                        await ResponsPaymentConsult(telegramModel, botSettings, null);
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("YookassaController", "ResponsPaymentConsult", exp.Message, telegramModel.message.chatId);
            }
        }

        public async void YooPaymentThreadConsult(String paymentId, TelegramModel telegramModel, SettingsModel botSettings)
        {
            await Task.Run(() => YooPaymentSuccessConsult(paymentId, telegramModel, botSettings));
        }

        public async void YooPaymentSuccessConsult(String paymentId, TelegramModel telegramModel, SettingsModel botSettings)
        {
            try
            {
                Thread.Sleep(1000);

                var yooPayment = YooPaymentGet(paymentId);

                var repeat = true;

                var timeoutMax = 300; //5 минут
                var timeoutCount = 0;

                while (repeat)
                {
                    Thread.Sleep(1000);
                    yooPayment = YooPaymentGet(paymentId);
                    if (yooPayment.Status == PaymentStatus.Succeeded)
                    {
                        repeat = false;
                    }
                    if (timeoutCount > timeoutMax) // timeout > 5 минут, выходим из цикла
                    {
                        repeat = false;
                    }
                    timeoutCount++;
                }

                if (timeoutCount > timeoutMax)
                {
                    await SendTextMessage(telegramModel,
                        "Истекло время ожидания оплаты по консультации, если Вы оплатили или вот-вот оплатите, после оплаты просто нажмите 'Далее'!",
                        new TelegramToolbars().CreateToolbarPaymentTimeoutConsult());
                }
                else
                {
                    //Добавляем токен после оплаты
                    var userController = new UserController();
                    userController.UpdatePayment(paymentId, PaymentStatus.Succeeded);

                    await SendTextMessage(telegramModel,
                        "Вам необходимо перейти в этот чат и наш специалист поможет разрешить проблему и подскажет, что нужно сделать, для получения стипендии.",
                        new TelegramToolbars().CreateToolbarPaymentConsult());

                    //Consult chat ID: 6148235538 (https://t.me/getmyid_bot) 
                    await SendTextMessage(telegramModel.botClient, 6148235538, $"Пользователь {userController.GetUsername(paymentId)} оплатил консультацию.", telegramModel.cancellationToken);

                    userController.UpdatePayment(paymentId, false);
                }
            }
            catch (Exception exp)
            {
                log.Error("YookassaController", "YooPaymentSuccessConsult", exp.Message, telegramModel.message.chatId);
            }
        }

        #endregion

        #region YooKassa api payment
        public Payment YooPaymentCreate(Decimal amount, String description)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                var client = new Client(
                   shopId: "988599",
                   secretKey: "live_6iz-Ys7cB0AlewjOYJ1OTb_ikkUSlRRJLLkcyFn2kX4");

                // 1. Создайте платеж и получите ссылку для оплаты
                var newPayment = new NewPayment
                {
                    Amount = new Amount { Value = amount, Currency = "RUB" },
                    PaymentMethodData = new PaymentMethod
                    {
                        Type = "bank_card"
                    },
                    Confirmation = new Confirmation
                    {
                        Type = ConfirmationType.Redirect,
                        ReturnUrl = "https://t.me/SocialStipendSupportBot"
                    },
                    Capture = true,
                    Description = description,
                    Receipt = new Receipt
                    {
                        Customer = new Customer
                        {
                            FullName = "Kirienko Evgeniy Sergeevich",
                            Email = "kirienko.mtb@yandex.ru",
                            Phone = "79105457536",
                            Inn = "402812702382"
                        },
                        Items = new List<ReceiptItem>()
                    {
                        new ReceiptItem
                        {
                            Description = "Оплата чек-лист.",
                            Quantity = 1,
                            Amount = new Amount { Value = amount, Currency = "RUB"},
                            VatCode = VatCode.NoVat,
                            PaymentMode = PaymentMode.FullPayment,
                            PaymentSubject = PaymentSubject.Service,
                            CountryOfOriginCode = "SC", //Social Consult
                            ProductCode = "none",
                            CustomsDeclarationNumber = "none",
                            Excise = "none",
                            Supplier = new Supplier
                            {
                                Name = "none",
                                Phone = "none",
                                Inn = "none"
                            }
                        }
                    }
                    }
                };
                return client.CreatePayment(newPayment);
            }
            catch (Exception exp)
            {
                log.Error("YookassaController", "YooPaymentCreate", exp.Message);
                return new Payment();
            }
        }

        public Payment YooPaymentGet(String paymnetId)
        {
            try
            {
                var client = new Client(
                   shopId: "988599",
                   secretKey: "live_6iz-Ys7cB0AlewjOYJ1OTb_ikkUSlRRJLLkcyFn2kX4");

                return client.GetPayment(paymnetId);
            }
            catch (Exception exp)
            {
                log.Error("YookassaController", "YooPaymentGet", exp.Message);
                return new Payment();
            }
        }
        #endregion

        #region ResponsPaymentWithWebApp
        //public async Task ResponsPaymentWithWebApp(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, SettingsModel botSettings)
        //{
        //    var message = new UpdateModel(update);

        //    var yooPayment = YooPaymentCreate(1, "");

        //    var userController = new Users.UserController();
        //    userController.InserPayment(message.Username, yooPayment.Id, (int)PaymentType.documents);

        //    await SendTextMessage(botClient, message.ChatId,
        //        "Нажмите кнопку для оплаты",
        //        new TelegramToolbars().CreateToolbarPayment("WithWebApp", yooPayment.Confirmation.ConfirmationUrl), cancellationToken);

        //    YooPaymentThread(yooPayment.Id, botClient, message, cancellationToken, botSettings);
        //}
        #endregion
    }
}
