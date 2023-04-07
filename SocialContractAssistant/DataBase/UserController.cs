using SocialContractAssistant.DataAccess;
using SocialContractAssistant.Logs;
using SocialContractAssistant.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Yandex.Checkout.V3;

namespace SocialContractAssistant.Users
{
    internal class UserController
    {
        LogController log = new LogController();

        #region Users control

        internal Boolean UserExists(Int64 chatId)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var user = db.Users.FirstOrDefault(p => p.ChatId == chatId);
                    return user != null;
                }
            }
            catch (Exception exp)
            {
                log.Error("UserController", "UserExists", exp.Message, chatId);
                return false;
            }
        }

        internal Boolean UserPaymentSuccess(Int64 chatId)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var payment = db.Payments.FirstOrDefault(p => p.ChatId == chatId && p.IsActive);
                    return payment != null;
                }
            }
            catch (Exception exp)
            {
                log.Error("UserController", "UserPaymentSuccess", exp.Message, chatId);
                return false;
            }
        }

        internal void InserUser(TelegramModel telegramModel)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    db.Users.Add(new UserModel()
                    {
                        Username = telegramModel.message.username,
                        ChatId = telegramModel.message.chatId,
                        Created = DateTime.Now,
                        FirstName = telegramModel.message.firstName,
                        SecondName = telegramModel.message.secondName
                    });
                    db.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                log.Error("UserController", "InserUser", exp.Message, telegramModel.message.chatId);
            }
        }


        //Ответ Да (optionId = 10221) на вопрос 1022
        internal Boolean isCollegeStudent(Int64 chatId)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var isCollege = db.Answers.FirstOrDefault(p => p.ChatId == chatId && p.QuestionId == 1022 && p.OptionId == 10221);
                    return isCollege != null ? true : false;
                }
            }
            catch (Exception exp)
            {
                log.Error("UserController", "isCollegeStudent", exp.Message, chatId);
                return false;
            }
        }

        //Ответ Да (optionId = 10241) на вопрос 1024
        internal Boolean isUniversityStudent(Int64 chatId)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var isUniversity = db.Answers.FirstOrDefault(p => p.ChatId == chatId && p.QuestionId == 1024 && p.OptionId == 10241);
                    return isUniversity != null ? true : false;
                }
            }
            catch (Exception exp)
            {
                log.Error("UserController", "isUniversityStudent", exp.Message, chatId);
                return false;
            }
        }

        #endregion

        #region Answers control

        internal void InserAnswer(Int64 chatId, Int32 optionId)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var option = db.Options.FirstOrDefault(p => p.Id == optionId);
                    var answer = db.Answers.FirstOrDefault(p => p.ChatId == chatId && p.QuestionId == option!.QuestionId);

                    if (answer == null)
                    {
                        db.Answers.Add(new AnswerModel()
                        {
                            ChatId = chatId,
                            QuestionId = option!.QuestionId,
                            OptionId = option.Id
                        });
                    }
                    else
                    {
                        answer.OptionId = option!.Id;
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                log.Error("UserController", "InserAnswer", exp.Message, chatId);
            }
        }

        internal void DeleteAnswers(Int64 chatId, Int32 applicationId)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var answers = db.Answers.Where(p => p.ChatId == chatId && p.Question!.ApplicationId == applicationId);

                    db.Answers.RemoveRange(answers);
                    db.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                log.Error("UserController", "DeleteAnswers", exp.Message, chatId);
            }
        }

        internal AnswerModel DeleteBackAnswer(Int64 chatId, Int32 applicationId)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var backAnswer = db.Answers.OrderBy(p => p.Id).LastOrDefault(p => p.ChatId == chatId && p.Question!.ApplicationId == applicationId);

                    db.Answers.Remove(backAnswer!);
                    db.SaveChanges();

                    return backAnswer!;
                }
            }
            catch (Exception exp)
            {
                log.Error("UserController", "DeleteBackAnswer", exp.Message, chatId);
                return new AnswerModel();
            }
        }

        internal AnswerModel DeleteLastAnswer(Int64 chatId, Int32 applicationId)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var lastAnswer = db.Answers.OrderBy(p => p.Id).LastOrDefault(p => p.ChatId == chatId && p.Question!.ApplicationId == applicationId);

                    db.Answers.Remove(lastAnswer!);
                    db.SaveChanges();

                    return lastAnswer!;
                }
            }
            catch (Exception exp)
            {
                log.Error("UserController", "DeleteLastAnswer", exp.Message, chatId);
                return new AnswerModel();
            }
        }

        #endregion

        public void InserMessage(Int64 chatId, Int32 messageId, String? type = null)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var message = new MessageModel()
                    {
                        ChatId = chatId,
                        MessageId = messageId,
                        Type = type,
                        Created = DateTime.Now
                    };

                    db.Messages.Add(message);
                    db.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                log.Error("UserController", "InserMessage", exp.Message, chatId);
            }
        }

        public async Task DeletePreviousMessage(ITelegramBotClient botClient, Int64 chatId)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var message = db.Messages.FirstOrDefault(p => p.ChatId == chatId && p.Type != "Doc");

                    if (message != null)
                    {
                        await botClient.DeleteMessageAsync(message.ChatId, message.MessageId);

                        db.Messages.Remove(message);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("UserController", "DeletePreviousMessage", exp.Message, chatId);
            }
        }

        public async Task DeleteMessages(TelegramModel telegramModel)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var messages = db.Messages.Where(p => p.ChatId == telegramModel.message.chatId);

                    foreach (var item in messages)
                    {
                        try
                        {
                            await telegramModel.botClient.DeleteMessageAsync(telegramModel.message.chatId, item.MessageId);
                        }
                        catch (Exception) { }
                    }

                    db.Messages.RemoveRange(messages);
                    db.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                log.Error("UserController", "DeleteMessages", exp.Message, telegramModel.message.chatId);
            }
        }

        #region Payment control

        internal void InserPayment(Int64 chatId, String paymentId, Int32 typeId)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    db.Payments.Add(new Models.PaymentModel()
                    {
                        ChatId = chatId,
                        PaymentId = paymentId,
                        Status = (int)PaymentStatus.Pending,
                        TypeId = typeId,
                        Created = DateTime.Now,
                        IsActive = true
                    });
                    db.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                log.Error("UserController", "InserPayment", exp.Message, chatId);
            }
        }

        internal void UpdatePayment(String paymentId, PaymentStatus status)
        {
            Int64 chatId = 0;
            try
            {
                using (var db = new DataBaseContext())
                {
                    var payment = db.Payments.FirstOrDefault(p => p.PaymentId == paymentId && p.IsActive);

                    chatId = payment!.ChatId;

                    if (payment != null)
                    {
                        payment.Status = (int)status;
                        payment.Updated = DateTime.Now;
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                log.Error("UserController", "UpdatePayment", exp.Message, chatId);
            }
        }

        internal void UpdatePayment(String paymentId, Boolean isActive)
        {
            Int64 chatId = 0;
            try
            {
                using (var db = new DataBaseContext())
                {
                    var payment = db.Payments.FirstOrDefault(p => p.PaymentId == paymentId && p.IsActive);
                    
                    chatId = payment!.ChatId;

                    if (payment != null)
                    {
                        payment.IsActive = isActive;
                        payment.Updated = DateTime.Now;
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                log.Error("UserController", "UpdatePayment", exp.Message, chatId);
            }
        }

        internal String? GetCheckPaymentId(Int64 chatId, Int32 typeId)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var lastPayment = db.Payments.OrderBy(p => p.Created).LastOrDefault(p => p.ChatId == chatId && p.IsActive && p.TypeId == typeId);

                    return lastPayment != null ? lastPayment.PaymentId : null;
                }
            }
            catch (Exception exp)
            {
                log.Error("UserController", "GetCheckPaymentId", exp.Message, chatId);
                return String.Empty;
            }
        }

        internal String GetUsername(string paymentId)
        {
            Int64 chatId = 0;
            try
            {
                using (var db = new DataBaseContext())
                {
                    var payment = db.Payments.FirstOrDefault(p => p.PaymentId == paymentId);
                    var user = db.Users.FirstOrDefault(p => p.ChatId == payment!.ChatId);

                    chatId = payment!.ChatId;

                    var name = String.Empty;

                    if (user!.Username != null) name += user.Username;
                    if (user.FirstName != null) name += " " + user.FirstName;
                    if (user.SecondName != null) name += " " + user.SecondName;

                    return name;
                }
            }
            catch (Exception exp)
            {
                log.Error("UserController", "GetUsername", exp.Message, chatId);
                return $"Пользователь не найден по paymentId = {paymentId}";
            }
        }

        #endregion
    }
}
