using System;
using System.Globalization;
namespace CCBankWebAPI.Dtos
{
    public class CollectionStmtRequestDto
    {
        private string fromDate,toDate;        
        public int AgentId { get; set; }
        public int AcHeadId { get; set; }
        //public DateTime FromDate { get {  return Convert.ToDateTime(fromDate.ToString("yyyy-MM-dd")); } set { FromDate=value } }
        // public DateTime ToDate { get; set; }
        public int AccountNo { get; set; }

        public string FromDate
        {
            get
            {
                return DateTime.Parse(fromDate).ToString("yyyy-MM-dd",CultureInfo.InvariantCulture); 
            }

            set
            {
                fromDate = value;
            }
        }

        public string ToDate
        {
            get
            {
               return DateTime.Parse(toDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            set
            {
                toDate = value;
            }
        }
    }
}