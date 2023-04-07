using SocialContractAssistant.DataAccess;
using SocialContractAssistant.Logs;
using SocialContractAssistant.Models;
using System.Text;

namespace SocialContractAssistant.Documents
{
    internal class DocumentController
    {
        LogController log = new LogController();

        public void Create(IDocuments docType, String fileName, Int64 chatId, Int32 applicationId)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    var answers = db.Answers.Where(p => p.ChatId == chatId && p.Question!.ApplicationId == applicationId);
                    docType.CreateDocuments(fileName, GetDocuments(answers).ToString());
                }
            }
            catch (Exception exp)
            {
                log.Error("DocumentController", "Create", exp.Message, chatId);
            }
        }

        public void CreateTest(IDocuments docType, String fileName)
        {
            try
            {
                using (var db = new DataBaseContext())
                {
                    docType.CreateDocuments(fileName, GetAllDocuments().ToString());
                }
            }
            catch (Exception exp)
            {
                log.Error("DocumentController", "CreateTest", exp.Message);
            }
        }

        #region CreateDocuments

        private StringBuilder GetDocuments(IQueryable<AnswerModel> answers)
        {
            StringBuilder docs = new StringBuilder();
            List<Int32> addedDocuments = new List<Int32>();
            try
            {
                using (var db = new DataBaseContext())
                {
                    foreach (var item in answers)
                    {
                        var option = db.Options.FirstOrDefault(p => p.Id == item.OptionId);
                        var linkDocuments = db.LinkDocuments.Where(p => p.LinkId == option!.LinkDocumentId);

                        using (var dblink = new DataBaseContext())
                        {
                            foreach (var link in linkDocuments)
                            {
                                var document = dblink.Documents.FirstOrDefault(p => p.Id == link.DocumentId && !addedDocuments.Contains(p.Id));
                                if (document != null)
                                {
                                    docs.AppendLine(document.Name);

                                    if (!String.IsNullOrEmpty(document.Description))
                                    {
                                        docs.AppendLine(document.Description);
                                    }

                                    addedDocuments.Add(document.Id);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("BytescountPdf", "GetDocuments", exp.Message);
            }
            return docs;
        }

        private StringBuilder GetAllDocuments()
        {
            StringBuilder docs = new StringBuilder();
            try
            {
                using (var dblink = new DataBaseContext())
                {
                    var documents = dblink.Documents;

                    foreach (var document in documents)
                    {
                        if (document != null)
                        {
                            docs.AppendLine(document.Name);

                            if (!String.IsNullOrEmpty(document.Description))
                            {
                                docs.AppendLine(document.Description);
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("NotepadeTxt", "CreateDocumentData", exp.Message);
            }
            return docs;
        }

        #endregion
    }
}
