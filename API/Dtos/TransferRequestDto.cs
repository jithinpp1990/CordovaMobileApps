using CCBankWebAPI.Infrastructure;

namespace CCBankWebAPI.Dtos
{
    public class TransferRequestDto : RequestBase
    {
        public AccountHeadModel FromAccount { get; set; }
        public BeneficiaryModel ToAccount { get; set; }
        public decimal Amount { get; set; }
        public string OTP { get; set; }
        public TransferType TransferType { get; set; }
        public RequestMode RequestMode { get; set; }
    }
}