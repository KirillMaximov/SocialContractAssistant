using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialContractAssistant.Models
{
    internal class OptionModel
    {
        public Int32 Id { get; set; }
        public Int32 QuestionId { get; set; }
        public String? Text { get; set; }
        public Int32? NextQuestionId { get; set; }
        public Int32? LinkDocumentId { get; set; }

        public virtual QuestionModel? Question { get; set; }
        public virtual QuestionModel? NextQuestion { get; set; }
        public virtual LinkDocumentModel? LinkDocument { get; set; }
    }
}
