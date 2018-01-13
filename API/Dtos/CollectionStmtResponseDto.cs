
namespace CCBankWebAPI.Dtos
{
    public class CollectionStmtResponseDto
    {
        public string AccountNo { get; set; }
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public string Date { get; set; }
        public string AcHeadName { get; set; }
    }
}