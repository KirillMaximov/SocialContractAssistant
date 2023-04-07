using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialContractAssistant.Models
{
    internal class UserModel
    {
        public Int64 ChatId { get; set; }
        public DateTime Created { get; set; }
        public String? Username { get; set; }
        public String? FirstName { get; set; }
        public String? SecondName { get; set; }
    }
}
