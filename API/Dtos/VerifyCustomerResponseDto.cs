namespace CCBankWebAPI.Dtos
{
    public class VerifyCustomerResponseDto
    {
        public bool ValidCustomer { get; set; }

        public string ErrorMessage { get; set; }
    }
}