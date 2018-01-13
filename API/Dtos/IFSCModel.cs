namespace CCBankWebAPI.Dtos
{
    public class IFSCModel : RequestBase
    {
        public string IFSC { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
    }
}