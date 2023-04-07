using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialContractAssistant.Models
{
    internal class QuestionModel
    {
        public Int32 Id { get; set; }
        public String? Text { get; set; }
        public Int32 TypeId { get; set; }
        public Int32 ApplicationId { get; set; }
    }
}
