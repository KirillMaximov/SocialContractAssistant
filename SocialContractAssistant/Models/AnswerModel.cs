using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialContractAssistant.Models
{
    internal class AnswerModel
    { 
        public Int32 Id { get; set; }
        public Int64 ChatId { get; set; }
        public Int32 QuestionId { get; set; }
        public Int32 OptionId { get; set; }
        public virtual QuestionModel? Question { get; set; }
    }
}
