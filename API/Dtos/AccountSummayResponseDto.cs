using System.Collections.Generic;

namespace CCBankWebAPI.Dtos
{
    public class AccountSummayResponseDto
    {
        public AccountSummayResponseDto()
        {
            AccountSummaryList = new List<AccountSummaryModel>();
            MemberDetails = new MemberModel();
        }
        public List<AccountSummaryModel> AccountSummaryList { get; set; }
        public MemberModel MemberDetails { get; set; }
    }
}