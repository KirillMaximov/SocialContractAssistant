using SocialContractAssistant.Logs;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Payments;

namespace SocialContractAssistant.Models
{
    internal class UpdateModel
    {
        public long chatId { get; }
        public int messageId { get; }
        public string? messageText { get; }
        public string? username { get; }
        public string? firstName { get; }
        public string? secondName { get; }
        public string? buttonData { get; }
        public SuccessfulPayment? payment { get; }

        LogController log = new LogController();
        public UpdateModel(Update update)
        {
            try
            {
                if (update.Message != null)
                {
                    chatId = update.Message.Chat.Id;
                    messageText = update.Message.Text!;
                    username = update.Message.From!.Username!;
                    firstName = update.Message.From.FirstName;
                    secondName = update.Message.From.LastName!;
                    payment = update.Message.SuccessfulPayment!;
                }
                if (update.CallbackQuery != null)
                {
                    chatId = update.CallbackQuery.Message!.Chat.Id;
                    messageId = update.CallbackQuery.Message.MessageId;
                    username = update.CallbackQuery.From!.Username!;
                    buttonData = update.CallbackQuery.Data!;
                }
            }
            catch (Exception exp)
            {
                log.Error("UpdateModel", "UpdateModel", exp.Message);
            }
        }
    }
}
