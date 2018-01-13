using System.Collections.Generic;

namespace CCBankWebAPI.Dtos
{
    public class NotificationsDto
    {
        public IList<string> Notifications { get; set; } = new List<string>();
    }
}