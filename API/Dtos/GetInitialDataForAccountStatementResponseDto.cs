namespace CCBankWebAPI.Dtos
{
    public class GetInitialDataForAccountStatementResponseDto
    {
        public GetInitialDataForAccountStatementResponseDto()
        {
            ApplyStatementModel = new AppyStatementModel();
        }
        public AppyStatementModel ApplyStatementModel { get; set; }
    }   
}