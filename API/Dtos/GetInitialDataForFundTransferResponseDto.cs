using System.Collections.Generic;

namespace CCBankWebAPI.Dtos
{
    public class GetInitialDataForFundTransferResponseDto
    {
        public IList<AccountHeadModel> FromAccounts { get; set; } = new List<AccountHeadModel>();
        public IList<BeneficiaryModel> ToAccounts { get; set; } = new List<BeneficiaryModel>();
    }
}