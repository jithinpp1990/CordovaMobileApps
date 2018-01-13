using CCBankWebAPI.Infrastructure;

namespace CCBankWebAPI.Dtos
{
    public class BeneficiaryModel : RequestBase
    {
        public int Id { get; set; }
        public string BeneficiaryName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AccountNumber { get; set; }
        public string ConfirmAccountNumber { get; set; }
        public string AccountType { get; set; }
        public string BankBranch { get; set; }
        public string BankName { get; set; }
        public int MaxLimit { get; set; }
        public string OTP { get; set; }
        public string IFSC { get; set; }
        public string TransferType { get; set; }
        public RequestMode RequestMode { get; set; }
        public string BeneficiaryType { get; set; }
        public bool IsEditable { get; set; }
    }
}