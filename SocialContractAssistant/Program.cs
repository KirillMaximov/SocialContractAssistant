using Telegram.Bot;
using SocialContractAssistant.CreateBot;

#region Создание ботов для нужного приложения

//var socialContractApplicationId = 1; //Social contract
//SocialStipend socialContractBot = new SocialStipend(socialContractApplicationId);
//var socialContractBotName = await socialContractBot.BotClient.GetMeAsync();
//Console.WriteLine($"Начинаем работу с @{socialContractBotName.Username}");

var socialStipendApplicationId = 2; //Social stipend
InterviewBot socialStipendBot = new InterviewBot(socialStipendApplicationId);
var socialStipendBotName = await socialStipendBot.botClient.GetMeAsync();
Console.WriteLine($"Начинаем работу с @{socialStipendBotName.Username}");

#endregion

#region Задержка для работы консоли

await Task.Delay(int.MaxValue);

#endregion

#region Удаление токена после работы

//socialContractBot.Cancellation.Cancel();
socialStipendBot.cancellation.Cancel();

#endregion