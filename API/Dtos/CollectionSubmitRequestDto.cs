namespace CCBankWebAPI.Dtos
{
    public class CollectionSubmitRequestDto : RequestBase
    {
        public int AcHeadId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string OTP { get; set; }
        public string RefNo { get; set; }
    }
}