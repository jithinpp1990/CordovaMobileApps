using System.Collections.Generic;

namespace CCBankWebAPI.Dtos
{
    public class ChequeBookResponseDto
    {
        public ChequeBookResponseDto()
        {
            ModeOfAccount = new List<AccountHeadModel>();
        }
        public IList<AccountHeadModel> ModeOfAccount { get; set; }

        public long depositId { get; set; }

        public int StmtFromDateDiff { get; set; }
        public string ReceiptHtml { get; set; }
    }
}