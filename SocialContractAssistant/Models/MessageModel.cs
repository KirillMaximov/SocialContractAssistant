using SocialContractAssistant.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialContractAssistant.Models
{
    internal class MessageModel
    {
        public Int32 Id { get; set; }
        public Int64 ChatId { get; set; }
        public Int32 MessageId { get; set; }
        public String? Type { get; set; }
        public DateTime Created { get; set; }
    }
}
