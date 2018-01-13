using System.Collections.Generic;

namespace CCBankWebAPI.Dtos
{
    public class BranchDetailsDto
    {
        public int BranchId { get; set; }

        public string BranchName { get; set; }

        public IList<string> Address { get; set; } = new List<string>();

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Manager { get; set; }

        public string ManagerContact { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }
}