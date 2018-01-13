namespace CCBankWebAPI.Dtos
{
    public class CheckBookRequestDto:RequestBase
    {
        public long DepositId { get; set; }
        public long AccountTypeId { get; set; }
    }
}