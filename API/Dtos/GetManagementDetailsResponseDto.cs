using System.Collections.Generic;

namespace CCBankWebAPI.Dtos
{
    public class GetManagementDetailsResponseDto
    {
        public IList<ManagementDetailsDto> ManagementDetailsList { get; set; } = new List<ManagementDetailsDto>();
    }
}