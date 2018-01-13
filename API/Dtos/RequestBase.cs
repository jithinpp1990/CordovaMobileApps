namespace CCBankWebAPI.Dtos
{
    public class RequestBase
    {
        public string SessionToken { get; set; }

        public string Username { get; set; }

        public string UserId { get; set; }

        public string DeviceId { get; set; }

        public string MemberId { get; set; }

        public string Service { get; set; }
        public string AppId { get; set; }

        public string AgentId { get; set; }
    }
}