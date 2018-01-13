
namespace CCBankWebAPI.Dtos
{
    public class CustomerDetDto : AcHeadDto
    {
        public int LedgerId { get; set; }
        public string AccountNo { get; set; }
        public int CustomerMemberId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerEmail { get; set; }
        public string Remarks { get; set; } = null;
    }
}