using SocialContractAssistant.DataAccess;
using SocialContractAssistant.Logs;
using SocialContractAssistant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace SocialContractAssistant.Documents
{
    internal class NotepadeTxt
    {
        LogController log = new LogController();
        public void CreateDocumentsTxt(String fileName, String documents)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                using (FileStream fs = File.Create(fileName))
                {
                    byte[] docs = new UTF8Encoding(true).GetBytes(documents);
                    fs.Write(docs, 0, docs.Length);
                }
            }
            catch (Exception exp)
            {
                log.Error("NotepadeTxt", "CreateDocumentsTxt", exp.Message);
            }
        }

        public String CreateDocumentData(IQueryable<AnswerModel> answers)
        {
            var documents = String.Empty;
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
                                var document = dblink.Documents.FirstOrDefault(p => p.Id == link.DocumentId);
                                if (document != null)
                                {
                                    documents += document.Name + Environment.NewLine;
                                    if (!String.IsNullOrEmpty(document.Description))
                                    {
                                        documents += document.Description + Environment.NewLine;
                                    }
                                    documents += Environment.NewLine;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("NotepadeTxt", "CreateDocumentData", exp.Message);
            }
            return documents;
        }
    }
}
