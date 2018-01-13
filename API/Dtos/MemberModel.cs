using System.Collections.Generic;

namespace CCBankWebAPI.Dtos
{
    public class MemberModel
    {
        public MemberModel()
        {
            AddressLine = new List<string>();
        }
        public string MembershipNo { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public IList<string> AddressLine { get; set; }
    }
}