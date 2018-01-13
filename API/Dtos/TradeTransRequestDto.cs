using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCBankWebAPI.Dtos
{
    public class TradeTransRequestDto : RequestBase
    {
        public AccountHeadModel FromAccount { get; set; }
        public decimal Amount { get; set; }
        public string OTP { get; set; }
        public string TradeCode { get; set; }
        public string TraderName { get; set; }
        public string TraderAddress { get; set; }
        public string TraderId { get; set; }
        public string RefNo { get; set; }


    }
}
