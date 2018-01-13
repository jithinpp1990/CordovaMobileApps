namespace CCBankWebAPI.Dtos
{
    public class VerifyCustomerRequestDto
    {
        public string CustomerId { get; set; }
        public string AgentCode { get; set; }
        public string OTP { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
        public string DeviceId { get; set; }
        public string Type { get; set; }
    }
}