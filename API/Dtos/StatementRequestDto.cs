using System;

namespace CCBankWebAPI.Dtos
{
    public class StatementRequestDto : RequestBase
    {
        public long DepositId { get; set; }

        public long AccountId { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public long AccountType { get; set; }
    }
}