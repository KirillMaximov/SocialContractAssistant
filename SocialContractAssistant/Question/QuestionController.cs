using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialContractAssistant.DataAccess;
using SocialContractAssistant.Documents;
using SocialContractAssistant.Enums;
using SocialContractAssistant.Logs;
using SocialContractAssistant.Models;
using SocialContractAssistant.Telegram;
using SocialContractAssistant.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocialContractAssistant.Questions
{
    internal class QuestionController : TelegramToolbars
    {
        LogController log = new LogController();
        public SenderModel GetStartQuestion(Int32 applicationId)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    //Находим данные по начальному вопросу
                    var question = db.Questions.FirstOrDefault(q => q.TypeId == (int)QuestionType.start && q.ApplicationId == applicationId);
                    var questionId = question!.Id;
                    var questionText = question!.Text!;

                    //Создаем меню для вопроса
                    var replyMarkup = CreateToolbar(questionId);

                    return new SenderModel(questionText, replyMarkup);
                }
            }
            catch (Exception exp)
            {
                log.Error("QuestionController", "GetStartQuestion", exp.Message);
                return new SenderModel();
            }
        }
    
        public SenderModel GetNextQuestion(Int32? questionId)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    //Находим данные по следующему вопросу
                    var nextQuestion = db.Questions.FirstOrDefault(q => q.Id == questionId);
                    var nextQuestionId = nextQuestion!.Id;
                    var nextQuestionText = nextQuestion!.Text;

                    //Создаем меню для следующего вопроса
                    var replyMarkup = new TelegramToolbars().CreateToolbar(nextQuestionId);

                    return new SenderModel(nextQuestionText!, replyMarkup);
                }
            }
            catch (Exception exp)
            {
                log.Error("QuestionController", "GetNextQuestion", exp.Message);
                return new SenderModel();
            }
        }
    
        public SenderModel GetBackQuestion(Int64 chatId, Int32 applicationId)
        {
            try
            {
                var userController = new UserController();

                var backAnswer = userController.DeleteBackAnswer(chatId, applicationId);
                var lastAnswer = userController.DeleteLastAnswer(chatId, applicationId);

                using (var db = new DataBaseContext())
                {
                    //Находим данные по следующему вопросу
                    var nextQuestion = db.Questions.FirstOrDefault(q => q.Id == lastAnswer!.QuestionId);
                    var nextQuestionId = nextQuestion!.Id;
                    var nextQuestionText = nextQuestion!.Text;

                    //Создаем меню для следующего вопроса
                    var replyMarkup = new TelegramToolbars().CreateToolbar(nextQuestionId);

                    return new SenderModel(nextQuestionText!, replyMarkup);
                }
            }
            catch (Exception exp)
            {
                log.Error("QuestionController", "GetBackQuestion", exp.Message);
                return new SenderModel();
            }
        }

        public SenderModel GetAfterQuestion(Int32 applicationId)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    //Находим данные по вопросу после оплаты
                    var question = db.Questions.FirstOrDefault(q => q.TypeId == (int)QuestionType.after && q.ApplicationId == applicationId);
                    var questionId = question!.Id;
                    var questionText = question!.Text;

                    //Создаем меню для вопроса
                    var replyMarkup = new TelegramToolbars().CreateToolbar(questionId);

                    return new SenderModel(questionText!, replyMarkup);
                }
            }
            catch (Exception exp)
            {
                log.Error("QuestionController", "GetAfterQuestion", exp.Message);
                return new SenderModel();
            }
        }

        public OptionModel GetOption(String buttonData)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var optionId = Convert.ToInt32(buttonData);
                    return db.Options.Include(p => p.Question).FirstOrDefault(p => p.Id == optionId)!;
                }
            }
            catch (Exception exp)
            {
                log.Error("QuestionController", "GetOption", exp.Message);
                return new OptionModel();
            }
        }
    }
}
