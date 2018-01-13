namespace CCBankWebAPI.Dtos
{
    public class AccountSummaryModel
    {
        public string AcccountName { get; set; }
        public string AccountNr { get; set; }
        public string AccountStartedOn { get; set; }
        public string CurrentBalance { get; set; }
        public long DepositId { get; set; }
        public string BranchCode { get; set; }
    }
}