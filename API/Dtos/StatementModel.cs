using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CCBankWebAPI.Dtos
{
    public class StatementModel
    {
        public string  ScrollDate { get; set; }
        public string ScrollNr { get; set; }
        public string ChequeNr { get; set; }
        public string CashOrTransfer { get; set; }
        public string Particulars { get; set; }
        public decimal Deposit { get; set; }
        public decimal Withdrawal { get; set; }
        public decimal Balance { get; set; }
        public string DrCr { get; set; }
        public string AccountNo { get; set; }
    }
}