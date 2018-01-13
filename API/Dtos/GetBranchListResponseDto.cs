using System.Collections.Generic;

namespace CCBankWebAPI.Dtos
{
    public class GetBranchListResponseDto
    {
        public GetBranchListResponseDto()
        {
            BranchDetailsList = new List<BranchDetailsDto>();
        }
        public IList<BranchDetailsDto> BranchDetailsList { get; set; }
    }
}