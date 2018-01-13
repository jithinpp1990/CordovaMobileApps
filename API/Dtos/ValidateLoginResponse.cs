namespace CCBankWebAPI.Dtos
{
    public class ValidateLoginResponse
    {
        public long MemberId { get; set; }
        public string ErrorMessage { get; set; }
        public string SessionToken { get; set; }
        public bool InvalidMobileNr { get; set; }
        public string appid { get; set; }
    }
}