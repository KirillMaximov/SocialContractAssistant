using Telegram.Bot;
using Telegram.Bot.Types;

namespace SocialContractAssistant.Models
{
    internal class TelegramModel
    {
        public ITelegramBotClient botClient { get; }
        public UpdateModel message { get; }
        public CancellationToken cancellationToken { get; }

        public TelegramModel(ITelegramBotClient _botClient, Update _update, CancellationToken _cancellationToken)
        {
            botClient = _botClient;
            message = new UpdateModel(_update);
            cancellationToken = _cancellationToken;
        }
    }
}
