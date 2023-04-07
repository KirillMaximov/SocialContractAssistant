using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using SocialContractAssistant.Models;
using Telegram.Bot.Types.InputFiles;
using SocialContractAssistant.Users;
using System.Threading;

namespace SocialContractAssistant.Telegram
{
    internal class TelegramSender
    {
        /// <summary>
        /// Отправка нового сообщения в телеграм;
        /// </summary>
        /// <param name="chatId">Номер чата в который отправляем сообщение</param>
        /// <param name="messageText">Текст сообщения</param>
        /// <param name="replyMarkup">Меню сообщения</param>
        /// <returns></returns>
        protected async Task SendTextMessage(
            TelegramModel telegramModel,
            string messageText, 
            IReplyMarkup replyMarkup)
        {
            Message sendMessage = await telegramModel.botClient.SendTextMessageAsync(
                chatId: telegramModel.message.chatId,
                text: messageText,
                replyMarkup: replyMarkup,
                cancellationToken: telegramModel.cancellationToken); //parseMode: ParseMode.Html (для добавления html текста с форматированием)

            await new UserController().DeletePreviousMessage(telegramModel.botClient, telegramModel.message.chatId);
            new UserController().InserMessage(sendMessage.Chat.Id, sendMessage.MessageId);
        }

        protected async Task SendTextMessage(
            ITelegramBotClient botClient,
            long chatId,
            string messageText,
            CancellationToken cancellationToken)
        {
            Message sendMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: messageText,
                cancellationToken: cancellationToken);
        }

        protected async Task SendTextMessage(
            TelegramModel telegramModel,
            string messageText)
        {
            Message sendMessage = await telegramModel.botClient.SendTextMessageAsync(
                chatId: telegramModel.message.chatId,
                text: messageText,
                cancellationToken: telegramModel.cancellationToken);

            await new UserController().DeletePreviousMessage(telegramModel.botClient, telegramModel.message.chatId);
            new UserController().InserMessage(sendMessage.Chat.Id, sendMessage.MessageId);
        }

        /// <summary>
        /// Отправка измененного сообщения в телеграм взамен текущему;
        /// </summary>
        /// <param name="chatId">Номер чата в котором изменяем сообщение</param>
        /// <param name="messageId">Номер сообщения которые будем изменять</param>
        /// <param name="messageText">Текст измененного сообщения</param>
        /// <param name="replyMarkup">Меню измененного сообщения</param>
        /// <returns></returns>
        protected async Task EditMessageText(
            TelegramModel telegramModel,
            string messageText, 
            InlineKeyboardMarkup replyMarkup)
        {
            Message sendMessage = await telegramModel.botClient.EditMessageTextAsync(
                chatId: telegramModel.message.chatId,
                messageId: telegramModel.message.messageId,
                text: messageText,
                replyMarkup: replyMarkup,
                cancellationToken: telegramModel.cancellationToken);
        }

        /// <summary>
        /// Отправка ссылки на оплату услуг;
        /// </summary>
        /// <param name="chatId">Номер чата в который отправляем сообщение</param>
        /// <param name="title">Гллавление оплаты</param>
        /// <param name="description">Описание оплаты</param>
        /// <param name="token">Ключ в системе платежей</param>
        /// <param name="prices">List<LabeledPrice> список цен на услуги</param>
        /// <param name="currency">Валюта для оплаты</param>
        /// <returns></returns>
        protected async Task SendInvoice(
            TelegramModel telegramModel,
            string title, 
            string description, 
            string token, 
            List<LabeledPrice> prices, 
            string currency = "RUB")
        {
            //https://core.telegram.org/bots/payments#supported-currencies
            Message sendMessage = await telegramModel.botClient.SendInvoiceAsync(
                chatId: telegramModel.message.chatId,
                title: title,
                description: description,
                providerToken: token,
                currency: currency,
                prices: prices,
                startParameter: "payment_for_services",
                payload: "test_invoice_payload");

            await new UserController().DeletePreviousMessage(telegramModel.botClient, telegramModel.message.chatId);
            new UserController().InserMessage(sendMessage.Chat.Id, sendMessage.MessageId);
        }

        protected async Task SendDocument(
            TelegramModel telegramModel,
            string sourceFile,
            string telegramName, 
            string caption)
        {
            using (var stream = System.IO.File.OpenRead(sourceFile))
            {
                InputOnlineFile documentFile = new InputOnlineFile(stream);
                documentFile.FileName = telegramName;

                Message sendMessage = await telegramModel.botClient.SendDocumentAsync(
                    chatId: telegramModel.message.chatId, 
                    document: documentFile, 
                    caption: caption);

                await new UserController().DeletePreviousMessage(telegramModel.botClient, telegramModel.message.chatId);
                new UserController().InserMessage(sendMessage.Chat.Id, sendMessage.MessageId, "Doc");
            }
        }
    }
}
