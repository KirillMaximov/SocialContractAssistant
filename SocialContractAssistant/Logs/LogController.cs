using SocialContractAssistant.DataAccess;
using SocialContractAssistant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialContractAssistant.Logs
{
    internal class LogController
    {
        public void Error(String _class, String _method, String _message, Int64 chatId = 0)
        {
            using (var db = new DataBaseContext())
            {
                var log = new LogModel();

                log.CreateDate= DateTime.Now;
                log.Class = _class;
                log.Method = _method;
                log.Message = _message;
                log.ChatId = chatId;

                db.Logs.Add(log);
                db.SaveChanges();
            }
        }
    }
}
