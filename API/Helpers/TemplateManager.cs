using System;

namespace CCBankWebAPI.Helpers
{
    public class TemplateManager
    {
        private static string _receiptTemplate { get; set; } = "<html><h3>-----Receipt-----</h3>Receipt date : {0} </br> Bill No.        : {1} </br>Scroll No     : {2}</br> Amount       : {3} </br><p>Peoples Urban co-op bank</p></html>";
        public static string GetReceipt(DateTime dateTime,string billno,string scrollno,string amount)
        {
            var receipt = string.Format(_receiptTemplate,dateTime,billno,scrollno,amount);
            return receipt;
        }
    }
}
