using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCBankWebAPI.Dtos
{
   public class TraderRequestModel : RequestBase
    {
        public string TradeCode { get; set; }
        public string TraderId { get; set; }
        public string TraderName { get; set; }
        public string TraderAddress { get; set; }
        public string TraderMobile { get; set; }
        public string QRCodeData { get; set; }

    }
}
