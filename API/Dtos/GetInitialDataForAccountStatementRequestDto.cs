namespace CCBankWebAPI.Dtos
{
    public class GetInitialDataForAccountStatementRequestDto:RequestBase
    {
        public GetInitialDataForAccountStatementRequestDto()
        {
            AccountHeadModel = new CheckBookRequestDto();
        }
        public CheckBookRequestDto AccountHeadModel { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
    }
}