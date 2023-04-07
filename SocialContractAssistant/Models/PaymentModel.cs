using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialContractAssistant.Models
{
    internal class PaymentModel
    {
        public Int32 Id { get; set; }
        public Int64 ChatId { get; set; }
        public String? PaymentId { get; set; }
        public Int32 Status { get; set; }
        public Int32 TypeId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public Boolean IsActive { get; set; }
    }
}
