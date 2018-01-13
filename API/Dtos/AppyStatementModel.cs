namespace CCBankWebAPI.Dtos
{
    public class AppyStatementModel
    {
        public StatementResponseDto ResponseStatement { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int depositId { get; set; }
    }
}