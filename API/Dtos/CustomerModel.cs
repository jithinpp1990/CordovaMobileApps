using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCBankWebAPI.Dtos
{
    public class CustomerModel:RequestBase
    {
        public string CustomerMemberId { get; set; }
    }
}
