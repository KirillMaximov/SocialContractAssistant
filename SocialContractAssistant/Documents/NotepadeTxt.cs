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
    internal class NotepadeTxt : IDocuments
    {
        LogController log = new LogController();

        public void CreateDocuments(String fileName, String documents)
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
                log.Error("NotepadeTxt", "CreateDocuments", exp.Message);
            }
        }
    }
}
