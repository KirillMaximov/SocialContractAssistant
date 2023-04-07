using SocialContractAssistant.DataAccess;
using SocialContractAssistant.Logs;
using SocialContractAssistant.Models;
using SocialContractAssistant.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SocialContractAssistant.Documents
{
    internal class DocumentController
    {
        LogController log = new LogController();
        public void CreateInPdf(String fileName, Int64 chatId, Int32 applicationId)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var answers = db.Answers.Where(p => p.ChatId == chatId && p.Question!.ApplicationId == applicationId);
                    new BytescountPdf().CreateDocument(fileName, answers);
                }
            }
            catch (Exception exp)
            {
                log.Error("DocumentController", "CreateInPdf", exp.Message, chatId);
            }
        }

        public void CreateInTxt(String fileName, Int64 chatId, Int32 applicationId)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var answers = db.Answers.Where(p => p.ChatId == chatId && p.Question!.ApplicationId == applicationId);
                    var documents = new NotepadeTxt().CreateDocumentData(answers);
                    new NotepadeTxt().CreateDocumentsTxt(fileName, documents);
                }
            }
            catch (Exception exp)
            {
                log.Error("DocumentController", "CreateInTxt", exp.Message, chatId);
            }
        }

        public void CreateInPdfTest(String fileName, Int64 chatId)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    new BytescountPdf().CreateDocumentTest(fileName);
                }
            }
            catch (Exception exp)
            {
                log.Error("DocumentController", "CreateInPdfTest", exp.Message, chatId);
            }
        }
    }
}
