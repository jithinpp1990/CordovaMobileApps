using System.Collections.Generic;
namespace CCBankWebAPI.Dtos
{
    public class AgentProfile
    {
        public MemberModel AgentPersonalaData { get; set; }
        public List<AgentAcSummaryDto> AgentAccountSummary { get; set; }
    }
}