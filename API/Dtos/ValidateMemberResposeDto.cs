namespace CCBankWebAPI.Dtos
{
    public class ValidateMemberResposeDto 
    {      
        public bool AuthenticationSuccess { get; set; }

        public string ErrorMessage { get; set; }

        public RequestBase SessionToken { get; set; }
    }
}