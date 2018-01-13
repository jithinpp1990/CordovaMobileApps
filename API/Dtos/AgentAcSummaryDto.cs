namespace CCBankWebAPI.Dtos
{
    public class AgentAcSummaryDto: AcHeadDto
    {
        public string AccountNo { get; set; }
        public decimal Balance { get; set; }
    }
}