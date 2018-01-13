using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCBankWebAPI.Dtos
{
    public class ServiceRequestModel
    {
        public IList<ServicesModel> ServiceStatus { get; set; }

    }
}
