
namespace CCBankWebAPI.Dtos
{
    public class AccountNoValidateRequestDto : AcHeadDto
    {
        public string AccountNo { get; set; }
        public string ReadMethod { get; set; }
        public string QRCodeData { get; set; }
    }
}