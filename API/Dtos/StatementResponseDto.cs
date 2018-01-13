using System.Collections.Generic;

namespace CCBankWebAPI.Dtos
{
    public class StatementResponseDto
    {
        public StatementResponseDto()
        {
            MemberDetails = new StatementMemberModel();
            StatementDetails = new List<StatementModel>();
        }
        public StatementMemberModel MemberDetails { get; set; }
        public List<StatementModel> StatementDetails { get; set; }
        public decimal OpeningBalance { get; set; }
        public string BalanceAmt { get; set; }
    }
}