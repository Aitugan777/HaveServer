using Microsoft.EntityFrameworkCore;

namespace AitukServer.Models
{
    [Owned]
    public class AWorkDay
    {
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool IsWorkingDay { get; set; }
    }
}
