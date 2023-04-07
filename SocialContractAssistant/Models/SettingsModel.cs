using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialContractAssistant.Models
{
    internal class SettingsModel
    {
        public Int32 Id { get; set; }
        public Int32 ApplicationId { get; set; }
        public String? TelegramToken { get; set; }
        public String? PaymentTokenTest { get; set; }
        public String? PaymentTokenLife { get; set; }
        public Int32 PriceAmount { get; set; }
        public String? PriceLable { get; set; }
        public String? PriceTitle { get; set; }
        public String? PriceDescription { get; set; }
    }
}
