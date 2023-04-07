using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialContractAssistant.Models
{
    internal class LogModel
    {
        public Int32 Id { get; set; }
        public DateTime CreateDate { get; set; }
        public String Class { get; set; }
        public String Method { get; set; }
        public String Message { get; set; }
        public Int64 ChatId { get; set; }
    }
}
