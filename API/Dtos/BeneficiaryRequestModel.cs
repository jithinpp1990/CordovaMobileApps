using CCBankWebAPI.Infrastructure;

namespace CCBankWebAPI.Dtos
{
    public class BeneficiaryRequestModel : RequestBase
    {
        public string TransferType { get; set; }
        public int BeneficiaryId { get; set; }
    }
}