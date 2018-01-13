using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCBankWebAPI.Dtos
{
    public class SMSContentModel:TradeTranResponseModel
    {
        public string status { get; set; }
        public string CustomerMessage { get; set; }
        public string TraderMessage { get; set; }
        public string CustomerMobile { get; set; }
        public string TraderMobile { get; set; }
        public string CustomerEmail { get; set; }
        public string TraderEmail { get; set; }
    }
}
